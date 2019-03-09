using UnityEngine;
using System;
using WiimoteApi.Internal;
using WiimoteApi.Util;

namespace WiimoteApi {

    public delegate void ReadResponder(byte[] data);

public class CS_WiiMote
{
        public bool bRumbleOn = false;
    // If a Nunchuck is currently connected to the Wii Remote's extension port,
    // this contains all relevant Nunchuck controller data as it is reported by
    // the Wiimote.  If no Nunchuck is connected, this is null.
    public CS_NunchuckData Nunchuck {
        get {
            if(current_ext == ExtensionController.NUNCHUCK)
                return (CS_NunchuckData)_Extension;
            return null;
        }
    }

    private CS_WiiMoteData _Extension;

    /// Status info data component.
    public CS_StatusData    Status     { get { return _Status; } }
    private CS_StatusData  _Status;

    // The RAW extension data reported by the Wii Remote.
    // Used for debugging new extension controllers.
    public CS_ReadOnlyArray<byte> RawExtension { get { return _RawExtension; } }
    private CS_ReadOnlyArray<byte> _RawExtension = null;

    // A pointer representing HIDApi's low-level device handle to this
    // Wii Remote.  Used when interfacing directly with HIDApi.
    public IntPtr hidapi_handle { get { return _hidapi_handle; } }
    private IntPtr _hidapi_handle = IntPtr.Zero;

    // The low-level bluetooth HID path of the WiiMote.
    // Used when interfacing directly with HIDApi.
    public string hidapi_path { get { return _hidapi_path; } }
    private string _hidapi_path;

    // Holds and handles the type of wiiMote
    // Currently only standard is accepted
    public WiimoteType Type { get { return _Type; } }
    private WiimoteType _Type;

    private CS_RegisterReadData CurrentReadData = null;

    // The current extension connected to the Wii Remote.  
    // Updated when the Wii Remote reports an extension change
    public ExtensionController current_ext { get { return _current_ext; } }
    private ExtensionController _current_ext = ExtensionController.NONE;

    public CS_WiiMote(IntPtr hidapi_handle, string hidapi_path, WiimoteType Type)
    {
        _hidapi_handle  = hidapi_handle;
        _hidapi_path    = hidapi_path;
        _Type    = Type;
        _Status = new CS_StatusData(this);
        _Extension = null;
    }
    // Two types of nunchuck IDS
    private const long ID_Nunchuck                  = 0x0000A4200000;
    private const long ID_Nunchuck2                 = 0xFF00A4200000;
    // Identifies if the extension in the wiimote is a Nunchuck or not
    // Will not recognise any othe extension
    private void RespondIdentifyExtension(byte[] data)
    {
        if (data.Length != 6)
            return;

        byte[] resized = new byte[8];
        for (int x = 0; x < 6; x++) resized[x] = data[5-x];
        long val = BitConverter.ToInt64(resized, 0);

       
        if (val == ID_Nunchuck || val == ID_Nunchuck2)
        {
            _current_ext = ExtensionController.NUNCHUCK;
            if (_Extension == null || _Extension.GetType() != typeof(CS_NunchuckData))
                _Extension = new CS_NunchuckData(this);
        }
        else
        {
            _current_ext = ExtensionController.NONE;
            _Extension = null;
        }
    }

    #region Setups

    // Returns true if all setup IR commands are sent to the wiimote succeessfully.
    // Updates the Wii Remote's data reporting mode based according to the basic IR settings.
    public bool SetupIRCamera(IRDataType type = IRDataType.BASIC)
    {
        int iReport;
        iReport = SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL_IR10_EXT6);
        if (iReport < 0) return false;
        return true;
    }

    // current_ext Updated based on the extension connected.
    // Will equal either None or Nunchuck
        private bool RequestIdentifyExtension()
    {
        int iReg = SendRegisterReadRequest(RegisterType.CONTROL, 0xA400FA, 6, RespondIdentifyExtension);
        return iReg > 0;
    }

