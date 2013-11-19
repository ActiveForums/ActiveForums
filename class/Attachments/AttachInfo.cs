//
// Active Forums - http://www.dnnsoftware.com
// Copyright (c) 2013
// by DNN Corp.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
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
