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
using DotNetNuke.Entities.Portals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public class ForumsReWriter : IHttpModule
	{
		private int _forumgroupId = -1;
		private int _forumId = -1;
		private int _tabId = -1;
		private int _moduleId = -1;
		private int _topicId = -1;
		private int _page = 1;
		private int _contentId = -1;
		private int _userId = -1;
		private int _archived = 0;
		private SettingsInfo _mainSettings = null;
		private int _urlType = 0; //0=default, 1= views, 2 = category, 3 = tag
		private int _otherId = -1;
		private int _categoryId = -1;
		private int _tagId = -1;
		public void Dispose()
		{

		}

		public void Init(System.Web.HttpApplication context)
		{
			context.BeginRequest += OnBeginRequest;
		}
		public void OnBeginRequest(object s, EventArgs e)
		{
			_forumId = -1;
			_tabId = -1;
			_moduleId = -1;
			_topicId = -1;
			_page = 1;
			_contentId = -1;
			_archived = 0;
			_forumgroupId = -1;
			_mainSettings = null;
			_categoryId = -1;
			_tagId = -1;

			HttpApplication app = (HttpApplication)s;
			HttpServerUtility Server = app.Server;
			HttpRequest Request = app.Request;
			HttpResponse Response = app.Response;
			string requestedPath = app.Request.Url.AbsoluteUri;
			HttpContext Context = ((HttpApplication)s).Context;
			int PortalId = -1;
			DotNetNuke.Entities.Portals.PortalAliasInfo objPortalAliasInfo = null;
			string sUrl = HttpContext.Current.Request.RawUrl.Replace("http://", string.Empty).Replace("https://", string.Empty);
			objPortalAliasInfo = PortalAliasController.Instance.GetPortalAlias(HttpContext.Current.Request.Url.Host);
			if (Request.RawUrl.ToLowerInvariant().Contains("404.aspx"))
			{
				string sEx = ".jpg,.gif,.png,.swf,.js,.css,.html,.htm,desktopmodules,portals,.ashx,.ico,.txt,.doc,.docx,.pdf,.xml,.xls,.xlsx,.ppt,.pptx,.csv,.zip,.asmx,.aspx";
				foreach (string sn in sEx.Split(','))
				{
					if (sUrl.Contains(sn))
					{
						// IO.File.AppendAllText(sPath, Request.RawUrl & "165<br />")
						return;
					}
				}
			}
			if (Request.Url.LocalPath.ToLower().Contains("scriptresource.axd") || Request.Url.LocalPath.ToLower().Contains("webresource.axd") || Request.Url.LocalPath.ToLower().Contains("viewer.aspx") || Request.Url.LocalPath.ToLower().Contains("cb.aspx") || Request.Url.LocalPath.ToLower().Contains("filesupload.aspx") || Request.Url.LocalPath.ToLower().Contains(".gif") || Request.Url.LocalPath.ToLower().Contains(".jpg") || Request.Url.LocalPath.ToLower().Contains(".css") || Request.Url.LocalPath.ToLower().Contains(".png") || Request.Url.LocalPath.ToLower().Contains(".swf") || Request.Url.LocalPath.ToLower().Contains(".htm") || Request.Url.LocalPath.ToLower().Contains(".html") || Request.Url.LocalPath.ToLower().Contains(".ashx") || Request.Url.LocalPath.ToLower().Contains(".cur") || Request.Url.LocalPath.ToLower().Contains(".ico") || Request.Url.LocalPath.ToLower().Contains(".txt") || Request.Url.LocalPath.ToLower().Contains(".pdf") || Request.Url.LocalPath.ToLower().Contains(".xml") || Request.Url.LocalPath.ToLower().Contains("/portals/") || Request.Url.LocalPath.ToLower().Contains("/desktopmodules/") || Request.Url.LocalPath.ToLower().Contains("evexport.aspx") || Request.Url.LocalPath.ToLower().Contains("signupjs.aspx") || Request.Url.LocalPath.ToLower().Contains("evsexport.aspx") || Request.Url.LocalPath.ToLower().Contains("fbcomm.aspx") || Request.Url.LocalPath.ToLower().Contains(".aspx") || Request.Url.LocalPath.ToLower().Contains(".js"))
			{
				return;
			}
			if (Request.Url.LocalPath.ToLower().Contains("install.aspx") || Request.Url.LocalPath.ToLower().Contains("installwizard.aspx") || Request.Url.LocalPath.ToLower().Contains("captcha.aspx") || Request.RawUrl.Contains("viewer.aspx") || Request.RawUrl.Contains("blank.html") || Request.RawUrl.Contains("default.htm") || Request.RawUrl.Contains("autosuggest.aspx"))
			{
				return;
			}
			PortalId = objPortalAliasInfo.PortalID;
			string searchURL = sUrl;
			searchURL = searchURL.Replace(objPortalAliasInfo.HTTPAlias, string.Empty);
			if (searchURL.Length < 2)
			{
				return;
			}
			string query = string.Empty;
			if (searchURL.Contains("?"))
			{
				query = searchURL.Substring(searchURL.IndexOf("?"));
				searchURL = searchURL.Substring(0, searchURL.IndexOf("?") - 1);
			}
			string newSearchURL = string.Empty;
			foreach (string up in searchURL.Split('/'))
			{
				if (! (string.IsNullOrEmpty(up)))
				{
					if (! (SimulateIsNumeric.IsNumeric(up)))
					{
						newSearchURL += up + "/";
					}
				}
			}
			bool canContinue = false;
			Data.Common db = new Data.Common();
			string tagName = string.Empty;
			string catName = string.Empty;
			if (newSearchURL.Contains("/category/") || newSearchURL.Contains("/tag/"))
			{
				if (newSearchURL.Contains("/category/"))
				{
					string cat = "/category/";
					int iEnd = newSearchURL.IndexOf("/", newSearchURL.IndexOf(cat) + cat.Length + 1);
					string catString = newSearchURL.Substring(newSearchURL.IndexOf(cat), iEnd - newSearchURL.IndexOf(cat));
					catName = catString.Replace(cat, string.Empty);
					catName = catName.Replace("/", string.Empty);
					newSearchURL = newSearchURL.Replace(catString, string.Empty);
				}
				if (newSearchURL.Contains("/tag/"))
				{
					string tag = "/tag/";
					int iEnd = newSearchURL.IndexOf("/", newSearchURL.IndexOf(tag) + tag.Length + 1);
					string tagString = newSearchURL.Substring(newSearchURL.IndexOf(tag), iEnd - newSearchURL.IndexOf(tag));
					tagName = tagString.Replace(tag, string.Empty);
					tagName = tagName.Replace("/", string.Empty);
					newSearchURL = newSearchURL.Replace(tagString, string.Empty);
				}
			}
			if ((sUrl.Contains("afv") && sUrl.Contains("post")) | (sUrl.Contains("afv") && sUrl.Contains("confirmaction")) | (sUrl.Contains("afv") && sUrl.Contains("sendto")) | (sUrl.Contains("afv") && sUrl.Contains("modreport")) | (sUrl.Contains("afv") && sUrl.Contains("search")) | sUrl.Contains("dnnprintmode") || sUrl.Contains("asg") || (sUrl.Contains("afv") && sUrl.Contains("modtopics")))
			{
				return;
			}
			try
			{
				using (IDataReader dr = db.URLSearch(PortalId, newSearchURL))
				{
					while (dr.Read())
					{
						_tabId = int.Parse(dr["TabID"].ToString());
						_moduleId = int.Parse(dr["ModuleId"].ToString());
						_forumgroupId = int.Parse(dr["ForumGroupId"].ToString());
						_forumId = int.Parse(dr["ForumId"].ToString());
						_topicId = int.Parse(dr["TopicId"].ToString());
						_archived = int.Parse(dr["Archived"].ToString());
						_otherId = int.Parse(dr["OtherId"].ToString());
						_urlType = int.Parse(dr["UrlType"].ToString());
						canContinue = true;
					}
					dr.Close();
				}
			}
			catch (Exception ex)
			{

			}
			if (! (string.IsNullOrEmpty(catName)))
			{
				_categoryId = db.Tag_GetIdByName(PortalId, _moduleId, catName, true);
				_otherId = _categoryId;
				_urlType = 2;
			}
			if (! (string.IsNullOrEmpty(tagName)))
			{
				_tagId = db.Tag_GetIdByName(PortalId, _moduleId, tagName, false);
				_otherId = _tagId;
				_urlType = 3;
			}

			if (_archived == 1)
			{
				sUrl = db.GetUrl(_moduleId, _forumgroupId, _forumId, _topicId, _userId, -1);
				if (! (string.IsNullOrEmpty(sUrl)))
				{
					string sHost = objPortalAliasInfo.HTTPAlias;
					if (sUrl.StartsWith("/"))
					{
						sUrl = sUrl.Substring(1);
					}
					if (! (sHost.EndsWith("/")))
					{
						sHost += "/";
					}
					sUrl = sHost + sUrl;
					if (! (sUrl.EndsWith("/")))
					{
						sUrl += "/";
					}
					if (! (sUrl.StartsWith("http")))
					{
						if (Request.IsSecureConnection)
						{
							sUrl = "https://" + sUrl;
						}
						else
						{
							sUrl = "http://" + sUrl;
						}
					}
					Response.Clear();
					Response.Status = "301 Moved Permanently";
					Response.AddHeader("Location", sUrl);
					Response.End();

				}
			}
			if (_moduleId > 0)
			{
				Entities.Modules.ModuleController objModules = new Entities.Modules.ModuleController();
				SettingsInfo objSettings = new SettingsInfo();
				objSettings.MainSettings = objModules.GetModuleSettings(_moduleId);
				_mainSettings = objSettings; // DataCache.MainSettings(_moduleId)
			}
			if (_mainSettings == null)
			{
				return;
			}
			if (! _mainSettings.URLRewriteEnabled)
			{
				return;
			}
			if (! canContinue && (Request.RawUrl.Contains(ParamKeys.TopicId) || Request.RawUrl.Contains(ParamKeys.ForumId) || Request.RawUrl.Contains(ParamKeys.GroupId)))
			{
				sUrl = HandleOldUrls(Request.RawUrl, objPortalAliasInfo.HTTPAlias);
				if (! (string.IsNullOrEmpty(sUrl)))
				{
					if (! (sUrl.StartsWith("http")))
					{
						if (Request.IsSecureConnection)
						{
							sUrl = "https://" + sUrl;
						}
						else
						{
							sUrl = "http://" + sUrl;
						}
					}
					Response.Clear();
					Response.Status = "301 Moved Permanently";
					Response.AddHeader("Location", sUrl);
					Response.End();
				}
			}
			if (! canContinue)
			{
				string topicUrl = string.Empty;
				if (newSearchURL.EndsWith("/"))
				{
					newSearchURL = newSearchURL.Substring(0, newSearchURL.Length - 1);
				}
				if (newSearchURL.Contains("/"))
				{
					topicUrl = newSearchURL.Substring(newSearchURL.LastIndexOf("/"));
				}
				topicUrl = topicUrl.Replace("/", string.Empty);
				if (! (string.IsNullOrEmpty(topicUrl)))
				{
					Data.Topics topicsDb = new Data.Topics();
					_topicId = topicsDb.TopicIdByUrl(PortalId, _moduleId, topicUrl.ToLowerInvariant());
					if (_topicId > 0)
					{
						sUrl = db.GetUrl(_moduleId, _forumgroupId, _forumId, _topicId, _userId, -1);
					}
					else
					{
						sUrl = string.Empty;
					}
					if (! (string.IsNullOrEmpty(sUrl)))
					{
						string sHost = objPortalAliasInfo.HTTPAlias;
						if (sHost.EndsWith("/") && sUrl.StartsWith("/"))
						{
							sUrl = sHost.Substring(0, sHost.Length - 1) + sUrl;
						}
						else if (! (sHost.EndsWith("/")) && ! (sUrl.StartsWith("/")))
						{
							sUrl = sHost + "/" + sUrl;
						}
						else
						{
							sUrl = sHost + sUrl;
						}
						if (sUrl.StartsWith("/"))
						{
							sUrl = sUrl.Substring(1);
						}
						if (! (sUrl.StartsWith("http")))
						{
							if (Request.IsSecureConnection)
							{
								sUrl = "https://" + sUrl;
							}
							else
							{
								sUrl = "http://" + sUrl;
							}
						}
						if (! (string.IsNullOrEmpty(sUrl)))
						{
							Response.Clear();
							Response.Status = "301 Moved Permanently";
							Response.AddHeader("Location", sUrl);
							Response.End();
						}
					}
				}

			}

			if (canContinue)
			{
				if (searchURL != newSearchURL)
				{
					string urlTail = searchURL.Replace(newSearchURL, string.Empty);
					if (urlTail.StartsWith("/"))
					{
						urlTail = urlTail.Substring(1);
					}
					if (urlTail.EndsWith("/"))
					{
						urlTail = urlTail.Substring(0, urlTail.Length - 1);
					}
					if (urlTail.Contains("/"))
					{
						urlTail = urlTail.Substring(0, urlTail.IndexOf("/") - 1);
					}
					if (SimulateIsNumeric.IsNumeric(urlTail))
					{
						_page = Convert.ToInt32(urlTail);
					}
				}

				string sPage = string.Empty;
				sPage = "&afpg=" + _page.ToString();
				string qs = string.Empty;
				if (sUrl.Contains("?"))
				{
					qs = "&" + sUrl.Substring(sUrl.IndexOf("?") + 1);
				}
				string catQS = string.Empty;
				if (_categoryId > 0)
				{
					catQS = "&act=" + _categoryId.ToString();
				}
				string sendTo = string.Empty;
				if ((_topicId > 0) || (_forumId > 0) || (_forumgroupId > 0))
				{
                    			sendTo = ResolveUrl(app.Context.Request.ApplicationPath, "~/default.aspx?tabid=" + _tabId +
		                        (_forumgroupId > 0 ? "&afg=" + _forumgroupId : string.Empty) +
                		        (_forumId > 0 ? "&aff=" + _forumId : string.Empty) +
                	        	(_topicId > 0 ? "&aft=" + _topicId : string.Empty) +
		                        sPage + qs +
                	        	((_forumgroupId > 0 || _forumId > 0) ? catQS : string.Empty));
		                }				
				else if (_urlType == 2 && _otherId > 0)
				{
					sendTo = ResolveUrl(app.Context.Request.ApplicationPath, "~/default.aspx?tabid=" + _tabId + "&act=" + _otherId + sPage + qs);
				}
				else if (_urlType == 3 && _otherId > 0)
				{
					sendTo = ResolveUrl(app.Context.Request.ApplicationPath, "~/default.aspx?tabid=" + _tabId + "&afv=grid&afgt=tags&aftg=" + _otherId + sPage + qs);
				}
				else if (_urlType == 1)
				{
					string v = string.Empty;
					switch (_otherId)
					{
						case 1:
							v = "unanswered";
							break;
						case 2:
							v = "notread";
							break;
						case 3:
							v = "mytopics";
							break;
						case 4:
							v = "activetopics";
							break;
                        case 5:
                            v = "afprofile";
                            break;
                        
					}
					sendTo = ResolveUrl(app.Context.Request.ApplicationPath, "~/default.aspx?tabid=" + _tabId + "&afv=grid&afgt=" + v + sPage + qs);
				}
				else if (_tabId > 0)
				{
					sendTo = ResolveUrl(app.Context.Request.ApplicationPath, "~/default.aspx?tabid=" + _tabId + sPage + qs);
				}

				RewriteUrl(app.Context, sendTo);
			}
		}
		internal static string ResolveUrl(string appPath, string url)
		{

			// String is Empty, just return Url
			if (url.Length == 0)
			{
				return url;
			}

			// String does not contain a ~, so just return Url
			if (url.StartsWith("~") == false)
			{
				return url;
			}

			// There is just the ~ in the Url, return the appPath
			if (url.Length == 1)
			{
				return appPath;
			}

			if (url.ToCharArray()[1] == '/' || url.ToCharArray()[1] == '\\')
			{
				// Url looks like ~/ or ~\
				if (appPath.Length > 1)
				{
					return appPath + "/" + url.Substring(2);
				}
				else
				{
					return "/" + url.Substring(2);
				}
			}
			else
			{
				// Url look like ~something
				if (appPath.Length > 1)
				{
					return appPath + "/" + url.Substring(1);
				}
				else
				{
					return appPath + url.Substring(1);
				}
			}

		}
		internal static void RewriteUrl(HttpContext context, string sendToUrl)
		{
			string x = "";
			string y = "";

			RewriteUrl(context, sendToUrl, ref x, ref y);
		}

		internal static void RewriteUrl(HttpContext context, string sendToUrl, ref string sendToUrlLessQString, ref string filePath)
		{

			// first strip the querystring, if any
			string queryString = string.Empty;
			sendToUrlLessQString = sendToUrl;

			if (sendToUrl.IndexOf("?") > 0)
			{
				sendToUrlLessQString = sendToUrl.Substring(0, sendToUrl.IndexOf("?"));
				queryString = sendToUrl.Substring(sendToUrl.IndexOf("?") + 1);
			}

			// grab the file's physical path
			filePath = string.Empty;
			filePath = context.Server.MapPath(sendToUrlLessQString);

			// rewrite the path..
			if (sendToUrlLessQString.Contains("/404.aspx?404;"))
			{
				sendToUrlLessQString = sendToUrlLessQString.Substring(sendToUrlLessQString.IndexOf("/404.aspx?404;") + 14);

			}
			context.RewritePath(sendToUrlLessQString, string.Empty, queryString);

			// NOTE!  The above RewritePath() overload is only supported in the .NET Framework 1.1
			// If you are using .NET Framework 1.0, use the below form instead:
			// context.RewritePath(sendToUrl);

		}
		private string HandleOldUrls(string rawUrl, string httpAlias)
		{
			string sUrl = string.Empty;
			string currURL = rawUrl;
			string splitter = "/";
			if (currURL.Contains("&"))
			{
				splitter = "&";
			}
			string[] parts = currURL.Split(Convert.ToChar(splitter));
			for (int i = 0; i < parts.Length; i++)
			{
				if (! (string.IsNullOrEmpty(parts[i])))
				{
					if (parts[i].ToLowerInvariant().Contains("="))
					{
						string[] pair = parts[i].Split('=');
						if (pair[0].ToLowerInvariant() == ParamKeys.ForumId)
						{
							_forumId = int.Parse(pair[1]);
						}
						else if (pair[0].ToLowerInvariant() == ParamKeys.TopicId)
						{
							_topicId = int.Parse(pair[1]);
						}
						else if (pair[0].ToLowerInvariant() == ParamKeys.PageId)
						{
							_page = int.Parse(pair[1]);
						}
						else if (pair[0].ToLowerInvariant() == ParamKeys.PageJumpId)
						{
							_page = int.Parse(pair[1]);
						}
						else if (pair[0].ToLowerInvariant() == ParamKeys.ContentJumpId)
						{
							_contentId = int.Parse(pair[1]);
						}
					}
					else
					{
						if (parts[i].ToLowerInvariant() == ParamKeys.ForumId)
						{
							_forumId = int.Parse(parts[i + 1]);
						}
						else if (parts[i].ToLowerInvariant() == ParamKeys.TopicId)
						{
							_topicId = int.Parse(parts[i + 1]);
						}
						else if (parts[i].ToLowerInvariant() == ParamKeys.PageId)
						{
							_page = int.Parse(parts[i + 1]);
						}
						else if (parts[i].ToLowerInvariant() == ParamKeys.PageJumpId)
						{
							_page = int.Parse(parts[i + 1]);
						}
						else if (parts[i].ToLowerInvariant() == "tabid")
						{
							_tabId = int.Parse(parts[i + 1]);
						}
						else if (parts[i].ToLowerInvariant() == ParamKeys.ContentJumpId)
						{
							_contentId = int.Parse(parts[i + 1]);
						}
					}
				}
			}
			Data.Common db = new Data.Common();
			sUrl = db.GetUrl(_moduleId, _forumgroupId, _forumId, _topicId, _userId, _contentId);
			if (! (string.IsNullOrEmpty(sUrl)))
			{

				string sHost = httpAlias;
				if (sUrl.StartsWith("/"))
				{
					sUrl = sUrl.Substring(1);
				}

				if (! (sHost.EndsWith("/")))
				{
					sHost += "/";
				}
				sUrl = sHost + sUrl;
				if (! (sUrl.EndsWith("/")))
				{
					sUrl += "/";
				}
				if (_contentId > 0)
				{
					if (_page > 1)
					{
						sUrl += _page + "/";
					}
				}


			}
			return sUrl;
		}
	}
}

