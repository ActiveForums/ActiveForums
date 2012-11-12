using System;

namespace DotNetNuke.Modules.ActiveForums
{
	public class AttachInfo
	{
	    private string _canRead = string.Empty;
		private string _fileUrl = string.Empty;

	    public string CanRead
		{
			get
			{
				return _canRead;
			}
			set
			{
				_canRead = value;
			}
		}

	    public int AttachID { get; set; }

	    public int ContentId { get; set; }

	    public int PostID { get; set; }

	    public int UserID { get; set; }

	    public string Filename { get; set; }

	    public Array FileData { get; set; }

	    public string ContentType { get; set; }

	    public int FileSize { get; set; }

	    public bool AllowDownload { get; set; }

	    public bool DisplayInline { get; set; }

	    public int DownloadCount { get; set; }

	    public int ParentAttachId { get; set; }

	    public string FileUrl
		{
			get
			{
				return _fileUrl;
			}
			set
			{
				_fileUrl = value;
			}
		}
	}
}
