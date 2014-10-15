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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using DotNetNuke.Entities.Users;
using DotNetNuke.Modules.ActiveForums.Extensions;
using DotNetNuke.Services.FileSystem;


namespace DotNetNuke.Modules.ActiveForums
{
    public class af_viewer : Framework.PageBase
    {

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [DebuggerStepThrough]
        private void InitializeComponent()
        {
        }

        //NOTE: The following placeholder declaration is required by the Web Form Designer.
        //Do not delete or move it.
        private object designerPlaceholderDeclaration;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //CODEGEN: This method call is required by the Web Form Designer
            //Do not modify it using the code editor.
            InitializeComponent();
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var attachmentId = Utilities.SafeConvertInt(Request.Params["AttachmentID"], -1);// Used for new attachments where the attachment is the actual file link (shouldn't appear in posts)
            var attachFileId = Utilities.SafeConvertInt(Request.Params["AttachID"], -1); // Used for legacy attachments where the attachid was actually the file id. (appears in posts)
            var portalId = Utilities.SafeConvertInt(Request.Params["PortalID"], -1);
            var moduleId = Utilities.SafeConvertInt(Request.Params["ModuleID"], -1);

            if (Page.IsPostBack || (attachmentId < 0 && attachFileId < 0) || portalId < 0 || moduleId < 0)
            {
                Response.StatusCode = 400;
                Response.Write("Invalid Request");
                Response.End();
                return;
            }

            // Get the attachment including the "Can Read" permission for the associated content id.
            var attachment = new Data.AttachController().Get(attachmentId, attachFileId, true);

            // Make sure the attachment exists
            if (attachment == null)
            {
                Response.StatusCode = 404;
                Response.Write("Not Found");
                Response.End();
                return;
            }

            // Make sure the user has read access
            var u = new UserController().GetUser(portalId, moduleId);
            if (u == null || !Permissions.HasAccess(attachment.CanRead, u.UserRoles))
            {
                Response.StatusCode = 401;
                Response.Write("Unauthorized");
                Response.End();
                return;
            }

            // Get the filename with the unique identifier prefix removed.
            var filename = Regex.Replace(attachment.FileName.TextOrEmpty(), @"__\d+__\d+__", string.Empty);

            // Some legacy attachments may still be stored in the DB.
            if (attachment.FileData != null)
            {
                Response.ContentType = attachment.ContentType;
                
                if (attachmentId > 0)
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.HtmlEncode(filename));
                else // Handle legacy inline attachments a bit differently
                    Response.AddHeader("Content-Disposition", "filename=" + Server.HtmlEncode(filename));
                
                Response.BinaryWrite(attachment.FileData);
                Response.End();
                return;
            }

            var fileManager = FileManager.Instance;

            string filePath = null;

            // If there is a file id, access the file using the file manager
            if (attachment.FileId.HasValue && attachment.FileId.Value > 0)
            {
                var file = fileManager.GetFile(attachment.FileId.Value);
                if (file != null)
                {
                    filePath = file.PhysicalPath;
                }
            }
                // Otherwise check the attachments directory (current and legacy)
            else
            {
                filePath = Server.MapPath(PortalSettings.HomeDirectory + "activeforums_Attach/") + attachment.FileName;

                // This is another check to support legacy attachments.
                if (!File.Exists(filePath))
                {
                    filePath = Server.MapPath(PortalSettings.HomeDirectory + "NTForums_Attach/") + attachment.FileName;
                }
            }

            // At this point, we should have a valid file path
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Response.StatusCode = 404;
                Response.Write("Not Found");
                Response.End();
                return;
            }

            var length = attachment.FileSize;
            if (length <= 0)
                length = new System.IO.FileInfo(filePath).Length;

            Response.Clear();
            Response.ContentType = attachment.ContentType;

            if(attachmentId > 0)
                Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.HtmlEncode(filename));
            else // Handle legacy inline attachments a bit differently
                Response.AddHeader("Content-Disposition", "filename=" + Server.HtmlEncode(filename));

            Response.AddHeader("Content-Length", length.ToString());
            Response.WriteFile(filePath);
            Response.Flush();
            Response.Close();
            Response.End();
        }
    }
}
