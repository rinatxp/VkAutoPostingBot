using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkAutoPostingBot
{
    public static class VkGroup
    {
        public static void Read()
        {
            using (var reader = new StreamReader(Config.GROUP_PATH))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line) && line[0] != '\\')
                    {
                        Data.Groups.Add(line.Split('/').Last());
                    }
                }
            }
        }
    }
}
