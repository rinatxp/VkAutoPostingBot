using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkAutoPostingBot
{
    public static class Data
    {
        public static VkNet.VkApi Api = new VkNet.VkApi();
        public static List<VkAccount> Accounts = new List<VkAccount>();
        public static List<string> Groups = new List<string>();
        public static BlockingCollection<VkMem> MemsPosted = new BlockingCollection<VkMem>();
        public static BlockingCollection<VkMem> MemsForTg = new BlockingCollection<VkMem>(100);
        public static BlockingCollection<VkMem> MemsInQueue = new BlockingCollection<VkMem>();

        public static void Init()
        {
            Logger.Init();
            VkAccount.Read();
            VkGroup.Read();
            VkMem.Read();
        }
    }
}
