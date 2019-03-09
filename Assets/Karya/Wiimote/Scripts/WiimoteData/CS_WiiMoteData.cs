using UnityEngine;
using System.Collections;

namespace WiimoteApi
{
    public abstract class CS_WiiMoteData
    {
        protected CS_WiiMote Owner;

        public CS_WiiMoteData(CS_WiiMote a_Owner)
        {
            this.Owner = a_Owner;
        }

        //Interprets raw byte data reported by the Wii Remote.
        public abstract bool InterpretData(byte[] data);
        
    }
}