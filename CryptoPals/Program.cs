using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace CryptoPals
{
	public static class Program
	{
		static void Main(string[] args)
		{
			typeof(Program).GetMethod("Challenge" + int.Parse(args[0]).ToString("D2"))!.Invoke(null, null);
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
			var decrypt = MyAes.MakeDecryptBlock(Bytes.FromASCII("YELLOW SUBMARINE"));
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
		public static void Challenge11()
		{
			Random rnd = new Random(345234);
			(bool, byte[]) RandomEncryption(byte[] input)
			{
				var prefix = rnd.NextNBytes(rnd.Next(5, 10));
				var postfix = rnd.NextNBytes(rnd.Next(5, 10));
				var total = prefix.Concat(input).Concat(postfix).ToArray().Pad_PKCS_7_Multiple(16);
				var key = rnd.NextNBytes(16);
				var iv = rnd.NextNBytes(16);
				var usedMode = rnd.NextBool();
				var encrypt = usedMode ? MyAes.MakeEncryptEBC(key) : MyAes.MakeDecryptCBC(key, iv);
				return (usedMode, encrypt(total));
			}

			for (int i = 0; i < 100; ++i)
			{
				bool usedMode = false;
				bool detectedMode = MyTools.IsEBC(clear =>
				{
					var (mode, chiper) = RandomEncryption(clear);
					usedMode = mode;
					return chiper;
				});
				Console.WriteLine($"{usedMode} => {detectedMode}");
				Console.WriteLine("======");
			}
		}
		public static void Challenge12()
		{
			byte[] unknownString = Bytes.FromBase64("Um9sbGluJyBpbiBteSA1LjAKV2l0aCBteSByYWctdG9wIGRvd24gc28gbXkgaGFpciBjYW4gYmxvdwpUaGUgZ2lybGllcyBvbiBzdGFuZGJ5IHdhdmluZyBqdXN0IHRvIHNheSBoaQpEaWQgeW91IHN0b3A/IE5vLCBJIGp1c3QgZHJvdmUgYnkK");
			Func<byte[], byte[]> baseEncrypt = MyAes.MakeEncryptEBC(new Random().NextNBytes(16));
			byte[] encrypt(byte[] clear) => baseEncrypt(Bytes.Concat(clear, unknownString));

			var blockSize = MyTools.DetectBlockSize(encrypt);
			bool isEBC = MyTools.IsEBC(encrypt, blockSize);
			if (!isEBC)
				throw new InvalidOperationException();
			var secret = new List<byte>();
			var block = new byte[blockSize];
			int curBlockStart = 0;
			int curBlockLast = curBlockStart + blockSize - 1;
			while (true)
			{
				Array.Copy(block, 1, block, 0, block.Length - 1); // Shift block one to the left
				var value = encrypt(new byte[blockSize - 1 - secret.Count % blockSize]).Subrange(curBlockStart, blockSize);
				bool found = false;
				foreach (byte b in Facts.ENGLISH_FREQ_MAP.BytesByDecreasingFrequencies)
				{
					block[^1] = b;
					var chiper = encrypt(block).Subrange(curBlockStart, blockSize);
					if (ArrayEqualComparer<byte>.Instance.Equals(chiper, value))
					{
						secret.Add(b);
						if (secret.Count % blockSize == 0)
						{
							var newBlock = new byte[curBlockLast + 1 + blockSize];
							Array.Copy(block, curBlockStart, newBlock, curBlockStart + blockSize, blockSize);
							block = newBlock;

							curBlockStart += blockSize;
							curBlockLast += blockSize;
						}
						found = true;
						break;
					}
				}
				if (!found)
					break; // Can we find a better way to stop. This seems kind of strange.
			}
			Console.WriteLine(secret.ToArray().ToASCII());
		}
		public static void Challenge13()
		{
			static string ProfileFor(string email) => new Profile(email, "10", "user").ToString();
			var key = new Random().NextNBytes(16);
			Func<byte[], byte[]> encrypt = MyAes.MakeEncryptEBC(key);
			Func<byte[], byte[]> decrypt = MyAes.MakeDecryptEBC(key);
			byte[] ProfileForChiper(string email) => encrypt(Encoding.ASCII.GetBytes(ProfileFor(email)));
			void ShowProfile(byte[] encrypted) => Console.WriteLine(Profile.FromString(Encoding.ASCII.GetString(decrypt(encrypted))));

			var blockSize = 16;
			var prefix = "email=";
			var role = "admin";
			var restAdminBlock = blockSize - role.Length;
			var x = ProfileForChiper(new string('x', blockSize - prefix.Length) + role + new string((char)restAdminBlock, restAdminBlock));
			var adminBlock = x.BlockN(1, blockSize);
			var y = ProfileForChiper("foobar@web.de");
			Array.Copy(adminBlock, 0, y, 2 * blockSize, blockSize);
			ShowProfile(y);
		}
	}

	public sealed class Profile
	{
		public readonly string Email;
		public readonly string Uid;
		public readonly string Role;

		public Profile(string email, string uid, string role)
		{
			Email = email.Replace("&", "").Replace("=", "");
			Uid = Uri.EscapeDataString(uid);
			Role = Uri.EscapeDataString(role);
		}

		public static Profile FromString(string str)
		{
			var queryString = HttpUtility.ParseQueryString(str);
			return new Profile(queryString["email"], queryString["uid"], queryString["role"]);
		}

		public override string ToString() => $"email={Email}&uid={Uid}&Role={Role}";
	}
}
