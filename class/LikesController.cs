using DotNetNuke.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNuke.Modules.ActiveForums
{
    class LikesController
    {
        public List<Likes> GetForPost(int postId)
        {
            List<Likes> likes = new List<Likes>();
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Likes>();
                likes = rep.Find("WHERE PostId = @0 AND Checked = 1", postId).ToList();
            }
            return likes;
        }

        public void Like(int contentId, int userId)
        {
            using (IDataContext ctx = DataContext.Instance())
            {
                var rep = ctx.GetRepository<Likes>();
                var like = rep.Find("Where PostId = @0 AND UserId = @1", contentId, userId).FirstOrDefault();

                if (like != null)
                {
                    if (like.Checked)
                        like.Checked = false;
                    else
                        like.Checked = true;
                    rep.Update(like);
                }
                else
                {
                    like = new Likes();
                    like.PostId = contentId;
                    like.UserId = userId;
                    like.Checked = true;
                    rep.Insert(like);
                }
            }
        }
    }
}
