using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.IO;
using DotNetNuke.Services.FileSystem;

//© 2004 - 2005 ActiveModules, Inc. All Rights Reserved
namespace DotNetNuke.Modules.ActiveForums
{
    public class af_viewer : DotNetNuke.Framework.PageBase
    {

        #region  Web Form Designer Generated Code

        //This call is required by the Web Form Designer.
        [System.Diagnostics.DebuggerStepThrough()]
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

            //Put user code to initialize the page here
            try
            {
                byte[] bindata = null;
                bool canView = false;
                string sContentType = string.Empty;
                if (!Page.IsPostBack)
                {
                    int AttachId = 0;
                    int intPortalID = 0;
                    int intModuleID = 0;
                    if (Request.Params["AttachID"] != null)
                    {
                        if (SimulateIsNumeric.IsNumeric(Request.Params["AttachID"]))
                        {
                            AttachId = Int32.Parse(Request.Params["AttachID"]);
                        }
                        else
                        {
                            AttachId = 0;
                        }
                    }
                    else
                    {
                        AttachId = 0;
                    }
                    if (Request.Params["PortalID"] != null)
                    {
                        if (SimulateIsNumeric.IsNumeric(Request.Params["PortalID"]))
                        {
                            intPortalID = Int32.Parse(Request.Params["PortalID"]);
                        }
                        else
                        {
                            intPortalID = 0;
                        }
                    }
                    else
                    {
                        intPortalID = 0;
                    }
                    if (Request.Params["ModuleID"] != null)
                    {
                        if (SimulateIsNumeric.IsNumeric(Request.Params["ModuleID"]))
                        {
                            intModuleID = Int32.Parse(Request.Params["ModuleID"]);
                        }
                        else
                        {
                            intModuleID = -1;
                        }
                    }
                    else
                    {
                        intModuleID = -1;
                    }
                    IFileManager _fileManager = FileManager.Instance;
                    IFileInfo _file = null;
                    if (AttachId > 0)
                    {
                        DotNetNuke.Entities.Users.UserInfo ui = DotNetNuke.Entities.Users.UserController.GetCurrentUserInfo();
                        //DotNetNuke.Modules.ActiveForums.Settings.LoadUser(objUserInfo.UserID, intPortalID, intModuleID)
                        UserController uc = new UserController();
                        User u = uc.GetUser(intPortalID, intModuleID);

                        Data.AttachController ac = new Data.AttachController();
                        AttachInfo ai = null;
                        try
                        {
                            if (Request.UrlReferrer.AbsolutePath.Contains("HtmlEditorProviders") | (Request.UrlReferrer.AbsolutePath.Contains("afv") & Request.UrlReferrer.AbsolutePath.Contains("post")))
                            {
                                ai = ac.Attach_Get(AttachId, -1, ui.UserID, false);
                            }
                            else
                            {
                                ai = ac.Attach_Get(AttachId, -1, ui.UserID, true);
                            }
                        }
                        catch (Exception ex)
                        {
                            ai = ac.Attach_Get(AttachId, -1, ui.UserID, true);
                        }
                        if (ai == null)
                        {
                            ai = new AttachInfo();
                            _file = _fileManager.GetFile(AttachId);
                            ai.AttachID = _file.FileId;
                            ai.AllowDownload = true;
                            ai.Filename = _file.FileName;
                            ai.FileUrl = _file.PhysicalPath;
                            ai.CanRead = "0;1;-3;-1;|||";
                            ai.ContentType = _file.ContentType;
                        }

                        if (ai != null & u != null)
                        {
                            Response.ContentType = ai.ContentType.ToString();
                            if (ai.FileData != null)
                            {
                                if (Permissions.HasAccess(ai.CanRead, u.UserRoles))
                                {
                                    bindata = (byte[])ai.FileData;
                                    Response.BinaryWrite(bindata);
                                    Response.AddHeader("Content-Disposition", "attachment;filename=" + Server.HtmlEncode(ai.Filename.ToString()));
                                }

                            }
                            else
                            {
                                if (Permissions.HasAccess(ai.CanRead, u.UserRoles))
                                {
                                    string fpath = string.Empty;
                                    string fName = string.Empty;
                                    if (string.IsNullOrEmpty(ai.FileUrl))
                                    {
                                        fpath = Server.MapPath(PortalSettings.HomeDirectory + "activeforums_Attach/");
                                        fpath += ai.Filename;
                                        fName = System.IO.Path.GetFileName(fpath);
                                    }
                                    else
                                    {

                                        _file = _fileManager.GetFile(ai.AttachID);
                                        fpath = _file.PhysicalPath;
                                        fName = _file.FileName;
                                    }

                                    if (System.IO.File.Exists(fpath))
                                    {

                                        //Dim vpath As String
                                        //vpath = PortalSettings.HomeDirectory & "activeforums_Attach/" & Server.HtmlEncode(ai.Filename)
                                        FileStream fs = new FileStream(fpath, FileMode.Open, FileAccess.Read);
                                        long contentLength = 0;
                                        if (fs != null)
                                        {
                                            bindata = GetStreamAsByteArray(fs);
                                            fs.Close();
                                        }
                                        string sExt = System.IO.Path.GetExtension(fName);
                                        Response.Clear();
                                        Response.AddHeader("Content-Disposition", "attachment; filename=" + Server.HtmlEncode(fName));
                                        Response.AddHeader("Content-Length", bindata.LongLength.ToString());
                                        sContentType = ai.ContentType;
                                        switch (sExt.ToLowerInvariant())
                                        {
                                            case ".png":
                                                sContentType = "image/png";
                                                break;
                                            case ".jpg":
                                            case ".jpeg":
                                                sContentType = "image/jpeg";
                                                break;
                                            case ".gif":
                                                sContentType = "image/gif";
                                                break;
                                            case ".bmp":
                                                sContentType = "image/bmp";
                                                break;
                                        }


                                        Response.ContentType = sContentType;
                                        Response.OutputStream.Write(bindata, 0, bindata.Length);
                                        Response.End();
                                    }
                                    else
                                    {
                                        fpath = Server.MapPath(PortalSettings.HomeDirectory + "NTForums_Attach/");
                                        fpath += ai.Filename;
                                        if (System.IO.File.Exists(fpath))
                                        {
                                            string vpath = null;
                                            vpath = PortalSettings.HomeDirectory + "activeforums_Attach/" + Server.HtmlEncode(ai.Filename);
                                            Response.Redirect(Page.ResolveUrl(vpath));
                                        }
                                    }
                                }


                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }

        }
        private byte[] GetStreamAsByteArray(System.IO.Stream stream)
        {
            int streamLength = Convert.ToInt32(stream.Length);
            byte[] fileData = new byte[Convert.ToInt32(stream.Length - 1) + 1];
            stream.Read(fileData, 0, Convert.ToInt32(stream.Length));
            stream.Close();
            return fileData;
        }
    }
}
