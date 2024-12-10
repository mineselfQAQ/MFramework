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

        public static string BytesToUTF8(byte[] buff)
        {
            return System.Text.Encoding.UTF8.GetString(buff);
        }

        public static string BytesToASCII(byte[] buff)
        {
            return System.Text.Encoding.ASCII.GetString(buff);
        }
    }
}
