using System;
using System.Text;

namespace CryptoPals
{
	public static class Program
	{
		static void Main(string[] args)
		{
			Third();
		}

		public static void First()
		{
			var input = Bytes.FromHex("49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d");
			Console.WriteLine(input.ToBase64());
		}
		public static void Second()
		{
			var first = Bytes.FromHex("1c0111001f010100061a024b53535009181c");
			var second = Bytes.FromHex("686974207468652062756c6c277320657965");
			Console.WriteLine(Operations.Xor(first, second).ToHex());
		}
		public static void Third()
		{
			var chiper = Bytes.FromHex("1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736");
			foreach (var opt in SingleByteCharDecryption.FindDecryptionKeys(chiper))
			{
				var text = Encoding.ASCII.GetString(opt.Item1.Raw);
				Console.WriteLine(text);
			}
		}
	}
}
