using System;
namespace KR.Security
{
    public static class CryptoExtensions
    {
        public static byte[] ToByteArray(this String source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            var value = source.ToArray();

            int length = value.Length;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                if (!Int32.TryParse(value[i].ToString(), out int _cast))
                    continue;

                bytes[i] = Convert.ToByte(_cast.ToString(), 16);
            }

            return bytes;
        }

        public static string ToString(this byte[] source, bool readable)
        {
            return readable ? BitConverter.ToString(source).Replace("-", "")
                : Convert.ToBase64String(source);
        }
    }   
}