    //Activates the Nunchuck if connected to the wiimote
    //If no extension is connected - errors caused (Only call if extension is connected)
    private bool ActivateExtension()
    {
        if (!Status.ext_connected)
            Debug.LogWarning("There is a request to activate an Extension controller even though it has not been confirmed to exist!  Trying anyway.");

        // Initialize the Extension by writing 0x55 to register 0xA400F0
        int iReg = SendRegisterWriteRequest(RegisterType.CONTROL, 0xA400F0, new byte[] { 0x55 });
        if (iReg < 0) return false;

        // Activate the Extension by writing 0x00 to register 0xA400FB
        iReg = SendRegisterWriteRequest(RegisterType.CONTROL, 0xA400FB, new byte[] { 0x00 });
        if (iReg < 0) return false;
        return true;
    }

    #endregion

    #region Write
    // Sends a generic block of data to the Wiimote using the specified OutputDataType.
    // Returns the total size of the data written, -1 if HIDApi reports an error, or < -1 if there is an invalid input.
    public int SendWithType(OutputDataType type, byte[] data)
    {
        byte[] final = new byte[data.Length + 1];
        final[0] = (byte)type;

        for (int x = 0; x < data.Length; x++)
            final[x + 1] = data[x];
        //Allows the wiimote to vibrate
            if (bRumbleOn)
            {
                final[0] |= 0x01;
                final[1] |= 0x01;
            }
            else
            {
                final[0] |= 0x00;
                final[1] |= 0x00;
            }
            int res = CS_WiiMoteManager.SendRaw(hidapi_handle, final);

        if (res < -1) Debug.LogError("Incorrect Input to HIDAPI.  No data has been sent.");
        return res;
    }

    public int SendDataReportMode(InputDataType mode)
    {
        if (mode == InputDataType.STATUS_INFO || mode == InputDataType.READ_MEMORY_REGISTERS)
        {
            Debug.LogError("Passed " + mode.ToString() + " to SendDataReportMode!");
            return -2;
        }

        return SendWithType(OutputDataType.DATA_REPORT_MODE, new byte[] { 0x00, (byte)mode });
    }


    // Requests a Wiimote Status update.
    // This will update the data in Status when the Wii Remote reports back.
    public int SendStatusInfoRequest()
    {
        return SendWithType(OutputDataType.STATUS_INFO_REQUEST, new byte[] { 0x00 });
    }

    // Requests the Wiimote to report data from its internal registers.
    // type: The type of register we would like to read from
    // offset: The starting offset of the block of data we would like to read
    // size: The size of the block of data we would like to read
    // Responder: This will be called when the Wiimote finishes reporting the requested data.
    public int SendRegisterReadRequest(RegisterType type, int offset, int size, ReadResponder Responder)
    {
        if (CurrentReadData != null)
        {
            Debug.LogWarning("Aborting read request; There is already a read request pending!");
            return -2;
        }


        CurrentReadData = new CS_RegisterReadData(offset, size, Responder);

        byte address_select = (byte)type;
        byte[] offsetArr = IntToBigEndian(offset, 3);
        byte[] sizeArr = IntToBigEndian(size, 2);

        byte[] total = new byte[] { address_select, offsetArr[0], offsetArr[1], offsetArr[2], 
            sizeArr[0], sizeArr[1] };

        return SendWithType(OutputDataType.READ_MEMORY_REGISTERS, total);
    }

    // Attempts to write a block of data to the Wii Remote's internal registers.
    // type: The type of register we would like to write to
    // offset: The starting offset of the block of data we would like to write to
    // data: Data to write to registers at offset.  Maximum length of 16.
    // On success, > 0, <= 0 on failure.
    // If data.Length > 16 write request will be ignored.
    public int SendRegisterWriteRequest(RegisterType type, int offset, byte[] data)
    {
        if (data.Length > 16) return -2;


        byte address_select = (byte)type;
        byte[] offsetArr = IntToBigEndian(offset, 3);

        byte[] total = new byte[21];
        total[0] = address_select;
        for (int x = 0; x < 3; x++) total[x + 1] = offsetArr[x];
        total[4] = (byte)data.Length;
        for (int x = 0; x < data.Length; x++) total[x + 5] = data[x];

        return SendWithType(OutputDataType.WRITE_MEMORY_REGISTERS, total);
    }
    #endregion

