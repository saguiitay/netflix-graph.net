namespace NFGraph.Net.Util
{
    public static class IntegerUtils
    {
        public static int NumberOfLeadingZeros(int i)
        {
            var count = 0;
            var x = i;

            while (x != 0)
            {
                x = x >> 1;
                count++;
            }

            return 32 - count;
        }
    }
}