using System.Collections.Generic;

namespace Common.DIFramework
{
    public static class CommandRetainer
    {
        public static Dictionary<long, object> objects = new Dictionary<long, object>();
        private static long index;

        public static long Retain(object obj)
        {
            objects.Add(index, obj);
            long ret = index;
            index++;
            return ret;
        }

        public static void Release(long id)
        {
            objects.Remove(id);
        }
    }
}
