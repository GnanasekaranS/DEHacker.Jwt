using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEHacker.Jwt.Helpers
{
    public interface IKeyVaultManager

    {

        public Task<string> GetSecret(string secretName);

    }
}
