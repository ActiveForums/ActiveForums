using DotNetNuke.ComponentModel.DataAnnotations;
using System.Web.Caching;

namespace DotNetNuke.Modules.ActiveForums
{
    [TableName("activeforums_Likes")]
    [PrimaryKey("Id", AutoIncrement = true)]
    [Scope("PostId")]
    [Cacheable("activeforums_Likes", CacheItemPriority.Normal)]
    class Likes
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public bool Checked { get; set; }
    }
}
