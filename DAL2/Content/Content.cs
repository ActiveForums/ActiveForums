using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DotNetNuke.Modules.ActiveForums.DAL2
{
    [TableName("activeforums_Content")]
    [PrimaryKey("ContentId")]
    class Content
    {
        public int ContentId { get; set; }
        public string Subject { get; set; }
        public string Summary { get; set; }
        public string Body { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public bool IsDeleted { get; set; }
        public string IPAddress { get; set; }
        public int ContentItemId { get; set; }
    }
}
