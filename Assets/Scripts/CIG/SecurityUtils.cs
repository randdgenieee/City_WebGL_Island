using System.Security.Cryptography;
using System.Text;

namespace CIG
{
	public static class SecurityUtils
	{
		public static string ToHashedString(this string text)
		{
			byte[] bytes = Encoding.ASCII.GetBytes(text);
			byte[] array = new SHA1CryptoServiceProvider().ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("X2"));
			}
			return stringBuilder.ToString();
		}
	}
}
