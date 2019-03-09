using WiimoteApi;

namespace WiimoteApi.Internal {
    public class CS_RegisterReadData
    {
        public CS_RegisterReadData(int a_iOffset, int a_iSize, ReadResponder Responder)
        {
            _Offset = a_iOffset;
            _Size = a_iSize;
            _Buffer = new byte[a_iSize];
            _ExpectedOffset = a_iOffset;
            _Responder = Responder;
        }

        public int ExpectedOffset
        {
            get { return _ExpectedOffset; }
        }
        private int _ExpectedOffset;

        public byte[] Buffer
        {
            get { return _Buffer; }
        }
        private byte[] _Buffer;

        public int Offset
        {
            get { return _Offset; }
        }
        private int _Offset;

        public int Size
        {
            get { return _Size; }
        }
        private int _Size;

        private ReadResponder _Responder;

        public bool AppendData(byte[] data)
        {
            int iStart = _ExpectedOffset - _Offset;
            int iEnd = iStart + data.Length;

            if (iEnd > _Buffer.Length)
                return false;

            for (int x = iStart; x < iEnd; x++)
            {
                _Buffer[x] = data[x - iStart];
            }

            _ExpectedOffset += data.Length;

            if (_ExpectedOffset >= _Offset + _Size)
                _Responder(_Buffer);

            return true;
        }
    }
}
