using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkAutoPostingBot
{
    public class VkAccount
    {
        private readonly TimeSpan timeInterval = new TimeSpan(0, Config.BLOCK_ACC_WAIT_MINUTES, 0);

        public VkAccount(string name, string token)
        {
            Name = name;
            Token = token;
            BlockTime = DateTime.UnixEpoch;
        }

        public string Name { get; private set; }
        public string Token { get; private set; }
        private DateTime BlockTime { get; set; }
        public TimeSpan TimeRemain
        {
            get
            {
                var span = timeInterval - (DateTime.Now - BlockTime);
                return span.Ticks < 0 ? TimeSpan.Zero : span;
            }
        }
        public bool IsBlocked
        {
            get
            {
                if ((DateTime.Now - BlockTime) > timeInterval)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            set
            {
                if (value)
                {
                    BlockTime = DateTime.Now;
                }
            }
        }

        public static void Read()
        {
            int lineNum = 1;
            using (StreamReader reader = new StreamReader(Config.ACC_PATH))
            {
                while (!reader.EndOfStream)
                {
                    try
                    {
                        string line = reader.ReadLine();
                        if (line != null)
                        {
                            string[] parts = line.Split(' ');

                            Data.Accounts.Add(new VkAccount(parts[0], parts[1]));
                        }

                        lineNum++;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("EXCEPTION when read accounts, line " + lineNum + " - " + ex.Message);
                    }
                }
            }
        }
    }
}
