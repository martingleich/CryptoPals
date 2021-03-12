using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace CryptoPals
{
	public static class MyAes
	{
		public static Func<byte[], byte[]> MakeEncryptBlock(byte[] key)
		{
			var aes = Aes.Create();
			aes.Mode = CipherMode.ECB;
			aes.Key = key;
			aes.Padding = PaddingMode.None;
			var transform = aes.CreateEncryptor();
			return clear => transform.TransformFinalBlock(clear, 0, clear.Length);
		}
		public static Func<byte[], byte[]> MakeDecryptBlock(byte[] key)
		{
			var aes = Aes.Create();
			aes.Mode = CipherMode.ECB;
			aes.Key = key;
			aes.Padding = PaddingMode.None;
			var transform = aes.CreateDecryptor();
			return clear => transform.TransformFinalBlock(clear, 0, clear.Length);
		}

		public static Func<byte[], byte[]> MakeEncryptEBC(byte[] key)
		{
			var encrypt = MakeEncryptBlock(key);

			return clearText =>
			{
				var padded = clearText.Pad_PKCS_7_Multiple(16);
				List<byte[]> blocks = new List<byte[]>();
				foreach (var block in padded.Split(16))
				{
					var chiper = encrypt(block);
					blocks.Add(chiper);
				}
				return blocks.Concat();
			};
		}
		public static Func<byte[], byte[]> MakeEncryptCBC(byte[] key, byte[] iv)
		{
			var encrypt = MakeEncryptBlock(key);

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
				return blocks.Concat();
			};
		}
		public static Func<byte[], byte[]> MakeDecryptCBC(byte[] key, byte[] iv)
		{
			var decrypt = MakeDecryptBlock(key);

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
				return blocks.Concat();
			};
		}
	}
}
