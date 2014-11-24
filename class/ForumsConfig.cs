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
using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public class ForumsConfig
	{
		public string sPath = HttpContext.Current.Server.MapPath("~/DesktopModules/ActiveForums/config/defaultsetup.config");
		public bool ForumsInit(int PortalId, int ModuleId)
		{
			try
			{
				//Initial Settings
				LoadSettings(PortalId, ModuleId);
				//Add Default Templates
				LoadTemplates(PortalId, ModuleId);
				//Add Default Status
				LoadFilters(PortalId, ModuleId);
				//Add Default Steps
				LoadRanks(PortalId, ModuleId);
				//Add Default Forums
				LoadDefaultForums(PortalId, ModuleId);
				return true;
			}
			catch (Exception ex)
			{
				Services.Exceptions.Exceptions.LogException(ex);
				return false;
			}
		}
		private void LoadSettings(int PortalId, int ModuleId)
		{
			try
			{
				var objModules = new Entities.Modules.ModuleController();
				var xDoc = new System.Xml.XmlDocument();
				xDoc.Load(sPath);
				if (xDoc != null)
				{

					System.Xml.XmlNode xRoot = xDoc.DocumentElement;
					System.Xml.XmlNodeList xNodeList = xRoot.SelectNodes("//mainsettings/setting");
					if (xNodeList.Count > 0)
					{
						int i;
						for (i = 0; i < xNodeList.Count; i++)
						{
							objModules.UpdateModuleSetting(ModuleId, xNodeList[i].Attributes["name"].Value, xNodeList[i].Attributes["value"].Value);

						}
					}
				}
				objModules.UpdateModuleSetting(ModuleId, SettingKeys.IsInstalled, "True");
				objModules.UpdateModuleSetting(ModuleId, "NeedsConvert", "False");
				try
				{
					System.Globalization.DateTimeFormatInfo nfi = new System.Globalization.CultureInfo("en-US", true).DateTimeFormat;


					objModules.UpdateModuleSetting(ModuleId, SettingKeys.InstallDate, DateTime.Now.ToString(new System.Globalization.CultureInfo("en-US")));
				}
				catch (Exception ex)
				{
					Services.Exceptions.Exceptions.LogException(ex);
				}
			}
			catch (Exception ex)
			{

			}
		}
		private void LoadTemplates(int PortalId, int ModuleId)
		{
			try
			{
				var xDoc = new System.Xml.XmlDocument();
				xDoc.Load(sPath);
				if (xDoc != null)
				{
					System.Xml.XmlNode xRoot = xDoc.DocumentElement;
					System.Xml.XmlNodeList xNodeList = xRoot.SelectNodes("//templates/template");
					if (xNodeList.Count > 0)
					{
						var tc = new TemplateController();
						int i;
						for (i = 0; i < xNodeList.Count; i++)
						{
							var ti = new TemplateInfo
							             {
							                 TemplateId = -1,
							                 TemplateType =
							                     (Templates.TemplateTypes)
							                     Enum.Parse(typeof (Templates.TemplateTypes), xNodeList[i].Attributes["templatetype"].Value),
							                 IsSystem = true,
							                 PortalId = PortalId,
							                 ModuleId = ModuleId,
							                 Title = xNodeList[i].Attributes["templatetitle"].Value,
							                 Subject = xNodeList[i].Attributes["templatesubject"].Value
							             };
						    string sHTML;
							sHTML = GetFileContent(xNodeList[i].Attributes["importfilehtml"].Value);
							string sText;
							sText = GetFileContent(xNodeList[i].Attributes["importfiletext"].Value);
							string sTemplate = sText;
							if (sHTML != string.Empty)
							{
								sTemplate = "<template><html>" + HttpContext.Current.Server.HtmlEncode(sHTML) + "</html><plaintext>" + sText + "</plaintext></template>";
							}
							ti.Template = sTemplate;
							tc.Template_Save(ti);
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}
		private void LoadFilters(int PortalId, int ModuleId)
		{
			Utilities.ImportFilter(PortalId, ModuleId);
		}
		private void LoadRanks(int PortalId, int ModuleId)
		{
			try
			{
				var xDoc = new System.Xml.XmlDocument();
				xDoc.Load(sPath);
				if (xDoc != null)
				{
					System.Xml.XmlNode xRoot = xDoc.DocumentElement;
					System.Xml.XmlNodeList xNodeList = xRoot.SelectNodes("//ranks/rank");
					if (xNodeList.Count > 0)
					{
						int i;
						for (i = 0; i < xNodeList.Count; i++)
						{
							DataProvider.Instance().Ranks_Save(PortalId, ModuleId, -1, xNodeList[i].Attributes["rankname"].Value, Convert.ToInt32(xNodeList[i].Attributes["rankmin"].Value), Convert.ToInt32(xNodeList[i].Attributes["rankmax"].Value), xNodeList[i].Attributes["rankimage"].Value);
						}
					}
				}
			}
			catch (Exception ex)
			{

			}
		}
		private void LoadDefaultForums(int PortalId, int ModuleId)
		{
			var xDoc = new System.Xml.XmlDocument();
			xDoc.Load(sPath);
			if (xDoc != null)
			{

				System.Xml.XmlNode xRoot = xDoc.DocumentElement;
				System.Xml.XmlNodeList xNodeList = xRoot.SelectNodes("//defaultforums/groups/group");
				if (xNodeList.Count > 0)
				{
					int i;
					for (i = 0; i < xNodeList.Count; i++)
					{
						var gi = new ForumGroupInfo
						             {
						                 ModuleId = ModuleId,
						                 ForumGroupId = -1,
						                 GroupName = xNodeList[i].Attributes["groupname"].Value,
						                 Active = xNodeList[i].Attributes["active"].Value == "1",
						                 Hidden = xNodeList[i].Attributes["hidden"].Value == "1",
						                 SortOrder = i,
						                 GroupSettingsKey = "",
						                 PermissionsId = -1
						             };
					    var gc = new ForumGroupController();
						int groupId;
						groupId = gc.Groups_Save(PortalId, gi, true);
						gi = gc.GetForumGroup(ModuleId, groupId);
						string sKey = "G:" + groupId.ToString();
						string sAllowHTML = "false";
						if (xNodeList[i].Attributes["allowhtml"] != null)
						{
							sAllowHTML = xNodeList[i].Attributes["allowhtml"].Value;
						}
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.TopicsTemplateId, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.TopicTemplateId, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EmailAddress, string.Empty);
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.UseFilter, "true");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowPostIcon, "true");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowEmoticons, "true");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowScript, "false");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.IndexContent, "false");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowRSS, "true");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowAttach, "true");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachCount, "3");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachMaxSize, "1000");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachTypeAllowed, "txt,tiff,pdf,xls,xlsx,doc,docx,ppt,pptx");
						//Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachStore, "FILESYSTEM");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachMaxHeight, "450");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachMaxWidth, "450");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachAllowBrowseSite, "true");
                        Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.MaxAttachHeight, "800");
                        Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.MaxAttachWidth, "800");
                        Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AttachInsertAllowed, "false");
                        Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ConvertingToJpegAllowed, "false");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AllowHTML, sAllowHTML);
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorType, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorHeight, "350");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorWidth, "99%");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorToolbar, "bold,italic,underline,quote");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.EditorStyle, "2");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.TopicFormId, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ReplyFormId, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.QuickReplyFormId, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ProfileTemplateId, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.IsModerated, "false");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.DefaultTrustValue, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.AutoTrustLevel, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModApproveTemplateId, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModRejectTemplateId, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModMoveTemplateId, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModDeleteTemplateId, "0");
						Settings.SaveSetting(ModuleId, sKey, ForumSettingKeys.ModNotifyTemplateId, "0");
						if (groupId != -1)
						{
							if (xNodeList[i].HasChildNodes)
							{
								System.Xml.XmlNodeList cNodes = xNodeList[i].ChildNodes;
								for (int c = 0; c < cNodes.Count; c++)
								{
									var fi = new Forum();
									var fc = new ForumController();
									fi.ForumID = -1;
									fi.ModuleId = ModuleId;
									fi.ForumGroupId = groupId;
									fi.ParentForumId = 0;
									fi.ForumName = cNodes[c].Attributes["forumname"].Value;
									fi.ForumDesc = cNodes[c].Attributes["forumdesc"].Value;
									fi.ForumSecurityKey = "G:" + groupId.ToString();
									fi.ForumSettingsKey = "G:" + groupId.ToString();
									fi.Active = cNodes[c].Attributes["active"].Value == "1";
									fi.Hidden = cNodes[c].Attributes["hidden"].Value == "1";
									fi.SortOrder = c;
									fi.PermissionsId = gi.PermissionsId;
									fc.Forums_Save(PortalId, fi, true, true);
								}
							}
						}
					}
				}
			}
		}

		private string GetFileContent(string FilePath)
		{
			string sPath = HttpContext.Current.Server.MapPath(FilePath);
			string sContents = string.Empty;
			System.IO.StreamReader objStreamReader;
			if (System.IO.File.Exists(sPath))
			{
				try
				{
					//objStreamReader = IO.File.OpenText(sPath)
					objStreamReader = new System.IO.StreamReader(sPath, System.Text.Encoding.ASCII);
					sContents = objStreamReader.ReadToEnd();
					objStreamReader.Close();
				}
				catch (Exception exc)
				{
					sContents = exc.Message;
				}
			}
			return sContents;
		}

		public void CreateForumForGroup(int PortalId, int ModuleId, int SocialGroupId, string Config)
		{
			var xDoc = new System.Xml.XmlDocument();
			xDoc.LoadXml(Config);
		    {
		        System.Xml.XmlNode xRoot = xDoc.DocumentElement;
		        System.Xml.XmlNodeList xNodeList = xRoot.SelectNodes("//defaultforums/groups/group");
		    }
		}

	}
}

