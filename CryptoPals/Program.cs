using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace CryptoPals
{
	public static class MyAes
	{
		public static Func<byte[], byte[]> MakeEncrypt(byte[] key)
		{
			var aes = Aes.Create();
			aes.Mode = CipherMode.ECB;
			aes.Key = key;
			aes.Padding = PaddingMode.None;
			var transform = aes.CreateEncryptor();
			return clear => transform.TransformFinalBlock(clear, 0, clear.Length);
		}
		public static Func<byte[], byte[]> MakeDecrypt(byte[] key)
		{
			var aes = Aes.Create();
			aes.Mode = CipherMode.ECB;
			aes.Key = key;
			aes.Padding = PaddingMode.None;
			var transform = aes.CreateDecryptor();
			return clear => transform.TransformFinalBlock(clear, 0, clear.Length);
		}

		public static Func<byte[], byte[]> MakeEncryptCBC(byte[] key, byte[] iv)
		{
			var encrypt = MakeEncrypt(key);

			return clearText =>
			{
				var padded = clearText.Pad_PKCS_7_Multiple(16);
				var lastChiper = iv;
				List<byte[]> blocks = new List<byte[]>();
				foreach (var block in padded.Split(16))
				{
					lastChiper = encrypt(block.Xor(lastChiper));
					blocks.Add(lastChiper);
				}
				return Bytes.FromRange(blocks.SelectMany(b => b));
			};
		}
		public static Func<byte[], byte[]> MakeDecryptCBC(byte[] key, byte[] iv)
		{
			var decrypt = MakeDecrypt(key);

			return chiperText =>
			{
				var lastChiper = iv;
				List<byte[]> blocks = new List<byte[]>();
				foreach (var block in chiperText.Split(16))
				{
					var clear = decrypt(block).Xor(lastChiper);
					lastChiper = block;
					blocks.Add(clear);
				}
				return Bytes.FromRange(blocks.SelectMany(b => b));
			};
		}
	}

	public static class Program
	{
		static void Main(string[] args)
		{
			Challenge10();
		}

		public static void Challenge01()
		{
			var input = Bytes.FromHex("49276d206b696c6c696e6720796f757220627261696e206c696b65206120706f69736f6e6f7573206d757368726f6f6d");
			Console.WriteLine(input.ToBase64());
		}
		public static void Challenge02()
		{
			var first = Bytes.FromHex("1c0111001f010100061a024b53535009181c");
			var second = Bytes.FromHex("686974207468652062756c6c277320657965");
			Console.WriteLine(Bytes.Xor(first, second).ToHex());
		}
		public static void Challenge03()
		{
			var chiper = Bytes.FromHex("1b37373331363f78151b7f2b783431333d78397828372d363c78373e783a393b3736");
			foreach (var opt in SingleByteXorChiper.FindDecryptionKeys(chiper, 10, 0.05))
			{
				var text = SingleByteXorChiper.Decrypt(chiper, opt.Item1).ToPrintableASCII();
				Console.WriteLine(text);
			}
		}
		public static void Challenge04()
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
		public static void Challenge05()
		{
			var clearText = Bytes.FromASCII("Burning 'em, if you ain't quick and nimble\nI go crazy when I hear a cymbal");
			var chiperText = RepeatingKeyXorChiper.Encrypt(clearText, Bytes.FromASCII("ICE"));
			Console.Write(chiperText.ToHex());
		}
		public static void Challenge06()
		{
			var chiperText = Bytes.FromBase64(System.IO.File.ReadAllLines("Challenge6.txt", Encoding.ASCII));
			foreach (var option in RepeatingKeyXorChiper.FindDecryption(chiperText).Take(1))
			{
				Console.WriteLine(option.ToASCII());
			}
		}

		public static void Challenge07()
		{
			var chiperText = Bytes.FromBase64(System.IO.File.ReadAllLines("Challenge7.base64", Encoding.ASCII));
			var decrypt = MyAes.MakeDecrypt(Bytes.FromASCII("YELLOW SUBMARINE"));
			for (int i = 0; i < chiperText.Length; i += 16)
				Console.Write(decrypt(chiperText.Subrange(i, 16)).ToASCII());
		}
		public static void Challenge08()
		{
			var lines = System.IO.File.ReadAllLines("Challenge8.hex.lines", Encoding.ASCII).Select(Bytes.FromHex).ToArray();
			int id = 0;
			foreach (var line in lines)
			{
				++id;
				var blocks = line.Split(16);
				if (blocks.ToHashSet(ArrayEqualComparer<byte>.Instance).Count != blocks.Length)
					Console.WriteLine($"The line {id} is most likely AES-ECB encrypted.");
			}
		}
		public static void Challenge09()
		{
			var value = Bytes.FromASCII("YELLOW SUBMARINE").Pad_PKCS_7(20);
			Console.Write(value.ToPrintableASCII());
		}
		public static void Challenge10()
		{
			var chiperText = Bytes.FromBase64(System.IO.File.ReadAllLines("Challenge10.base64"));
			var decrypt = MyAes.MakeDecryptCBC(Bytes.FromASCII("YELLOW SUBMARINE"), new byte[16]);
			Console.WriteLine(decrypt(chiperText).ToASCII());
		}
	}
}
