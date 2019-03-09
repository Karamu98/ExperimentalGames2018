using WiimoteApi.Util;

namespace WiimoteApi {
    public class CS_NunchuckData : CS_WiiMoteData
    {
        // Nunchuck Analog Stick values.  This is a size 2 Array [X, Y]. 
        // Generally analog stick returns values in the range 35-228 for X and 27-220 for Y.  Centre being approx 128 (varies)
        //Check centre for the Nunchuck used and adjust accordingly in the main for the Camera Movement.
        public CS_ReadOnlyArray<byte> stick { get { return _stick_readonly; } }
        private CS_ReadOnlyArray<byte> _stick_readonly;
        private byte[] _stick;
        //Allow the buttons to be read
        // Button: C
        public bool c { get { return _c; } }
        private bool _c;
        // Button: Z
        public bool z { get { return _z; } }
        private bool _z;

        public CS_NunchuckData(CS_WiiMote Owner)
            : base(Owner)
        {
            _stick = new byte[2];
            _stick_readonly = new CS_ReadOnlyArray<byte>(_stick);
        }

        public override bool InterpretData(byte[] data) {
            if(data == null || data.Length < 6) {
                _stick[0] = 128; _stick[1] = 128;
                _c = false;
                _z = false;
                return false;
            }

            _stick[0] = data[0];
            _stick[1] = data[1];

            _c = (data[5] & 0x02) != 0x02;
            _z = (data[5] & 0x01) != 0x01;
            return true;
        }

        //Returns vector2 [X, Y] array of the analog stick's position, in the range 0 - 1.  This takes into account typical Nunchuck data ranges and zero points.
        public float[] GetStick01() {
            float[] fRet = new float[2];
            fRet[0] = _stick[0];
            fRet[0] -= 35;
            fRet[1] = stick[1];
            fRet[1] -= 27;
            for(int x=0;x<2;x++) {
                fRet[x] /= 193f;
            }
            return fRet;
        }
    }
}