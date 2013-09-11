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

using System.Web;
using System.Text;

namespace DotNetNuke.Modules.ActiveForums.Handlers
{
	public class HandlerBase : System.Web.IHttpHandler
	{
		internal enum OutputCodes: int
		{
			Success,
			UnsupportedRequest,
			AuthenticationFailed,
			Exception,
			NoResults,
			AccessDenied
		}
		private User _userProfile;
		private Hashtable _params;
		private bool _isValid = false;
		private int _gid = -1;
		private int _groupid = -1;
		private int _upid = -1;
		private DotNetNuke.Entities.Portals.PortalSettings _ps;
		private SettingsInfo _mainSettings;
		private bool _AdminRequired = false;
		public bool AdminRequired
		{
			get
			{
				return _AdminRequired;
			}
			set
			{
				_AdminRequired = value;
			}
		}
		private int _pid = -1;
		private int _mid = -1;
		private int _UserId = -1;
		public int UserId
		{
			get
			{
				return _UserId;
			}
			set
			{
				_UserId = value;
			}
		}
		public int PortalId
		{
			get
			{
				if (HttpContext.Current.Request.QueryString["PortalId"] != null && SimulateIsNumeric.IsNumeric(HttpContext.Current.Request.QueryString["PortalId"]))
				{
					return int.Parse(HttpContext.Current.Request.QueryString["PortalId"]);
				}
				else
				{
					return _pid;
				}
			}
		}
		public int ModuleId
		{
			get
			{
				if (HttpContext.Current.Request.QueryString["ModuleId"] != null && SimulateIsNumeric.IsNumeric(HttpContext.Current.Request.QueryString["ModuleId"]))
				{
					return int.Parse(HttpContext.Current.Request.QueryString["ModuleId"]);
				}
				else
				{
					return _mid;
				}
			}
		}
		public int TabId
		{
			get
			{
				if (HttpContext.Current.Request.QueryString["TabId"] != null && SimulateIsNumeric.IsNumeric(HttpContext.Current.Request.QueryString["TabId"]))
				{
					return int.Parse(HttpContext.Current.Request.QueryString["TabId"]);
				}
				else
				{
					return -1;
				}
			}
		}
		public bool IsDebug
		{
			get
			{
				if (HttpContext.Current.Request.QueryString["amdebug"] != null)
				{
					return bool.Parse(HttpContext.Current.Request.QueryString["amdebug"]);
				}
				else
				{
					return false;
				}
			}
		}
		public SettingsInfo MainSettings
		{
			get
			{
				return _mainSettings;
			}
		}
		public bool IsValid
		{
			get
			{
				return _isValid;
			}
		}
		public DotNetNuke.Entities.Portals.PortalSettings PS
		{
			get
			{
				return _ps;
			}
		}
		public int RequestOption
		{
			get
			{
				if (HttpContext.Current.Request.QueryString["opt"] != null && SimulateIsNumeric.IsNumeric(HttpContext.Current.Request.QueryString["opt"]))
				{
					return int.Parse(HttpContext.Current.Request.QueryString["opt"]);
				}
				else
				{
					return -1;
				}
			}
		}
		public Hashtable Params
		{
			get
			{
				return _params;
			}
		}

		public int UPID
		{
			get
			{
				return _upid;
			}
		}

		private bool _IsAuthenticated = false;
		public bool IsAuthenticated
		{
			get
			{
				return _IsAuthenticated;
			}
			set
			{
				_IsAuthenticated = value;
			}
		}
		private string _Username = string.Empty;
		public string Username
		{
			get
			{
				return _Username;
			}
			set
			{
				_Username = value;
			}
		}
		internal User ForumUser
		{
			get
			{
				UserController uc = new UserController();
				return uc.GetUser(PortalId, ModuleId);
			}
		}


		public virtual bool IsReusable
		{
			get
			{
				return false;
			}
		}


