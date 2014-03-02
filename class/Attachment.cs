using System.Runtime.Serialization;

namespace DotNetNuke.Modules.ActiveForums
{
    // Used to exchange data with SQL
    public class Attachment
    {      
        public int AttachmentId { get; set; }

        public int ContentId { get; set; }

        public int UserId { get; set; }
        
        public string FileName { get; set; }
       
        public long FileSize { get; set; }

        public string ContentType { get; set; }
        
        public int? FileId { get; set; }

        public byte[] FileData { get; set; }

        public bool? AllowDownload { get; set; }
    }

    public class PermissionAttachment : Attachment
    {
        public string CanRead { get; set; }
    }

    // Used to exchange data with the client via JSON
    [DataContract]
    public class ClientAttachment
    {
        [DataMember(Name="id",IsRequired = false, EmitDefaultValue = false)]
        public int? AttachmentId { get; set; }
        
        [DataMember(Name="fileName", IsRequired = true)]
        public string FileName { get; set; }

        [DataMember(Name = "contentType", IsRequired = true)]
        public string ContentType { get; set; }

        [DataMember(Name = "fileSize")]
        public long FileSize { get; set; }

        [DataMember(Name = "fileId", IsRequired = false, EmitDefaultValue = false)]
        public int? FileId { get; set; }

        [DataMember(Name = "uploadId", IsRequired = false, EmitDefaultValue = false)]
        public string UploadId { get; set; }
    }
}
