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

using System.Xml;

namespace DotNetNuke.Modules.ActiveForums
{
	public class ConfigUtils
	{
		public bool EnableRewriter(string configPath)
		{
			try
			{
				XmlDocument xDoc = new XmlDocument();
				xDoc.Load(configPath);
				if (xDoc != null)
				{
					System.Xml.XmlNode xRoot = xDoc.DocumentElement;
					System.Xml.XmlNode xNode = xRoot.SelectSingleNode("//system.webServer/modules");
					if (xNode != null)
					{
						if (xNode.Attributes["runAllManagedModulesForAllRequests"] == null)
						{
							XmlAttribute xAttrib = xDoc.CreateAttribute("runAllManagedModulesForAllRequests");
							xAttrib.Value = "true";
							xNode.Attributes.Append(xAttrib);
						}
						bool isInstalled = false;
						foreach (XmlNode n in xNode.ChildNodes)
						{
							if (n.Attributes["name"].Value == "ForumsReWriter")
							{
								isInstalled = true;
								break;
							}
						}
						if (! isInstalled)
						{
							XmlElement xNewNode = xDoc.CreateElement("add");
							XmlAttribute xAttrib = xDoc.CreateAttribute("name");
							xAttrib.Value = "ForumsReWriter";
							xNewNode.Attributes.Append(xAttrib);
							xAttrib = xDoc.CreateAttribute("type");
							xAttrib.Value = "DotNetNuke.Modules.ActiveForums.ForumsReWriter, DotNetNuke.Modules.ActiveForums";
							xNewNode.Attributes.Append(xAttrib);
							xAttrib = xDoc.CreateAttribute("preCondition");
							xAttrib.Value = "managedHandler";
							xNewNode.Attributes.Append(xAttrib);
							xNode.PrependChild(xNewNode);
							xDoc.Save(configPath);
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}


		}
		public bool DisableRewriter(string configPath)
		{
			try
			{
				XmlDocument xDoc = new XmlDocument();
				xDoc.Load(configPath);
				if (xDoc != null)
				{
					System.Xml.XmlNode xRoot = xDoc.DocumentElement;
					System.Xml.XmlNode xNode = xRoot.SelectSingleNode("//system.webServer/modules");
					if (xNode != null)
					{
						bool isInstalled = false;
						foreach (XmlNode n in xNode.ChildNodes)
						{
							if (n.Attributes["name"].Value == "ForumsReWriter")
							{
								xNode.RemoveChild(n);
								isInstalled = true;
								break;
							}
						}
						if (isInstalled)
						{
							xDoc.Save(configPath);
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}


		}
	}
}

