using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkAutoPostingBot
{
    public static class Config
    {
        public const string MEM_PATH = "data/mems.txt";
        public const string GROUP_PATH = "data/groups.txt";
        public const string ACC_PATH = "data/accounts.txt";

        public const string TOKEN = ""; 
        public const long CHAT_ID = 0; 
        public const long GROUP_ID = 0L; 
        public const int POST_MINUTES_INTERVAL = 40;
        public const int BLOCK_ACC_WAIT_MINUTES = 60;
    }
}
