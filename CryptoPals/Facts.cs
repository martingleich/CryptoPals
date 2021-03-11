namespace CryptoPals
{
	public static class Facts
	{
		public static readonly ByteFreqMap ENGLISH_FREQ_MAP = ByteFreqMap.FromValues(System.IO.File.ReadAllBytes("raw_english.txt"));
	}
}
