using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Model;

namespace VkAutoPostingBot
{
    public static class AccountManager
    {
        public static void TryAuthorizeNext()
        {
        nextAcc: var acc = GetNormalAccount();

            try
            {
                Data.Api.Authorize(new ApiAuthParams
                {
                    AccessToken = acc.Token,
                    Settings = VkNet.Enums.Filters.Settings.Wall
                });
            }
            catch
            {
                Logger.Log($"Account {acc.Name} blocked, change to another one");
                acc.IsBlocked = true;
                goto nextAcc;
            }
        }

        private static VkAccount GetNormalAccount()
        {
            Thread.Sleep(Data.Accounts.Min(a => a.TimeRemain));
            return Data.Accounts.Find(f => !f.IsBlocked);
        }
    }
}
