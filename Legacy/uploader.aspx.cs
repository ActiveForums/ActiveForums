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
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.IO;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class uploader : AMPageBase
    {

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            try
            {
                UserController uc = new UserController();
                User ui = uc.GetUser(PortalSettings.PortalId, -1);
                ForumController fc = new ForumController();
                Forum fi = fc.Forums_Get(Convert.ToInt32(Request.Params["ForumId"]), ui.UserId, true);

                if (fi != null)
                {
                    if (Permissions.HasPerm(fi.Security.Attach, ui.UserRoles))
                    {
                        if (inpFile.HasFile)
                        {
                            string sFile = string.Empty;
                            string sExt = string.Empty;
                            int maxImgHeight = fi.AttachMaxHeight;
                            int maxImgWidth = fi.AttachMaxWidth;
                            string contentType = inpFile.PostedFile.ContentType;
                            sFile = Path.GetFileName(inpFile.PostedFile.FileName).Replace(" ", "_");

                            sExt = Path.GetExtension(sFile);
                            if (sFile.Length >= 250)
                            {
                                sFile = sFile.Replace(sExt, string.Empty);
                                sFile = sFile.Substring(0, (250 - sExt.Length));
                                sFile = sFile + sExt;
                            }
                            sExt = sExt.Replace(".", string.Empty);
                            if (!(fi.AttachTypeAllowed.ToString().ToLower().Contains(sExt.ToLower())))
                            {
                                Response.Write("<script type=\"text/javascript\">window.top.af_setMessage('" + Utilities.GetSharedResource("[RESX:Error:BlockedFile]") + "');</script>");
                                return;
                            }
                            if (fi.AttachMaxSize > 0)
                            {
                                if ((inpFile.PostedFile.ContentLength / 1024.0) > fi.AttachMaxSize)
                                {
                                    Response.Write("<script type=\"text/javascript\">window.top.af_setMessage('" + string.Format(Utilities.GetSharedResource("[RESX:Error:FileTooLarge]"), fi.AttachMaxSize) + "');</script>");
                                    return;
                                }
                            }

                            Stream inpStream = inpFile.PostedFile.InputStream;
                            MemoryStream imgStream = new MemoryStream();
                            bool useMemStream = false;
                            bool allowDownload = true;
                            bool displayInline = false;
                            if (sExt.ToLower() == "jpg" || sExt.ToLower() == "gif" || sExt.ToLower() == "bmp" || sExt.ToLower() == "png" || sExt.ToLower() == "jpeg")
                            {
                                useMemStream = true;
                                imgStream = (MemoryStream)(Images.CreateImageForDB(inpStream, maxImgHeight, maxImgWidth));
                                contentType = "image/x-png";
                                allowDownload = false;
                                displayInline = true;
                            }

                            Data.AttachController ac = new Data.AttachController();
                            AttachInfo ai = new AttachInfo();
                            ai.ContentId = -1;
                            ai.UserID = ui.UserId;

                            ai.ContentType = contentType;
                            ai.DisplayInline = displayInline;
                            ai.AllowDownload = allowDownload;
                            ai.ParentAttachId = 0;
                            if (fi.AttachStore == AttachStores.DATABASE)
                            {
                                if (useMemStream)
                                {
                                    ai.FileSize = Convert.ToInt32(imgStream.Length);
                                    ai.FileData = imgStream.ToArray();
                                }
                                else
                                {

                                    byte[] byteData = new byte[Convert.ToInt32(inpStream.Length - 1) + 1];
                                    inpStream.Read(byteData, 0, Convert.ToInt32(inpStream.Length));
                                    ai.FileSize = Convert.ToInt32(inpStream.Length);
                                    ai.FileData = byteData;
                                }
                                ai.Filename = sFile;
                            }
                            else
                            {
                                if (useMemStream)
                                {
                                    ai.FileSize = Convert.ToInt32(imgStream.Length);
                                    ai.Filename = SaveToFile(imgStream, sFile);
                                }
                                else
                                {
                                    byte[] byteData = new byte[Convert.ToInt32(inpStream.Length) + 1];
                                    inpStream.Read(byteData, 0, Convert.ToInt32(inpStream.Length));
                                    ai.FileSize = Convert.ToInt32(inpStream.Length);
                                    ai.Filename = SaveToFile(inpFile, sFile);
                                }

                            }
                            int attachId = ac.Attach_Save(ai);
                            Response.Write("<script type=\"text/javascript\">window.top.af_isUploaded(" + attachId.ToString() + ");</script>");
                        }
                    }
                    else
                    {
                        inpFile.Visible = false;
                    }
                }
                else
                {
                    inpFile.Visible = false;
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script type=\"text/javascript\">window.top.af_setMessage('" + ex.Message + "');</script>");
            }

        }
        private string SaveToFile(MemoryStream sFile, string Filename)
        {
            string strPath = null;
            strPath = Request.MapPath(PortalSettings.HomeDirectory + "activeforums_Attach");
            if (!(Directory.Exists(strPath)))
            {
                Directory.CreateDirectory(strPath);
            }
            int i = 0;
            string sFullFile = strPath + "\\" + Filename;
            string tmpFileName = Filename;
            while (File.Exists(sFullFile))
            {
                i += 1;
                Filename = i.ToString().PadLeft(3, '0') + "_" + tmpFileName;
                sFullFile = strPath + "\\" + Filename;
            }
            FileStream fs = File.OpenWrite(sFullFile);
            fs.Write(sFile.GetBuffer(), 0, Convert.ToInt32(sFile.Position));
            fs.Close();
            return Filename;
        }

        private string SaveToFile(FileUpload inpFile, string Filename)
        {
            string strPath = null;
            strPath = Request.MapPath(PortalSettings.HomeDirectory + "activeforums_Attach");
            if (!(Directory.Exists(strPath)))
            {
                Directory.CreateDirectory(strPath);
            }
            int i = 0;
            string sFullFile = strPath + "\\" + Filename;
            while (File.Exists(sFullFile))
            {
                i += 1;
                Filename = i.ToString().PadLeft(3, '0') + "_" + Filename;
                sFullFile = strPath + "\\" + Filename;
            }
            inpFile.PostedFile.SaveAs(sFullFile);
            return Filename;
        }
    }
}
