using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet.Model.Attachments;

namespace VkAutoPostingBot
{
    public static class GetNewMems
    {
        public static IEnumerable<VkMem> Get(IReadOnlyCollection<VkNet.Model.Group> grInfo,
                                            ulong offset)
        {
            List<VkMem> memsForTg = new List<VkMem>();

            foreach (var g in grInfo)
            {
                Thread.Sleep(350);
                IReadOnlyCollection<Post> posts;

                try
                {
                    posts = Data.Api.Wall.Get(new VkNet.Model.RequestParams.WallGetParams
                    {
                        Count = 10,
                        Offset = offset,
                        OwnerId = g.Id * -1,
                        Extended = true,
                        Filter = VkNet.Enums.SafetyEnums.WallFilter.Owner
                    }).WallPosts;
                }
                catch
                {
                    Logger.Log("В черном списке у группы " + g.Name);
                    continue;
                }

                if (posts.Count == 0) continue;

                double likesKoef = posts.Where(w => w.Views != null).
                                Average(a => (double)a.Likes.Count / a.Views.Count) * 1.15;

                foreach (var p in posts)
                {
                    if (IfNeedToAddMem(p, likesKoef, g))
                    {
                        var photoUrls = from f in p.Attachments
                                        where f.Instance is Photo
                                        from w in (f.Instance as Photo).Sizes
                                        orderby w.Width
                                        group w by f.Instance into s
                                        select s.Last().Url.OriginalString;

                        memsForTg.Add(new VkMem(
                                            g.Id,
                                            p.Id.Value,
                                            p.Date.Value,
                                            p.Text,
                                            g,
                                            p.Attachments.Select(s => s.Instance),
                                            photoUrls));
                    }
                }
            }

            return memsForTg.OrderBy(o => o.Date);
        }

        private static bool IfNeedToAddMem(Post p, double likesKoef, VkNet.Model.Group g)
        {
            if (p.Attachment == null ||
                p.Attachment.Instance == null ||
                p.Attachments[0].Instance is not Photo ||
                p.Attachment.Instance.Id == null)
            {
                return false;
            }

            if (p.IsPinned != null && p.IsPinned.Value)
            {
                return false;
            }

            if (p.MarkedAsAds || p.PostType != VkNet.Enums.SafetyEnums.PostType.Post)
            {
                return false;
            }

            if ((double)p.Likes.Count / p.Views.Count < likesKoef)
            {
                return false;
            }

            if (Data.MemsPosted.Any(e => e.GroupID == g.Id && e.PostID == p.Id))
            {
                return false;
            }

            if (Data.MemsForTg.Any(e => e.GroupID == g.Id && e.PostID == p.Id))
            {
                return false;
            }

            return true;
        }
    }
}
