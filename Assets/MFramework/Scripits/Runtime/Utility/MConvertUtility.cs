namespace MFramework
{
    public static class MConvertUtility
    {
        public static byte[] UTF8ToBytes(string str)
        {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static byte[] ASCIIToBytes(string str)
        {
            return System.Text.Encoding.ASCII.GetBytes(str);
        }
    }
}
