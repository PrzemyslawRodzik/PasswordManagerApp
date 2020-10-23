using System;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;


namespace PasswordManagerApp.Handlers
{
    /// <summary>
    /// Check if passwords have been pwned using https://haveibeenpwned.com/API/v2#PwnedPasswords
    /// </summary>
    public  class PwnedPasswords
    {
        
        private static Lazy<HttpClient> _fallbackClient = new Lazy<HttpClient>(() =>
        {
            return new HttpClient();
        });

        /// <summary>
        /// Check if the password is pwned.
        /// </summary>
        /// <param name="password">The password you want to check</param>
        /// <param name="cancellationToken">Cancellationtoken to abort the http request</param>
        /// <param name="httpClient">In web-based projects it's recommended that you re-ues existing httpclients</param>
        /// <returns></returns>
        public async static Task<int> IsPasswordPwnedAsync(string password, CancellationToken cancellationToken, HttpClient httpClient = null)
        {
            if (password == null) return -1;
            byte[] byteString = Encoding.UTF8.GetBytes(password);
            byte[] hashBytes = null;
            var hashString = string.Empty;
            using (var sha1 = SHA1.Create())
            {
                hashBytes = sha1.ComputeHash(byteString);
            }
            var sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }
            hashString = sb.ToString();

            string hashFirstFive = hashString.Substring(0, 5);
            string hashLeftover = hashString.Substring(5, hashString.Length - 5);

            HttpClient client = httpClient;
            if (httpClient == null)
            {
                client = _fallbackClient.Value;
            }

            try
            {
                var response = await client.GetStringAsync($"https://api.pwnedpasswords.com/range/{hashFirstFive}");
                return ReduceResult(hashString, response);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        private static int ReduceResult(string hashToFind, string listOfHashes)
        {
            var hashes = listOfHashes.Split(Environment.NewLine);
            var first5 = hashToFind.Substring(0, 5);
            var found = hashes.FirstOrDefault(h => $"{first5}{h}".Contains(hashToFind));
            if (found == null)
            {
                return -1;
            }
            return Convert.ToInt32(found.Split(':')[1]);
        }
    }
}
