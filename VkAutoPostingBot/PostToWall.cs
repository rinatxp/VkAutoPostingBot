using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;

namespace VkAutoPostingBot
{
    internal static class PostToWall
    {
        public static void Post(VkMem mem)
        {
            string cpRight = $"https://vk.com/{mem.Group.ScreenName}?w=wall-{mem.GroupID}_{mem.PostID}";

            try
            {
                Data.Api.Wall.Post(new VkNet.Model.RequestParams.WallPostParams
                {
                    Copyright = cpRight,
                    Signed = false,
                    OwnerId = -Config.GROUP_ID,
                    FromGroup = true,
                    Message = mem.Message,
                    Attachments = mem.Attachments
                });
            }
            catch
            {
                Logger.Log("Не удалось запостить мем: " + cpRight);
                throw;
            }
        }
    }
}
