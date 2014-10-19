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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Host;
using DotNetNuke.Modules.ActiveForums.Extensions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Api;
using DotNetNuke.Web.Api.Internal;
using System.Data;
using System.Web.Script.Serialization;
using System.Collections.Generic;


namespace DotNetNuke.Modules.ActiveForums
{
    [DnnAuthorize]
    [ValidateAntiForgeryToken]
    public class ForumServiceController : DnnApiController
    {
        public HttpResponseMessage CreateThumbnail(CreateThumbnailDTO dto)
        {
            var fileManager = FileManager.Instance;
            var folderManager = FolderManager.Instance;

            var file = fileManager.GetFile(dto.FileId);

            if (file == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "File Not Found");
                //return Json(new {Result = "error"});
            }
            
            var folder = folderManager.GetFolder(file.FolderId);

            var ext = file.Extension;
            if (!(ext.StartsWith(".")))
            {
                ext = "." + ext;
            }
            
            var sizedPhoto = file.FileName.Replace(ext, "_" + dto.Width.ToString(CultureInfo.InvariantCulture) + "x" + dto.Height.ToString(CultureInfo.InvariantCulture) + ext);

            var newPhoto = fileManager.AddFile(folder, sizedPhoto, fileManager.GetFileContent(file));
            ImageUtils.CreateImage(newPhoto.PhysicalPath, dto.Height, dto.Width);
            newPhoto = fileManager.UpdateFile(newPhoto);

            return Request.CreateResponse(HttpStatusCode.OK, newPhoto.ToJson());
        }

        public string EncryptTicket(EncryptTicketDTO dto)
        {
            var x = Common.Globals.LinkClick(dto.Url, ActiveModule.TabID, ActiveModule.ModuleID, false);
            return UrlUtils.EncryptParameter(UrlUtils.GetParameterValue(dto.Url));
        }

