using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoPals
{
	public static class Program
	{
		static void Main(string[] args)
		{
			Sixth();
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
			Console.WriteLine(Bytes.Xor(first, second).ToHex());
		}
		public static void Third()
		{
			var chiper = Bytes.FromHex("1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736");
			foreach (var opt in SingleByteXorChiper.FindDecryptionKeys(chiper, 10, 0.05))
			{
				var text = SingleByteXorChiper.Decrypt(chiper, opt.Item1).ToPrintableASCII();
				Console.WriteLine(text);
			}
		}
		public static void Fourth()
		{
			var lines = System.IO.File.ReadLines("Challenge4.txt", Encoding.ASCII).ToArray();
			var guesses = from entry in lines.AddIDs()
						  let chiper = Bytes.FromHex(entry.Value)
						  from guess in SingleByteXorChiper.FindDecryptionKeys(chiper, 3, 0.05)
						  orderby guess.Item2
						  select (SingleByteXorChiper.Decrypt(chiper, guess.Item1).ToPrintableASCII(), entry.Id);
			foreach (var (clearText, id) in guesses.Take(10))
			{
				Console.Write($"{id:D3}: ");
				Console.WriteLine(clearText);
			}
		}
		public static void Fifth()
		{
			var clearText = Bytes.FromASCII("Burning 'em, if you ain't quick and nimble\nI go crazy when I hear a cymbal");
			var chiperText = RepeatingKeyXorChiper.Encrypt(clearText, Bytes.FromASCII("ICE"));
			Console.Write(chiperText.ToHex());
		}

		public static void Sixth()
		{
			var chiperText = Bytes.FromBase64(string.Join("", System.IO.File.ReadAllLines("Challenge6.txt", Encoding.ASCII)));
			foreach (var option in RepeatingKeyXorChiper.FindDecryption(chiperText).Take(1))
			{
				Console.WriteLine(option.ToASCII());
			}
		}

		public static void Seventh()
		{
			var aes = Aes.Create();
			aes.BlockSize = 128;
			aes.Mode = CipherMode.ECB;
		}
	}
}
