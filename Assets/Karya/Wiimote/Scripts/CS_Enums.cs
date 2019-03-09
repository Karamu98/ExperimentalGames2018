namespace WiimoteApi {
    // A data storage register that can be read from / written to: in SendRegisterWriteRequest and SendRegisterReadRequest
    public enum RegisterType
{
    // The Wii Remote's 16kB generic EEPROM memory module.  This is used to store calibration data
    EEPROM = 0x00,
    // The Wii Remote's control registers, used for managing the Wii Remote's peripherals (such as extension
    // controllers, the speakers, and the IR camera). This will be used for managing the Nunchuck.
    CONTROL = 0x04
}

// Represents all data that can be sent from the host to the Wii Remote.
// This information is used by the remote to change its internal read/write remote.
public enum OutputDataType
{
    DATA_REPORT_MODE = 0x12,
    STATUS_INFO_REQUEST = 0x15,
    WRITE_MEMORY_REGISTERS = 0x16,
    READ_MEMORY_REGISTERS = 0x17,
}

// Represents all data that can be sent from the Wii Remote to the host.
// This information is used by the host as basic controller data from the Wii Remote.
// All REPORT_ types represent the actual data types that can be sent from the contoller.
public enum InputDataType
{
    STATUS_INFO = 0x20,
    READ_MEMORY_REGISTERS = 0x21,
    REPORT_BUTTONS_IR10_EXT9 = 0x36,
    /// Data Report Mode: Buttons, Accelerometer, 10 IR Bytes (IRDataType::BASIC), 6 Extension Bytes
    REPORT_BUTTONS_ACCEL_IR10_EXT6 = 0x37,
    /// Data Report Mode: 21 Extension Bytes
}

// These are the 3 types of IR data accepted by the Wii Remote.  Offering more
// or less IR data in exchange for space for other data (such as extension
// controllers or accelerometer data).
// We need space for extension data and therefore require the basic IR type - 
// Maximum extension data and minimum IR Data.
// As we wont be using IR we do not need to worry about the space it has.
public enum IRDataType
{
    //10 bytes
    BASIC = 1,
}

public enum ExtensionController
{
    /// No Extension Controller is connected.
    NONE, 
    /// A Nunchuck Controller is connected.
    NUNCHUCK, 
}


public enum WiimoteType {
    // The original Wii Remote (Bluetooth Name: RVL-CNT-01).  This includes all Wii Remotes manufactured for the original Wii.
    // Used to find wii motes
    WIIMOTE, 
}

} // namespace WiimoteApi