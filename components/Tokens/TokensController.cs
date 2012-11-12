using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public class TokensController
	{
		internal List<Token> TokensList()
		{
			return TokensList(string.Empty);
		}
		internal List<Token> TokensList(string group)
		{
			try
			{
				List<Token> li = new List<Token>();
				Token tk = null;
				System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
				string sPath = HttpContext.Current.Server.MapPath("~/DesktopModules/activeforums/config/tokens.config");
				xDoc.Load(sPath);
				if (xDoc != null)
				{
					System.Xml.XmlNode xRoot = xDoc.DocumentElement;
					string sQuery = "//tokens/token";
					if (! (group == string.Empty))
					{
						sQuery = sQuery + "[@group='" + group + "' or @group='*']";
					}
					System.Xml.XmlNodeList xNodeList = xRoot.SelectNodes(sQuery);
					if (xNodeList.Count > 0)
					{
						int i = 0;
						for (i = 0; i < xNodeList.Count; i++)
						{
							tk = new Token();
							tk.Group = xNodeList[i].Attributes["group"].Value;
							tk.TokenTag = xNodeList[i].Attributes["name"].Value;
							if (xNodeList[i].Attributes["value"] != null)
							{
								tk.TokenReplace = Utilities.HTMLDecode(xNodeList[i].Attributes["value"].Value);
							}
							else
							{
								tk.TokenReplace = Utilities.HTMLDecode(xNodeList[i].ChildNodes[0].InnerText);
							}

							li.Add(tk);
						}
					}
				}
				return li;
			}
			catch (Exception ex)
			{
				return null;
			}
		}
		internal string TokenGet(string group, string TokenName)
		{
			string sOut = string.Empty;
			List<Token> tl = TokensList(group);
			foreach (Token t in tl)
			{
				if (t.TokenTag == TokenName)
				{
					sOut = t.TokenReplace;
					break;
				}
			}
			return sOut;
		}
	}
}

