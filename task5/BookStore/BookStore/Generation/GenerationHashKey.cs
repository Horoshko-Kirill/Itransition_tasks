

namespace BookStore.Generation
{
    public class GenerationHashKey
    {

        public static int GetHashSeedKey(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hash = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
            return BitConverter.ToInt32(hash, 0);
        }

        public static byte[] GetHashByteKey(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            return sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        }

    }
}
