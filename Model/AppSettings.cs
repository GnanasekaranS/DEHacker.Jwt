using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEHacker.Jwt.Model
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public int TokenExpiryInMinutes { get; set; }
    }
}
