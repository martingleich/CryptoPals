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
			Assert.Equal(result, new Bytes(input).ToBase64());
		}
	}
}
