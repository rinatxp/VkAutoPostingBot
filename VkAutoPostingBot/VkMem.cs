using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model.Attachments;
using VkNet.Model;

namespace VkAutoPostingBot
{
    public class VkMem
    {
        private static object lockObj = new object();

        public VkMem(long groupID, long postID)
        {
            GroupID = groupID;
            PostID = postID;
        }
        public VkMem(long groupID, long postID, DateTime date, string message, Group group,
                    IEnumerable<MediaAttachment> attachment,
                    IEnumerable<string> photoURLs)
        {
            GroupID = groupID;
            PostID = postID;
            Date = date;
            Message = message;
            Group = group;
            Attachments = attachment;
            PhotoURLs = photoURLs;
        }

        public long GroupID { get; }
        public long PostID { get; }
        public DateTime Date { get; }
        public string Message { get; }
        public Group Group { get; set; }
        public IEnumerable<MediaAttachment> Attachments { get; set; }
        public IEnumerable<string> PhotoURLs { get; set; }


        public static void Read()
        {
            int lineNum = 1;

            using (var reader = new StreamReader(Config.MEM_PATH))
            {
                while (!reader.EndOfStream)
                {
                    try
                    {
                        string line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] parts = line.Split(' ');
                            VkMem mem = new VkMem(long.Parse(parts[0]), long.Parse(parts[1]));

                            Data.MemsPosted.Add(mem);
                        }

                        lineNum++;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("EXCEPTION when read mems, line " + lineNum + " - " + ex.Message);
                    }
                }
            }
        }

        public static void Write(VkMem mem)
        {
            lock (lockObj)
            {
                StreamWriter writer = new StreamWriter(Config.MEM_PATH, true);

                try
                {
                    writer.WriteLine($"{mem.GroupID} {mem.PostID}");
                }
                catch (Exception ex)
                {
                    Logger.Log("EXCEPTION when write mem - " + ex.Message);
                }
                finally
                {
                    writer.Close();
                }
            }
        }
    }
}
