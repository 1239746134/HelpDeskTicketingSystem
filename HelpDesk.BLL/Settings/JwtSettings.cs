using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.BLL.Settings
{
    public class JwtSettings
    {
        public string Key { get; set; }          // 对称密钥（≥32 字节）
        public string Issuer { get; set; }        // 签发者
        public string Audience { get; set; }      // 受众
        public int ExpiryMinutes { get; set; }    // 过期分钟数
    }
}
