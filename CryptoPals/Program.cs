using System;

namespace CryptoPals
{
	public static class Program
	{
		static void Main(string[] args)
		{
			First();
		}

		public static void First()
		{
			var input = Bytes.FromHex("49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d");
			Console.WriteLine(input.ToBase64());
		}
	}
}
