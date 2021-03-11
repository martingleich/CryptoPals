using System;
using System.Linq;
using System.Text;

namespace CryptoPals
{
	public static class Program
	{
		static void Main(string[] args)
		{
			Fourth();
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
			foreach (var opt in SingleByteCharDecryption.FindDecryptionKeys(chiper, 10, 0.05))
			{
				var text = opt.Item1.ToPrintableASCII();
				Console.WriteLine(text);
			}
		}
		public static void Fourth()
		{
			var lines = System.IO.File.ReadLines(@"C:\Home\source\CryptoPals\Challenge4.txt", Encoding.ASCII).ToArray();
			var guesses = from entry in lines.AddIDs()
						  let chiper = Bytes.FromHex(entry.Value)
						  from guess in SingleByteCharDecryption.FindDecryptionKeys(chiper, 3, 0.05)
						  orderby guess.Item3
						  select (guess.Item1, entry.Id, guess.Item3);
			foreach (var (lineBytes, id, err) in guesses.Take(10))
			{
				Console.Write($"{id:D3}/{err}: ");
				Console.WriteLine(lineBytes.ToPrintableASCII());
			}
		}
	}
}
