using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Runtime.InteropServices;
using WiimoteApi.Internal;

namespace WiimoteApi { 

public class CS_WiiMoteManager
{
    private const ushort c_uVendorIDWiiMote = 0x057e;
    private const ushort c_uProductIDWiiMote = 0x0306;

    // A list of all currently connected Wii Remotes.
    public static List<CS_WiiMote> Wiimotes { get { return _Wiimotes; } }
    private static List<CS_WiiMote> _Wiimotes = new List<CS_WiiMote>();

    // If true, WiimoteManager and Wiimote will write data reports and other debug
    // messages to the console.  Any incorrect usages / errors will still be reported.
    public static bool bDebugMessages = false;

    // The maximum time, in milliseconds, between data report writes.
    // Prevents WiimoteApi from attempting to write faster than most bluetooth drivers can handle.
    public static int s_iMaxWriteFrequency = 20; // In ms
    private static Queue<WriteQueueData> WriteQueue;

    // ------------------ RAW HIDAPI INTERFACE ------------------

    // Attempts to find connected Wii Remotes
    public static bool FindWiimotes()
    {
        bool bTemp = _FindWiimotes(WiimoteType.WIIMOTE);
        return bTemp;
    }

    private static bool _FindWiimotes(WiimoteType a_MoteType)
    {
        ushort uVendor = 0;
        ushort uProduct = 0;

        if(a_MoteType == WiimoteType.WIIMOTE) {
            uVendor = c_uVendorIDWiiMote;
            uProduct = c_uProductIDWiiMote;
        }

        IntPtr ptr = CS_HIDapi.hid_enumerate(uVendor, uProduct);
        IntPtr cur_ptr = ptr;

        if (ptr == IntPtr.Zero)
            return false;

        hid_device_info enumerate = (hid_device_info)Marshal.PtrToStructure(ptr, typeof(hid_device_info));

        bool bHasFound = false;

        while(cur_ptr != IntPtr.Zero)
        {
            CS_WiiMote Remote = null;
            bool bEnd = false;
            foreach (CS_WiiMote r in Wiimotes)
            {
                if (bEnd)
                    continue;

                if (r.hidapi_path.Equals(enumerate.path))
                {
                    Remote = r;
                    bEnd = true;
                }
            }
            if (Remote == null)
            {
                IntPtr handle = CS_HIDapi.hid_open_path(enumerate.path);

                Remote = new CS_WiiMote(handle, enumerate.path, a_MoteType);

                if (bDebugMessages)
                    Debug.Log("Found New Remote: " + Remote.hidapi_path);

                Wiimotes.Add(Remote);
                Remote.SendStatusInfoRequest();
            }

            cur_ptr = enumerate.next;
            if(cur_ptr != IntPtr.Zero)
                enumerate = (hid_device_info)Marshal.PtrToStructure(cur_ptr, typeof(hid_device_info));
        }

        CS_HIDapi.hid_free_enumeration(ptr);

        return bHasFound;
    }

    // Disables the given Wiimote by closing its bluetooth HID connection.  Also removes the remote from Wiimotes
    // Not Currently Implemented
    public static void Clean(CS_WiiMote remote)
    {
        if (remote != null)
		{
			if (remote.hidapi_handle != IntPtr.Zero)
				CS_HIDapi.hid_close (remote.hidapi_handle);

			Wiimotes.Remove (remote);
		}
    }

    // If any Wii Remotes are connected and found by FindWiimote
    public static bool HasWiimote()
    {
        return !(Wiimotes.Count <= 0 || Wiimotes[0] == null || Wiimotes[0].hidapi_handle == IntPtr.Zero);
    }

    // Sends RAW DATA to the given bluetooth HID device (WiiMote).
    public static int SendRaw(IntPtr hidapi_wiimote, byte[] data)
    {
        if (hidapi_wiimote == IntPtr.Zero) return -2;

        if (WriteQueue == null)
        {
            WriteQueue = new Queue<WriteQueueData>();
            SendThreadObj = new Thread(new ThreadStart(SendThread));
            SendThreadObj.Start();
        }

        WriteQueueData wqd = new WriteQueueData();
        wqd.pointer = hidapi_wiimote;
        wqd.data = data;
        lock(WriteQueue)
            WriteQueue.Enqueue(wqd);

        return 0;
    }

    private static Thread SendThreadObj;
    private static void SendThread()
    {
        while (true)
        {
            lock (WriteQueue)
            {
                if (WriteQueue.Count != 0)
                {
                    WriteQueueData wqd = WriteQueue.Dequeue();
                    int res = CS_HIDapi.hid_write(wqd.pointer, wqd.data, new UIntPtr(Convert.ToUInt32(wqd.data.Length)));
                    if (res == -1) Debug.LogError("HidAPI reports error " + res + " on write: " + Marshal.PtrToStringUni(CS_HIDapi.hid_error(wqd.pointer)));
                    else if (bDebugMessages) Debug.Log("Sent " + res + "b: [" + wqd.data[0].ToString("X").PadLeft(2, '0') + "] " + BitConverter.ToString(wqd.data, 1));
                }
            }
            Thread.Sleep(s_iMaxWriteFrequency);
        }
    }

    // Recieves RAW DATA to the given bluetooth HID device (wiimote)
    public static int RecieveRaw(IntPtr hidapi_wiimote, byte[] buf)
    {
        if (hidapi_wiimote == IntPtr.Zero) return -2;

        CS_HIDapi.hid_set_nonblocking(hidapi_wiimote, 1);
        int iRes = CS_HIDapi.hid_read(hidapi_wiimote, buf, new UIntPtr(Convert.ToUInt32(buf.Length)));

        return iRes;
    }

    private class WriteQueueData {
        public IntPtr pointer;
        public byte[] data;
    }
}
} // namespace WiimoteApi