using System.Collections.Generic;

namespace IndexerLib
{
    public class ConcurrentList<T>
    {
        private object syncObj = new object();
        private List<T> data;

        public ConcurrentList()
        {
            data = new List<T>();
        }

        public void Add(T val)
        {
            lock(syncObj)
            {
                data.Add(val);
            }
        }
    }
}