        [HttpPost]
        [IFrameSupportedValidateAntiForgeryToken]
        public Task<HttpResponseMessage> UploadFile()
        {
            // This method uploads an attachment to a temporary directory and returns a JSON object containing information about the original file
            // including the temporary file name.  When the post is saved/updated, the temporary file is moved to the appropriate attachment directory


            // Have to a reference to these variables as the internal reference isn't available.
            // in the async result.
            var request = Request;
            var portalSettings = PortalSettings;
            var userInfo = portalSettings.UserInfo;
            var forumUser = new UserController().GetUser(ActiveModule.PortalID, ActiveModule.ModuleID, userInfo.UserID);

            if (!request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotAcceptable));
            }

            const string uploadPath = "activeforums_Upload";

            var folderManager = FolderManager.Instance;
            if(!folderManager.FolderExists(ActiveModule.PortalID, uploadPath))
            {
                folderManager.AddFolder(ActiveModule.PortalID, uploadPath);
            }
            var folder = folderManager.GetFolder(ActiveModule.PortalID, uploadPath);

            var provider = new MultipartFormDataStreamProvider(folder.PhysicalPath);

            var task = request.Content.ReadAsMultipartAsync(provider).ContinueWith(t =>
            {
                if (t.IsFaulted || t.IsCanceled)
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);

                // Make sure a temp file was uploaded and that it exists
                var file = provider.FileData.FirstOrDefault();
                if (file == null || string.IsNullOrWhiteSpace(file.LocalFileName) || !File.Exists(file.LocalFileName))
                {
                    return request.CreateErrorResponse(HttpStatusCode.NoContent, "No File Found");
                }

                // Get the file name without the full path
                var localFileName = Path.GetFileName(file.LocalFileName).TextOrEmpty();

                // Check to make sure that a forum was specified and that the the user has upload permissions
                // This is only an initial check, it will be done again when the file is saved to a post.
                
                int forumId;
                if (!int.TryParse(provider.FormData["forumId"], out forumId))
                {
                    File.Delete(file.LocalFileName);
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Forum Not Specified");
                }

                // Make sure that we can find the forum and that attachments are allowed
                var fc = new ForumController();
                var forum = fc.Forums_Get(ActiveModule.PortalID, ActiveModule.ModuleID, forumId, userInfo.UserID, true, true, -1);

                if (forum == null || !forum.AllowAttach)
                {
                    File.Delete(file.LocalFileName);
                    return request.CreateErrorResponse(HttpStatusCode.BadRequest, "Forum Not Found");
                }

                // Make sure the user has permissions to attach files
                if(forumUser == null || !Permissions.HasPerm(forum.Security.Attach, forumUser.UserRoles))
                {
                    File.Delete(file.LocalFileName);
                    return request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Not Authorized");
                }

           
                // Make sure that the file size does not exceed the limit (in KB) for the forum
                // Have to do this since content length is not available when using MultipartFormDataStreamProvider
                var di = new DirectoryInfo(folder.PhysicalPath);
                var fileSize = di.GetFiles(localFileName)[0].Length;

                var maxAllowedFileSize = (long)forum.AttachMaxSize*1024;

                if((forum.AttachMaxSize > 0) && (fileSize > maxAllowedFileSize))
                {
                    File.Delete(file.LocalFileName);
                    return request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Exceeds Max File Size");
                }


                // Get the original file name from the content disposition header
                var fileName = file.Headers.ContentDisposition.FileName.Replace("\"", "");

                if(string.IsNullOrWhiteSpace(fileName))
                {
                    File.Delete(file.LocalFileName);
                    return request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Invalid File");
                }


                // Make sure we have an acceptable extension type.
                // Check against both the forum configuration and the host configuration
                var extension = Path.GetExtension(fileName).TextOrEmpty().Replace(".", string.Empty).ToLower();
                var isForumAllowedExtension = string.IsNullOrWhiteSpace(forum.AttachTypeAllowed) || forum.AttachTypeAllowed.Replace(".", "").Split(',').Any(val => val == extension);
                if(string.IsNullOrEmpty(extension) || !isForumAllowedExtension || !Host.AllowedExtensionWhitelist.IsAllowedExtension(extension))
                {
                    File.Delete(file.LocalFileName);
                    return request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "File Type Not Allowed");
                }
                

                // IE<=9 Hack - can't return application/json
                var mediaType = "application/json";
                if (!request.Headers.Accept .Any(h => h.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase)))
                    mediaType = "text/html";
                   
                var result = new ClientAttachment() {
                    ContentType = file.Headers.ContentType.MediaType,
                    FileName = fileName,
                    FileSize = fileSize,
                    UploadId =  localFileName
                };

                return Request.CreateResponse(HttpStatusCode.Accepted, result, mediaType);
            });

            return task;
        }

        [HttpGet]
        public HttpResponseMessage GetTopicList(int ForumId)
        {
            var portalSettings = PortalSettings;
            var userInfo = portalSettings.UserInfo;
            var forumUser = new UserController().GetUser(portalSettings.PortalId, ActiveModule.ModuleID, userInfo.UserID);
            var fc = new ForumController();
            var forumIds = fc.GetForumsForUser(forumUser.UserRoles, portalSettings.PortalId, ActiveModule.ModuleID, "CanEdit");

            DataSet ds = DataProvider.Instance().UI_TopicsView(portalSettings.PortalId, ActiveModule.ModuleID, ForumId, userInfo.UserID, 0, 20, userInfo.IsSuperUser, SortColumns.ReplyCreated);
            if (ds.Tables.Count > 0)
            {
                DataTable dtTopics = ds.Tables[3];

                Dictionary<string, string> rows = new Dictionary<string, string>(); ;
                foreach (DataRow dr in dtTopics.Rows)
                {
                    rows.Add(dr["TopicId"].ToString(), dr["Subject"].ToString());
                }

                return Request.CreateResponse(HttpStatusCode.OK, rows.ToJson());
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);
        }

        [HttpGet]
        public HttpResponseMessage GetForumsList()
        {
            var portalSettings = PortalSettings;
            var userInfo = portalSettings.UserInfo;
            var forumUser = new UserController().GetUser(portalSettings.PortalId, ActiveModule.ModuleID, userInfo.UserID);
            var fc = new ForumController();
            var forumIds = fc.GetForumsForUser(forumUser.UserRoles, portalSettings.PortalId, ActiveModule.ModuleID, "CanEdit");

            DataTable ForumTable = fc.GetForumView(portalSettings.PortalId, ActiveModule.ModuleID, userInfo.UserID, userInfo.IsSuperUser, forumIds);

            Dictionary<string, string> rows = new Dictionary<string, string>();;
            foreach (DataRow dr in ForumTable.Rows)
            {
                rows.Add(dr["ForumId"].ToString(),dr["ForumName"].ToString());
            }
            return Request.CreateResponse(HttpStatusCode.OK, rows.ToJson());
        }

        public class CreateSplitDTO
        {
            public int OldTopicId { get; set; }
            public int NewTopicId { get; set; }
            public int NewForumId { get; set; }
            public string Subject { get; set; }
            public string Replies { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage CreateSplit(CreateSplitDTO dto)
        {
            //var modSplit = Permissions.HasPerm(_drSecurity["CanModSplit"].ToString(), ForumUser.UserRoles);

            var portalSettings = PortalSettings;
            var userInfo = portalSettings.UserInfo;
            var forumUser = new UserController().GetUser(portalSettings.PortalId, ActiveModule.ModuleID, userInfo.UserID);
            var fc = new ForumController();
            //var forumIds = fc.GetForumsForUser(forumUser.UserRoles, portalSettings.PortalId, ActiveModule.ModuleID, "CanEdit");
            
            var tc = new TopicsController();

            int topicId;

            if (dto.NewTopicId < 1)
            {   
                var subject = Utilities.CleanString(portalSettings.PortalId, dto.Subject, false, EditorTypes.TEXTBOX, false, false, ActiveModule.ModuleID, string.Empty, false);
                
                topicId = tc.Topic_QuickCreate(portalSettings.PortalId, ActiveModule.ModuleID, dto.NewForumId, subject, string.Empty, userInfo.UserID, userInfo.DisplayName, true, userInfo.LastIPAddress);
            }
            else
            {
                topicId = dto.NewTopicId;
            }

            tc.Replies_Split(dto.OldTopicId, topicId, dto.Replies);

            return Request.CreateResponse(HttpStatusCode.OK);
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
