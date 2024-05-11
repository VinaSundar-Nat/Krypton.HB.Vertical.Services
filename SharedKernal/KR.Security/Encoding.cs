using System;
using System.Text;

namespace KR.Security
{

    public interface IBase64Convert
    {
        string Encode(string source);
        string Decode(string source);
    }

    public partial class Crypto : IBase64Convert
    {
		public string Encode(string source)
		{
            byte[] bytesToEncode = Encoding.UTF8.GetBytes(source);
            return Convert.ToBase64String(bytesToEncode);
        }

        public string Decode(string source)
        {
            byte[] decodedBytes = Convert.FromBase64String(source);
            return Encoding.UTF8.GetString(decodedBytes);
        }
    }
}