		public virtual void ProcessRequest(System.Web.HttpContext context)
		{
			try
			{
				if (HttpContext.Current.Items["PortalSettings"] != null)
				{
					_ps = (DotNetNuke.Entities.Portals.PortalSettings)(HttpContext.Current.Items["PortalSettings"]);
					_pid = _ps.PortalId;
				}
				else
				{
					string DomainName = null;
					DotNetNuke.Entities.Portals.PortalAliasInfo objPortalAliasInfo = null;
					string sUrl = HttpContext.Current.Request.RawUrl.Replace("http://", string.Empty).Replace("https://", string.Empty);
					objPortalAliasInfo = DotNetNuke.Entities.Portals.PortalSettings.GetPortalAliasInfo(HttpContext.Current.Request.Url.Host);
					_pid = objPortalAliasInfo.PortalID;
					_ps = DotNetNuke.Entities.Portals.PortalController.GetCurrentPortalSettings();


				}

				//Dim sc As New Social.SocialSettings
				//_mainSettings = sc.LoadSettings[_ps.PortalId]
				_mainSettings = DataCache.MainSettings(ModuleId);
				//  If context.Request.IsAuthenticated Then
				_isValid = true;
				if (AdminRequired & ! context.Request.IsAuthenticated)
				{
					_isValid = false;
					return;
				}
				if (AdminRequired && context.Request.IsAuthenticated)
				{
					//_isValid = DotNetNuke.Security.PortalSecurity.IsInRole(_ps.AdministratorRoleName)
					DotNetNuke.Entities.Modules.ModuleController objMC = new DotNetNuke.Entities.Modules.ModuleController();
					DotNetNuke.Entities.Modules.ModuleInfo objM = objMC.GetModule(ModuleId, TabId);
					string roleIds = Permissions.GetRoleIds(objM.AuthorizedEditRoles.Split(';'), PortalId);
					_isValid = Modules.ActiveForums.Permissions.HasAccess(roleIds, ForumUser.UserRoles);
				}
				else if (AdminRequired & ! context.Request.IsAuthenticated)
				{
					_isValid = false;
					return;
				}
				string p = HttpContext.Current.Request.Params["p"];
				if (! (string.IsNullOrEmpty(p)))
				{
					_params = Utilities.JSON.ConvertFromJSONAssoicativeArrayToHashTable(p);
				}

				if (context.Request.Files.Count == 0)
				{
					string jsonPost = string.Empty;
					string prop = string.Empty;
					bool propComplete = true;
					string val = string.Empty;
					string tmp = string.Empty;
					bool bObj = false;
					//Arrays
					List<string> slist = null;
					//Dim pairs As NameValueCollection = Nothing
					Hashtable pairs = null;
					Hashtable subPairs = null;

					Hashtable ht = new Hashtable();
					int idx = 0;
					string parentProp = string.Empty;
					string skip = "{}[]:," + ((char)(34)).ToString();
					using (System.IO.StreamReader sr = new System.IO.StreamReader(context.Request.InputStream, System.Text.Encoding.UTF8))
					{
						while ( ! (sr.EndOfStream))
						{
							char c = (char)(sr.Read());
							if (idx > 0 && c == '[')
							{
								c = (char)(sr.Read());
								bObj = true;
							}
							if (idx > 0 && c == '{')
							{
								if (pairs == null)
								{
									parentProp = prop;
									prop = string.Empty;
									tmp = string.Empty;
									//pairs = New NameValueCollection
									pairs = new Hashtable();
								}
								else if (subPairs == null)
								{
									string subString = c.ToString();
									while ( c != '}')
									{
										c = (char)(sr.Read());
										subString += c;
										if (c == '}')
										{
											break;
										}
									}
									subPairs = Utilities.JSON.ConvertFromJSONAssoicativeArrayToHashTable(subString);
									pairs.Add(prop, subPairs);
									prop = string.Empty;
									tmp = string.Empty;
									subPairs = null;
									c = (char)(sr.Read());
								}
							}

							if (idx > 0 && bObj == true && ! (c == '{'))
							{
								string subItem = string.Empty;
								while ( c != ']')
								{
									if (slist == null)
									{
										slist = new List<string>();
									}
									if (skip.IndexOf(c) == -1)
									{
										subItem += c;
									}

									c = (char)(sr.Read());
									if (c == ',' || c == ']')
									{
										slist.Add(subItem);
										subItem = string.Empty;
									}
									if (c == ']')
									{
										c = (char)(sr.Read());
										bObj = false;
										break;

									}
								}
							}
							if (c == ':')
							{
								prop = tmp;
								tmp = string.Empty;
							}
							if (skip.IndexOf(c) == -1)
							{
								tmp += c;
							}
							if (c == ',' || c == '}')
							{
								if (! (string.IsNullOrEmpty(tmp)))
								{
									tmp = HttpUtility.UrlDecode(tmp);
								}
								if (slist != null)
								{
									ht.Add(prop, slist);
									slist = null;
								}
								else if (pairs != null && c == ',' && ! (string.IsNullOrEmpty(prop)))
								{
									pairs.Add(prop, tmp);
								}
								else if (pairs != null && c == '}')
								{
									if (! (string.IsNullOrEmpty(tmp)))
									{
										pairs.Add(prop, tmp);
									}
									ht.Add(parentProp, pairs);
									parentProp = string.Empty;
									pairs = null;
								}
								else if (! (string.IsNullOrEmpty(prop)))
								{
									ht.Add(prop, tmp);
								}

								prop = string.Empty;
								tmp = string.Empty;
							}

							idx += 1;

						}
						if (pairs != null & ! (string.IsNullOrEmpty(parentProp)))
						{
							ht.Add(parentProp, pairs);
						}
						else if (! (string.IsNullOrEmpty(prop)) && ! (string.IsNullOrEmpty(tmp)))
						{
							ht.Add(prop, HttpUtility.UrlDecode(tmp));
						}
						else if (! (string.IsNullOrEmpty(prop)) && slist != null)
						{
							ht.Add(prop, slist);
						}

						//jsonPost = sr.ReadToEnd()
						sr.Close();
					}
					_params = ht;
					//End If
				}
				else
				{
					Hashtable ht = new Hashtable();
					foreach (string s in context.Request.Params.AllKeys)
					{
						if (! (ht.ContainsKey(s)))
						{
							ht.Add(s, context.Request.Params[s]);
						}

					}
					_params = ht;
				}

				if (HttpContext.Current.Request.IsAuthenticated)
				{
					UserId = UserController.GetUserIdByUserName(PortalId, HttpContext.Current.User.Identity.Name);
				}
				else
				{
					UserId = -1;
				}

			}
			catch (Exception ex)
			{
				_isValid = false;
				Exceptions.LogException(ex);

			}



		}
		internal string BuildOutput(string text, OutputCodes code, bool success)
		{
			return BuildOutput(text, code, success, false);
		}
		internal string BuildOutput(string text, OutputCodes code, bool success, bool resultisobject)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("[");
			sb.Append("{");
			sb.Append("\"success\":");
			if (success)
			{
				sb.Append("true,");
			}
			else
			{
				sb.Append("false,");
			}
			if (! success)
			{
				switch (code)
				{
					case OutputCodes.Exception:
						sb.Append(Utilities.JSON.Pair("error", text));
						break;
					case OutputCodes.AuthenticationFailed:
						sb.Append(Utilities.JSON.Pair("error", "Authentication Failed"));
						break;
					case OutputCodes.UnsupportedRequest:
						sb.Append(Utilities.JSON.Pair("error", "Unsupported Request"));
						break;
					case OutputCodes.NoResults:
						sb.Append(Utilities.JSON.Pair("error", "No Results"));
						break;
					case OutputCodes.AccessDenied:
						sb.Append(Utilities.JSON.Pair("error", "Access Denied"));

						break;
				}
				sb.Append(",");
			}
			if (string.IsNullOrEmpty(text))
			{
				resultisobject = true;
				text = "null";
			}
			sb.Append("\"result\":");
			if (resultisobject)
			{
				sb.Append(text);
			}
			else
			{
				sb.Append("\"" + Utilities.JSON.EscapeJsonString(text) + "\"");
			}

			sb.Append("}");
			if (IsDebug)
			{
				sb.Append(",{");
				foreach (string s in Params.Keys)
				{
					sb.Append(Utilities.JSON.Pair(s, Params[s].ToString()));
					sb.Append(",");
				}
				sb.Append(Utilities.JSON.Pair("userid", UserId.ToString()));
				sb.Append(",");
				sb.Append(Utilities.JSON.Pair("url", HttpContext.Current.Request.RawUrl.ToString()));
				sb.Append("}");
			}
			sb.Append("]");
			return sb.ToString();
		}
	}
}
