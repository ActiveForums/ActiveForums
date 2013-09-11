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
using System.Globalization;
using System.Net;
using System.Net.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;


namespace DotNetNuke.Modules.ActiveForums
{
    [ValidateAntiForgeryToken]
    public class ForumServiceController : DnnApiController
    {
        [DnnAuthorize()]
        public HttpResponseMessage CreateThumbnail(CreateThumbnailDTO dto)
        {
            IFileManager _fileManager = FileManager.Instance;
            IFolderManager _folderManager = FolderManager.Instance;

            IFileInfo _file = _fileManager.GetFile(dto.FileId);

            if (_file == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "File Not Found");
                //return Json(new {Result = "error"});
            }
            IFolderInfo _folder = _folderManager.GetFolder(_file.FolderId);

            string ext = _file.Extension;
            if (!(ext.StartsWith(".")))
            {
                ext = "." + ext;
            }
            string sizedPhoto = _file.FileName.Replace(ext, "_" + dto.Width.ToString(CultureInfo.InvariantCulture) + "x" + dto.Height.ToString(CultureInfo.InvariantCulture) + ext);

            IFileInfo newPhoto = _fileManager.AddFile(_folder, sizedPhoto, _fileManager.GetFileContent(_file));
            sizedPhoto = ImageUtils.CreateImage(newPhoto.PhysicalPath, dto.Height, dto.Width);
            newPhoto = _fileManager.UpdateFile(newPhoto);

            return Request.CreateResponse(HttpStatusCode.OK, newPhoto.ToJson());
        }

        [DnnAuthorize()]
        public string EncryptTicket(EncryptTicketDTO dto)
        {
            return UrlUtils.EncryptParameter(UrlUtils.GetParameterValue(dto.Url));
        }

        public class CreateThumbnailDTO
        {
            public int FileId { get; set; }
            public int Height { get; set; }
            public int Width { get; set; }
        }

        public class EncryptTicketDTO
        {
            public string Url { get; set; }
        }
    }
}
