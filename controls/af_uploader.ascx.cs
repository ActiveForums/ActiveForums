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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_uploader : ForumBase
    {
        private int PendingAttach = 0;
        public int ContentId = -1;
        public int AuthorId = -1;
        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cbAttach.CallbackEvent += cbAttach_Callback;
            cbMyFiles.CallbackEvent += cbMyFiles_Callback;

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Permissions.HasPerm(ForumInfo.Security.Attach, ForumUser.UserRoles) || Permissions.HasPerm(ForumInfo.Security.ModEdit, ForumUser.UserRoles))
            {
                btnUpload.ImageUrl = ImagePath + "/images/upload16.png";
                btnUpload.ObjectId = "btnUpload";
                btnUpload.Text = GetSharedResource("[RESX:Upload]");
                if (!cbAttach.IsCallback)
                {
                    BindAttach(string.Empty);
                    BindMyFiles();
                }
            }

        }
        private void cbAttach_Callback(object sender, Modules.ActiveForums.Controls.CallBackEventArgs e)
        {
            string attachIds = e.Parameters[1].ToString();
            switch (e.Parameters[0].ToLowerInvariant())
            {

                case "delcont":
                    {
                        if (SimulateIsNumeric.IsNumeric(e.Parameters[2]))
                        {
                            int aid = Convert.ToInt32(e.Parameters[2]);
                            int uid = -1;
                            if (SimulateIsNumeric.IsNumeric(e.Parameters[3]))
                            {
                                uid = Convert.ToInt32(e.Parameters[3]);
                            }
                            if ((uid == this.UserId && !(this.UserId == -1)) | Permissions.HasPerm(ForumInfo.Security.ModDelete, ForumUser.UserRoles) || UserInfo.IsSuperUser)
                            {
                                Data.AttachController adb = new Data.AttachController();
                                adb.Attach_Delete(aid, ContentId);
                                //ac.Attach_Delete(aid, -1, uid)
                            }

                        }
                        break;
                    }
                case "thumb":
                    {
                        if (SimulateIsNumeric.IsNumeric(e.Parameters[2]))
                        {
                            int aid = Convert.ToInt32(e.Parameters[2]);
                            Data.AttachController ac = new Data.AttachController();
                            int uid = -1;
                            if (SimulateIsNumeric.IsNumeric(e.Parameters[3]))
                            {
                                uid = Convert.ToInt32(e.Parameters[3]);
                            }
                            AttachInfo ai = ac.Attach_Get(aid, -1, uid, false);
                            if (ai != null)
                            {
                                int w = Convert.ToInt32(e.Parameters[4]);
                                int h = Convert.ToInt32(e.Parameters[5]);
                                System.IO.MemoryStream imgStream = new System.IO.MemoryStream();
                                string fpath = string.Empty;
                                int fileSize = 0;
                                string tmpFilename = string.Empty;
                                if (ai.FileData != null)
                                {
                                    byte[] bindata = null;
                                    bindata = (byte[])ai.FileData;
                                    System.IO.MemoryStream memStream = new System.IO.MemoryStream(bindata);
                                    imgStream = (System.IO.MemoryStream)(Images.CreateImageForDB(memStream, h, w));
                                    fileSize = Convert.ToInt32(imgStream.Length);
                                    tmpFilename = "thumb_" + ai.Filename;
                                }
                                else
                                {

                                    fpath = Server.MapPath(PortalSettings.HomeDirectory + "activeforums_Attach/");
                                    //fpath &= "thumb_" & ai.Filename
                                    tmpFilename = "thumb_" + ai.Filename;
                                    string sFullFile = fpath + tmpFilename;
                                    int i = 0;

                                    while (File.Exists(sFullFile))
                                    {
                                        i += 1;
                                        tmpFilename = i.ToString().PadLeft(3, '0') + "_thumb_" + ai.Filename;
                                        sFullFile = fpath + tmpFilename;
                                    }
                                    File.Copy(fpath + ai.Filename, sFullFile);
                                    Images.CreateImage(sFullFile, h, w);
                                    fileSize = (int)new FileInfo(sFullFile).Length;
                                }
                                AttachInfo aiThumb = new AttachInfo();
                                aiThumb.ContentId = -1;
                                aiThumb.UserID = ai.UserID;
                                aiThumb.Filename = tmpFilename;
                                aiThumb.ContentType = "image/x-png";
                                aiThumb.FileSize = fileSize;
                                if (ForumInfo.AttachStore == AttachStores.DATABASE)
                                {
                                    aiThumb.FileData = imgStream.ToArray();
                                    //File.Delete(fpath & "thumb_" & ai.Filename)
                                }
                                aiThumb.ParentAttachId = aid;
                                int thumbId = ac.Attach_Save(aiThumb);
                                attachIds += thumbId.ToString() + ";";
                                BindMyFiles();
                                if (Convert.ToBoolean(e.Parameters[4]))
                                {
                                    string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + "/";
                                    string s = "<script type=\"text/javascript\">";
                                    string sInsert = string.Empty;
                                    if (ForumInfo.AllowHTML && ForumInfo.EditorType != EditorTypes.TEXTBOX)
                                    {
                                        sInsert = "<a href=\"" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + aid + "\" target=\"_blank\"><img src=" + strHost + "DesktopModules/ActiveForums/viewer.aspx?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + thumbId + " border=0 /></a>";
                                    }
                                    else
                                    {
                                        sInsert = "[THUMBNAIL:" + thumbId.ToString() + ":" + aid + "]";
                                    }

                                    s += "amaf_insertHTML('" + sInsert + "');";
                                    s += "</script>";
                                    LiteralControl litScript = new LiteralControl();
                                    litScript.Text = s;
                                    plhAttach.Controls.Add(litScript);

                                }


                            }

                        }
                        break;
                    }
                case "inline":
                    {
                        if (SimulateIsNumeric.IsNumeric(e.Parameters[2]))
                        {
                            int aid = Convert.ToInt32(e.Parameters[2]);
                            Data.AttachController ac = new Data.AttachController();
                            int uid = -1;
                            if (SimulateIsNumeric.IsNumeric(e.Parameters[3]))
                            {
                                uid = Convert.ToInt32(e.Parameters[3]);
                            }
                            AttachInfo ai = ac.Attach_Get(aid, -1, uid, false);
                            if (ai != null)
                            {
                                int opt = Convert.ToInt32(e.Parameters[4]);
                                if (opt == 0)
                                {
                                    ai.DisplayInline = true;
                                    ai.AllowDownload = false;
                                }
                                else
                                {
                                    if (ai.AllowDownload)
                                    {
                                        ai.DisplayInline = true;
                                        ai.AllowDownload = false;
                                    }
                                    else
                                    {
                                        ai.DisplayInline = false;
                                        ai.AllowDownload = true;
                                    }
                                }


                                ac.Attach_Save(ai);
                            }
                        }
                        break;
                    }
            }

            BindAttach(attachIds);
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
            plhAttach.RenderControl(htmlWriter);
            string html = stringWriter.GetStringBuilder().ToString();
            html = Utilities.LocalizeControl(html);
            LiteralControl lit = new LiteralControl();
            lit.Text = html;
            lit.RenderControl(e.Output);
        }
        private void cbMyFiles_Callback(object sender, Modules.ActiveForums.Controls.CallBackEventArgs e)
        {
            string attachIds = e.Parameters[1].ToString();
            switch (e.Parameters[0].ToLowerInvariant())
            {

                case "del":
                    if (SimulateIsNumeric.IsNumeric(e.Parameters[2]))
                    {
                        int aid = Convert.ToInt32(e.Parameters[2]);
                        Data.AttachController ac = new Data.AttachController();
                        int uid = -1;
                        if (SimulateIsNumeric.IsNumeric(e.Parameters[3]))
                        {
                            uid = Convert.ToInt32(e.Parameters[3]);
                        }
                        if ((uid == this.UserId && !(this.UserId == -1)) | Permissions.HasPerm(ForumInfo.Security.ModDelete, ForumUser.UserRoles) || UserInfo.IsSuperUser)
                        {
                            ac.Attach_Delete(aid, -1, uid);
                        }

                    }


                    break;
            }
            PendingAttach = 0;
            plhMyFiles.Controls.Clear();
            BindMyFiles();
            System.IO.StringWriter stringWriter = new System.IO.StringWriter();
            HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);

            plhMyFiles.RenderControl(htmlWriter);
            string html = stringWriter.GetStringBuilder().ToString();
            html = Utilities.LocalizeControl(html);
            LiteralControl lit = new LiteralControl();
            lit.Text = html;
            lit.RenderControl(e.Output);
        }
        #endregion
        #region Private Methods
        private string BuildAttachGrid(string AttachIds)
        {
            return BuildAttachGrid(AuthorId, AttachIds, -1, -1, string.Empty, string.Empty);
        }
        private string BuildAttachGrid(int Uid, string AttachIds, int RowIndex, int MaxRows, string SortColumn, string Sort)
        {
            int mode = 0;
            if (RowIndex > -1)
            {
                mode = 1;
            }
            PendingAttach = 0;
            string sOut = string.Empty;
            Data.AttachController ac = new Data.AttachController();
            int i = 0;
            string strHost = DotNetNuke.Common.Globals.AddHTTP(DotNetNuke.Common.Globals.GetDomainName(Request)) + "/";
            List<AttachInfo> al = null;
            if (RowIndex == -1)
            {
                al = ac.Attach_ListAttachFiles(Uid, AttachIds);
            }
            else
            {
                al = ac.Attach_ListMyFiles(Uid, RowIndex, MaxRows, SortColumn, Sort);
            }
            foreach (AttachInfo ai in al)
            {
                PendingAttach += 1;
                string insertHTML = string.Empty;
                string insertThumb = string.Empty;
                string toggleInline = string.Empty;
                string deleteAttach = Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/delete12.png");
                if (mode == 1)
                {
                    deleteAttach = "<img src=\"" + deleteAttach + "\" style=\"cursor:pointer;\" onclick=\"af_delAttach(" + ai.AttachID.ToString() + "," + ai.UserID.ToString() + ");\" />";
                }
                else
                {
                    deleteAttach = "<img src=\"" + deleteAttach + "\" style=\"cursor:pointer;\" onclick=\"af_delContAttach(" + ai.AttachID.ToString() + "," + ai.UserID.ToString() + ");\" />";
                }
                if (ai.Filename.ToLowerInvariant().Contains(".jpg") | ai.Filename.ToLowerInvariant().Contains(".bmp") | ai.Filename.ToLowerInvariant().Contains(".gif") | ai.Filename.ToLowerInvariant().Contains(".png") | ai.Filename.ToLowerInvariant().Contains(".jpeg"))
                {
                    int w = 0;
                    int h = 0;
                    string fpath = null;
                    fpath = Server.MapPath(PortalSettings.HomeDirectory + "activeforums_Attach/");
                    fpath += ai.Filename;
                    byte[] bindata = null;
                    bindata = (byte[])ai.FileData;
                    if (bindata != null)
                    {
                        System.IO.MemoryStream memStream = new System.IO.MemoryStream(bindata);
                        try
                        {
                            System.Drawing.Image g = System.Drawing.Image.FromStream(memStream);
                            if (g != null)
                            {
                                w = g.Width;
                                h = g.Height;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {

                        try
                        {
                            System.Drawing.Image g = System.Drawing.Image.FromFile(fpath);
                            if (g != null)
                            {
                                w = g.Width;
                                h = g.Height;
                            }
                        }
                        catch (Exception ex)
                        {

                        }


                    }

                    if (mode == 0)
                    {
                        string sInsert = string.Empty;
                        string sClose = string.Empty;
                        if (ai.ParentAttachId > 0)
                        {
                            sInsert = "<a href=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/viewer.aspx") + "?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + ai.ParentAttachId + "\" target=\"_blank\">";
                            sClose = "</a>";
                        }
                        if (ai.FileData == null)
                        {
                            string vpath = null;
                            vpath = PortalSettings.HomeDirectory + "activeforums_Attach/";
                            sInsert += "<img src=\"" + vpath + ai.Filename + "\" border=\"0\" class=\"afimg\" />";
                        }
                        else
                        {
                            sInsert += "<img src=" + Page.ResolveUrl("~/DesktopModules/ActiveForums/viewer.aspx") + "?portalid=" + PortalId + "&moduleid=" + ModuleId + "&attachid=" + ai.AttachID + " border=0 class=\"afimg\" />";
                        }

                        sInsert += sClose;
                        if (ForumInfo.AllowHTML && ForumInfo.EditorType != EditorTypes.TEXTBOX)
                        {
                            insertHTML = "<a href=\"javascript:amaf_insertHTML('" + Server.HtmlEncode(sInsert) + "');amaf_toggleInline(" + ai.AttachID + "," + ai.UserID + ",0);\"><img border=\"0\" src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/image_insert.png") + "\" style=\"cursor:pointer;\" alt=\"[RESX:InsertImage]\" /></a>";
                        }
                        else
                        {
                            if (ai.ParentAttachId > 0)
                            {
                                insertHTML = "<a href=\"javascript:amaf_insertHTML('[THUMBNAIL:" + ai.AttachID + ":" + ai.ParentAttachId + "]');amaf_toggleInline(" + ai.AttachID + "," + ai.UserID + ",0);\"><img border=\"0\" src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/image_insert.png") + "\" style=\"cursor:pointer;\" alt=\"[RESX:InsertImage]\" /></a>";
                            }
                            else
                            {
                                insertHTML = "<a href=\"javascript:amaf_insertHTML('[IMAGE:" + ai.AttachID + "]');amaf_toggleInline(" + ai.AttachID + "," + ai.UserID + ",0);\"><img border=\"0\" src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/image_insert.png") + "\" style=\"cursor:pointer;\" alt=\"[RESX:InsertImage]\" /></a>";
                            }

                        }
                        insertThumb = "<a href=\"javascript:amaf_insertThumbnail(" + ai.AttachID + "," + ai.UserID + "," + w + "," + h + ");\"><img src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/image_thumb.png") + "\" style=\"cursor:pointer;\" alt=\"[RESX:CreateThumbnail]\" border=\"0\" /></a>";
                        if (ai.AllowDownload)
                        {
                            toggleInline = "<a href=\"javascript:amaf_toggleInline(" + ai.AttachID + "," + ai.UserID + ",1);\"><img border=\"0\" src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/checkbox.png") + "\" style=\"cursor:pointer;\" alt=\"[RESX:AllowDownload]\" /></a>";
                        }
                        else
                        {
                            toggleInline = "<a href=\"javascript:amaf_toggleInline(" + ai.AttachID + "," + ai.UserID + ",1);\"><img border=\"0\" src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/checkbox_unchecked.png") + "\" style=\"cursor:pointer;\" alt=\"[RESX:AllowDownload]\" /></a>";
                        }
                    }
                    else
                    {
                        toggleInline = "<a href=\"javascript:amaf_addAttach(" + ai.AttachID + "," + ai.UserID + ");\"><img border=\"0\" src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/add.png") + "\" style=\"cursor:pointer;\" alt=\"[RESX:AddAttach]\" /></a>";
                    }
                }
                else
                {
                    if (mode == 1)
                    {
                        toggleInline = "<a href=\"javascript:amaf_addAttach(" + ai.AttachID + "," + ai.UserID + ");\"><img border=\"0\" src=\"" + Page.ResolveUrl("~/DesktopModules/ActiveForums/Images/add.png") + "\" style=\"cursor:pointer;\" alt=\"[RESX:AddAttach]\" /></a>";
                    }
                }
                string rClass = "afrow";
                if (i % 2 == 0)
                {
                    rClass += " afhighlight";
                }
                sOut += "<tr onmouseout=\"this.className='" + rClass + "';\" class=\"" + rClass + "\"><td>" + ai.Filename + "</td><td>" + Utilities.FormatFileSize(ai.FileSize) + "</td><td align=\"center\">" + insertThumb + "</td><td align=\"center\">" + insertHTML + "</td><td align=\"center\">" + toggleInline + "</td><td align=\"center\">" + deleteAttach + "</td></tr>";
                i += 1;
            }
            string tbl = string.Empty;
            string createThumbHD = string.Empty;
            string insertImageHD = string.Empty;
            string downloadHD = string.Empty;
            if (sOut != "")
            {

                if (mode == 0)
                {
                    createThumbHD = "[RESX:CreateThumbnail]";
                    insertImageHD = "[RESX:InsertImage]";
                    downloadHD = "[RESX:AllowDownload]";
                }
                else
                {
                    downloadHD = "[RESX:AddAttach]";
                }
                tbl = "<table cellpadding=\"0\" cellspacing=\"0\" width=\"95%\"><tr><td class=\"afattachhead\">[RESX:FileName]</td><td class=\"afattachhead\">[RESX:FileSize]</td><td class=\"afattachhead\" align=\"center\">" + createThumbHD + "</td><td class=\"afattachhead\" align=\"center\">" + insertImageHD + "</td><td class=\"afattachhead\" align=\"center\">" + downloadHD;
                tbl += "</td><td class=\"afattachhead\" align=\"center\">[RESX:Delete]</td></tr>" + sOut + "</table>";
            }
            return tbl;
        }

        private void BindAttach(string AttachIds)
        {
            if (UserId > 0)
            {
                string sOut = string.Empty;
                Literal lit = new Literal();

                if (AttachIds == string.Empty)
                {
                    Data.AttachController adb = new Data.AttachController();
                    AttachIds = adb.GetAttachIds(AuthorId, ContentId);
                }


                sOut = BuildAttachGrid(AttachIds);
                if (PendingAttach >= ForumInfo.AttachCount)
                {
                    sOut += "<script type=\"text/javascript\">disableUpload();</script>";
                }
                else if (cbAttach.IsCallback)
                {
                    sOut += "<script type=\"text/javascript\">enableUpload();</script>";
                }
                if (!(AttachIds == string.Empty))
                {
                    sOut += "<script type=\"text/javascript\">window.amaf_setAttachIds('" + AttachIds + "');</script>";
                }
                lit.Text = sOut;
                plhAttach.Controls.Add(lit);

            }
        }
        private void BindMyFiles()
        {
            if (AuthorId > 0)
            {
                string sOut = string.Empty;
                Literal lit = new Literal();
                sOut = BuildAttachGrid(ForumUser.UserId, string.Empty, 0, 300, "Filename", "ASC");
                sOut += "<script type=\"text/javascript\">window.amaf_myfileCount('" + PendingAttach + "');</script>";
                lit.Text = sOut;
                plhMyFiles.Controls.Add(lit);
            }
        }

        #endregion

    }
}
