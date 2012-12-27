using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using DotNetNuke.Web.Services;
using System.Web;
using System.Web.Mvc;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Services.Journal;
using DotNetNuke.Services.FileSystem;
using System.Globalization;


namespace DotNetNuke.Modules.ActiveForums
{
	[ValidateAntiForgeryToken()]
	public class ForumServiceController : DnnController
	{
		[DnnAuthorize()]
		public ActionResult CreateThumbnail(int FileId, int Height, int Width)
		{

			IFileManager _fileManager = FileManager.Instance;
			IFolderManager _folderManager = FolderManager.Instance;

			IFileInfo _file = _fileManager.GetFile(FileId);

			if (_file == null)
			{
//ORIGINAL LINE: return Json(new With {.Result = "error"})
//INSTANT C# WARNING: Every field in a C# anonymous type initializer is immutable.
				return Json(new {Result = "error"});
			}
			IFolderInfo _folder = _folderManager.GetFolder(_file.FolderId);

			string ext = _file.Extension;
			if (! (ext.StartsWith(".")))
			{
				ext = "." + ext;
			}
			string sizedPhoto = _file.FileName.Replace(ext, "_" + Width.ToString(CultureInfo.InvariantCulture) + "x" + Height.ToString(CultureInfo.InvariantCulture) + ext);

			IFileInfo newPhoto = _fileManager.AddFile(_folder, sizedPhoto, _fileManager.GetFileContent(_file));
			sizedPhoto = ImageUtils.CreateImage(newPhoto.PhysicalPath, Height, Width);
			newPhoto = _fileManager.UpdateFile(newPhoto);

			return Json(newPhoto);

		}
		[DnnAuthorize()]
		public string EncryptTicket(string Url)
		{
			return UrlUtils.EncryptParameter(UrlUtils.GetParameterValue(Url));
		}

	}
}
