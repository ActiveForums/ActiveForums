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
