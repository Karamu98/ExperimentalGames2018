namespace WiimoteApi.Util
{
    //This is used for basic data encapsulation
    public class CS_ReadOnlyArray<T>
    {
        private T[] _data;

        public int Length
        {
            get { return _data.Length; }
        }

        public CS_ReadOnlyArray(T[] data)
        {
            _data = data;
        }

        public T this[int x]
        {
            get
            {
                return _data[x];
            }
        }
    }
}