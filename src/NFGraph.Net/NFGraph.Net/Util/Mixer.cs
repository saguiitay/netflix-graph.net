namespace NFGraph.Net.Util
{
    public static class Mixer
    {
        public static int HashInt(int key)
        {
            key = ~key + (key << 15);
            key = key ^ (key >> 12);
            key = key + (key << 2);
            key = key ^ (key >> 4);
            key = key*2057;
            key = key ^ (key >> 16);
            return key & int.MaxValue;
        }
    }
}