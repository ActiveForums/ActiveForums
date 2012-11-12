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

