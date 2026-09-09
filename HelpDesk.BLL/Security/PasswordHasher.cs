using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.Security
{
    public class PasswordHasher : IPasswordHasher
    {
        private const int SaltSize = 16;         // 盐长度：128 bit
        private const int KeySize = 32;          // 派生密钥长度：256 bit
        private const int Iterations = 100000;   // 迭代次数

        public string Hash(string password)
        {
            //生成随机盐
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            //PBKDF2 派生哈希
            byte[] key = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
            //存储格式：盐.哈希（都 Base64 编码）
            return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }

        public bool Verify(string password, string hashed)
        {
            string[] parts = hashed.Split('.');
            if (parts.Length != 2) return false;

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] expectedKey = Convert.FromBase64String(parts[1]);

            //用同一盐重算，比较
            byte[] actualKey = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, Iterations, HashAlgorithmName.SHA256, expectedKey.Length);

            //恒定时间比较，防时序攻击
            return CryptographicOperations.FixedTimeEquals(actualKey, expectedKey);
        }
    }
}
