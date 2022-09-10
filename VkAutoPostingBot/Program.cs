using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkAutoPostingBot
{
    internal class Program
    {
        static void Main()
        {
            Data.Init();
            AccountManager.TryAuthorizeNext();

            new Thread(Tg.Start).Start();
            new Thread(QueueMy.PostMems).Start();

            var grInfo = Data.Api.Groups.GetById(Data.Groups, null, null);
            ulong OFFSET = 100;

            while (true)
            {
                var mems = GetNewMems.Get(grInfo, OFFSET);

                foreach (var m in mems)
                {
                    Data.MemsForTg.Add(m);
                }

                OFFSET += 10;
            }
        }
    }
}
