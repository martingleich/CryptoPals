using CryptoPals;
using Xunit;

namespace UnitTests
{
	public class UnitTest1
	{
		[Theory]
		[InlineData("", new byte[]{})]
		[InlineData("ab", new byte[] { 0xab })]
		[InlineData("AB", new byte[] { 0xAB })]
		[InlineData("05", new byte[] { 0x05 })]
		[InlineData("05ab", new byte[] { 0x05, 0xab })]
		public void FromHex(string input, byte[] result)
		{
			Assert.Equal(new Bytes(result), Bytes.FromHex(input));
		}

		[Theory]
		[InlineData(new byte[] { }, "")]
		[InlineData(new byte[] {1}, "AQ==")]
		[InlineData(new byte[] {1, 2}, "AQI=")]
		[InlineData(new byte[] {1, 2, 3}, "AQID")]
		[InlineData(new byte[] {123, 76, 234, 54, 23, 255, 254}, "e0zqNhf//g==")]
		[InlineData(new byte[] {0x48, 0x61, 0x6c, 0x6c, 0x6f, 0x20, 0x57, 0x65, 0x6c, 0x74}, "SGFsbG8gV2VsdA==")]
		public void ToBase64(byte[] input, string result)
		{
			var base64 = new Bytes(input).ToBase64();
			Assert.Equal(result, base64);
			var original = Bytes.FromBase64(base64);
			Assert.Equal(new Bytes(input), original);
		}

		[Fact]
		public void RepeatingKeyXor_Encrypt()
		{
			var clearText = Bytes.FromASCII("Burning 'em, if you ain't quick and nimble\nI go crazy when I hear a cymbal");
			var chiperText = RepeatingKeyXorChiper.Encrypt(clearText, Bytes.FromASCII("ICE"));
			Assert.Equal(Bytes.FromHex("0b3637272a2b2e63622c2e69692a23693a2a3c6324202d623d63343c2a26226324272765272a282b2f20430a652e2c652a3124333a653e2b2027630c692b20283165286326302e27282f"), chiperText);
		}

		[Fact]
		public void HammingDistance()
		{
			var a = Bytes.FromASCII("this is a test");
			var b = Bytes.FromASCII("wokka wokka!!!");
			var d = RepeatingKeyXorChiper.HammingDistance(a, b);
			Assert.Equal(37, d);
		}
	}
}