    #region Read
    // Reads and interprets data reported by the Wii Remote.
    // On success, > 0, < 0 on failure, 0 if nothing has been recieved.
    public int ReadWiimoteData()
    {
        byte[] buf = new byte[22];
        int iStatus = CS_WiiMoteManager.RecieveRaw(hidapi_handle, buf);
        if (iStatus <= 0) return iStatus; // Either there is an error or nothings been received

        int iTypeSize = GetInputDataTypeSize((InputDataType)buf[0]);
        byte[] data = new byte[iTypeSize];
        for (int x = 0; x < data.Length; x++)
            data[x] = buf[x + 1];

        if (CS_WiiMoteManager.bDebugMessages)
            Debug.Log("Recieved: [" + buf[0].ToString("X").PadLeft(2, '0') + "] " + BitConverter.ToString(data));
        byte[] ext = null;

        switch ((InputDataType)buf[0]) // buf[0] is the output ID byte
        {
            case InputDataType.STATUS_INFO:
                Debug.Log("Status");
                byte flags = data[2];
                byte battery_level = data[5];

                bool bOld_Ext_Connected = Status.ext_connected;

                byte[] total = new byte[] { flags, battery_level };
                Status.InterpretData(total);

                if (Status.ext_connected != bOld_Ext_Connected)
                {
                    if (Status.ext_connected)                //Only reads the extensions if the Nunchuck is connected
                    {                                        
                        Debug.Log("An extension has been connected.");
                        ActivateExtension();
                        RequestIdentifyExtension();         // Identifies if the extension was a Nunchuck or not
                    }
                    else
                    { 
                        _current_ext = ExtensionController.NONE;
                        Debug.Log("An extension has been disconnected.");
                    }
                }
                break;
            case InputDataType.READ_MEMORY_REGISTERS:

                if (CurrentReadData == null)
                {
                    Debug.LogWarning("Recived Register Read Report when none was expected.  Ignoring.");
                    return iStatus;
                }

                byte size = (byte)((data[2] >> 4) + 0x01);
                byte error = (byte)(data[2] & 0x0f);
                // Error 0x07 means reading from a write-only register
                if (error == 0x07)
                {
                    CurrentReadData = null;
                    return iStatus;
                }
                // uLowOffset is reversed because the Wii Remote reports are in Big Endian order
                ushort uLowOffset = BitConverter.ToUInt16(new byte[] { data[4], data[3] }, 0);
                ushort uExpected = (ushort)CurrentReadData.ExpectedOffset;
                if (uExpected != uLowOffset)
                    Debug.LogWarning("Expected Register Read Offset (" + uExpected + ") does not match reported offset from Wii Remote (" + uLowOffset + ")");
                byte[] read = new byte[size];
                for (int x = 0; x < size; x++)
                    read[x] = data[x + 5];

                CurrentReadData.AppendData(read);
                if (CurrentReadData.ExpectedOffset >= CurrentReadData.Offset + CurrentReadData.Size)
                    CurrentReadData = null;

                break;
            case InputDataType.REPORT_BUTTONS_IR10_EXT9:

                ext = new byte[9];
                for (int x = 0; x < 9; x++)
                    ext[x] = data[x + 12];

                if (_Extension != null)
                    _Extension.InterpretData(ext);
                break;
            case InputDataType.REPORT_BUTTONS_ACCEL_IR10_EXT6: 
                  ext = new byte[6];
                for (int x = 0; x < 6; x++)
                    ext[x] = data[x + 15];

                if (_Extension != null)
                    _Extension.InterpretData(ext);
                break;
        }

        if(ext == null)
            _RawExtension = null;
        else
            _RawExtension = new CS_ReadOnlyArray<byte>(ext);

        return iStatus;
    }

    // The size, in bytes, of a given Wiimote InputDataType when reported by the Wiimote.
    // This is at most 21 bytes.
    public static int GetInputDataTypeSize(InputDataType type)
    {
        switch (type)
        {
            case InputDataType.STATUS_INFO:
                return 6;
            case InputDataType.READ_MEMORY_REGISTERS:
                return 21;
            case InputDataType.REPORT_BUTTONS_IR10_EXT9:
                return 21;
            case InputDataType.REPORT_BUTTONS_ACCEL_IR10_EXT6:
                return 21;

        }
        return 0;
    }


    #endregion

    public static byte[] IntToBigEndian(int input, int len)
    {
        byte[] intBytes = BitConverter.GetBytes(input);
        Array.Resize(ref intBytes, len);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(intBytes);

        return intBytes;
    }
}
}
