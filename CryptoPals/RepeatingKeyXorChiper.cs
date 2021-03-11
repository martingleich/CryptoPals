using System.Linq;

namespace CryptoPals
{
	public static class RepeatingKeyXorChiper
	{
		public static Bytes Encrypt(Bytes clearText, Bytes key) => new Bytes(clearText.Select((c, id) => c.Xor(key[id % key.Count])));
	}
}
