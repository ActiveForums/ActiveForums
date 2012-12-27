using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Xml;
using DotNetNuke.Services.Localization;
using System.IO;
using System.Web;

namespace DotNetNuke.Modules.ActiveForums
{
	public class LocalizationUtils
	{
		public bool SaveResource(string name, string resourceText, int PortalId)
		{
			XmlDocument portalResources = new XmlDocument();
			XmlDocument defaultResources = new XmlDocument();
			XmlNode parent = null;
			string filename = null;

			try
			{
				defaultResources.Load(GetResourceFile("", Localization.SystemLocale, PortalId));
				filename = GetResourceFile("Portal", Localization.SystemLocale, PortalId);
				if (File.Exists(filename))
				{
					portalResources.Load(filename);
				}
				else
				{
					portalResources.Load(GetResourceFile("", Localization.SystemLocale, PortalId));
				}

				UpdateResourceFileNode(portalResources, name, resourceText);
				//UpdateResourceFileNode(portalResources, "GroupProperties_" + prop.PropertyName + ".Help", resourceText)
				//UpdateResourceFileNode(portalResources, "GroupProperties_" + prop.PropertyCategory + ".Header", prop.PropertyCategory)

				// remove unmodified keys
				foreach (XmlNode node in portalResources.SelectNodes("//root/data"))
				{
					XmlNode defaultNode = defaultResources.SelectSingleNode("//root/data[@name='" + node.Attributes["name"].Value + "']");
					if (defaultNode != null && defaultNode.InnerXml == node.InnerXml)
					{
						parent = node.ParentNode;
						parent.RemoveChild(node);
					}
				}
				// remove duplicate keys
				foreach (XmlNode node in portalResources.SelectNodes("//root/data"))
				{
					if (portalResources.SelectNodes("//root/data[@name='" + node.Attributes["name"].Value + "']").Count > 1)
					{
						parent = node.ParentNode;
						parent.RemoveChild(node);
					}
				}

				if (portalResources.SelectNodes("//root/data").Count > 0)
				{
					// there's something to save
					portalResources.Save(filename);
				}
				else
				{
					// nothing to be saved, if file exists delete
					if (File.Exists(filename))
					{
						File.Delete(filename);
					}
				}
				return true;
			}
			catch (Exception exc) //Module failed to load
			{
				return false;
				// UI.Skins.Skin.AddModuleMessage(Me, Localization.GetString("Save.ErrorMessage", Me.LocalResourceFile), UI.Skins.Controls.ModuleMessage.ModuleMessageType.YellowWarning)
			}
		}

		public string GetResourceFile(string type, string language, int PortalId)
		{
			string resourcefilename = "~/DesktopModules/ActiveForums/app_localresources/SharedResources.resx";
			//If IO.File.Exists(HttpContext.Current.Server.MapPath("~/admin/users/app_localresources/profile.ascx.resx")) Then
			//    resourcefilename = "~/admin/users/app_localresources/profile.ascx.resx"
			//ElseIf IO.File.Exists(HttpContext.Current.Server.MapPath("~/desktopmodules/admin/security/app_localresources/profile.ascx.resx")) Then
			//    resourcefilename = "~/desktopmodules/admin/security/app_localresources/profile.ascx.resx"
			//End If
			if (language != Localization.SystemLocale)
			{
				resourcefilename = resourcefilename.Substring(0, resourcefilename.Length - 5) + "." + language + ".resx";
			}

			if (type == "Portal")
			{
				resourcefilename = resourcefilename.Substring(0, resourcefilename.Length - 5) + "." + "Portal-" + PortalId.ToString() + ".resx";
			}

			return HttpContext.Current.Server.MapPath(resourcefilename);

		}

		public void UpdateResourceFileNode(XmlDocument xmlDoc, string key, string text)
		{
			XmlNode node = null;
			XmlNode nodeData = null;
			XmlAttribute attr = null;

			node = xmlDoc.SelectSingleNode("//root/data[@name='" + key + "']/value");
			if (node == null)
			{
				// missing entry
				nodeData = xmlDoc.CreateElement("data");
				attr = xmlDoc.CreateAttribute("name");
				attr.Value = key;
				nodeData.Attributes.Append(attr);
				xmlDoc.SelectSingleNode("//root").AppendChild(nodeData);
				node = nodeData.AppendChild(xmlDoc.CreateElement("value"));
			}
			node.InnerXml = HttpContext.Current.Server.HtmlEncode(text);

		}
	}
}