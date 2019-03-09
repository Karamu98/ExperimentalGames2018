using UnityEngine;
using WiimoteApi.Util;

namespace WiimoteApi
{
    public class CS_StatusData : CS_WiiMoteData
    {
        /// True if an extension controller is connected (Nunchuck), as reported by the Wii Remote.
        /// This is only updated when the Wii Remote sends status reports.
        public bool ext_connected { get { return _ext_connected; } }
        private bool _ext_connected;

        //Can be extended to take into account other extensions or battery levels
        public CS_StatusData(CS_WiiMote Owner)
            : base(Owner)
        {
            //Constructor
        }

        public override bool InterpretData(byte[] data)
        {
            if (data == null || data.Length != 2) return false;

            byte flags = data[0];
            _ext_connected = (flags & 0x02) == 0x02;

            return true;
        }
    }
}