using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkAutoPostingBot
{
    public static class QueueMy
    {
        public static void PostMems()
        {
            while (true)
            {
                var mem = Data.MemsInQueue.Take();

                try
                {
                    PostToWall.Post(mem);
                    Thread.Sleep(new TimeSpan(0, Config.POST_MINUTES_INTERVAL, 0));
                }
                catch { }
                finally
                {
                    VkMem.Write(mem);
                }
            }
        }
    }
}
