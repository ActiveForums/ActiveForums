using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web.UI;
using System.Reflection;

namespace DotNetNuke.Modules.ActiveForums
{
	public class Environment
	{
		public static bool UpdateBreadCrumb(ControlCollection ctrls, string ForumBread)
		{
			if (string.IsNullOrEmpty(ForumBread))
			{
				return true;
			}
			string[] bcText = ForumBread.Split('|');
			try
			{
				foreach (Control ctrl in ctrls)
				{
					if (ctrl is DotNetNuke.UI.Skins.SkinObjectBase && ctrl.TemplateControl.AppRelativeVirtualPath != null)
					{
						if (ctrl.TemplateControl.AppRelativeVirtualPath.ToLowerInvariant().Contains("breadcrumb.ascx"))
						{
							object o = ctrl.GetType().GetProperty("Separator").GetValue(ctrl, BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
							object cssObject = ctrl.GetType().GetProperty("CssClass").GetValue(ctrl, BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
							string css = "SkinObject";
							if (cssObject != null)
							{
								if (! (string.IsNullOrEmpty(cssObject.ToString())))
								{
									css = cssObject.ToString();
								}
							}
                            string sText = string.Empty;
                            if (o != null)
                            {
                                sText = o.ToString();
                            }
							
							string sBread = string.Empty;
							foreach (string s in bcText)
							{
								if (! (string.IsNullOrEmpty(s)))
								{
									var newValue = s.Replace("<a ", "<a class=\"" + css + "\"");
                                    sBread += sText + newValue;
								}
							}
							((System.Web.UI.WebControls.Label)(ctrl.FindControl("lblBreadCrumb"))).Text += sBread;
							break;
						}
					}
					if (ctrl.Controls.Count > 0)
					{
						UpdateBreadCrumb(ctrl.Controls, ForumBread);
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}
		public static bool UpdateMeta(ref DotNetNuke.Framework.CDefault bp, string Title, string Description, string Keywords)
		{
			if (bp == null)
			{
				return false;
			}
			try
			{
				if (! (string.IsNullOrEmpty(Title)))
				{
					if (! (string.IsNullOrEmpty(bp.Title)))
					{
						bp.Title = Title.Replace("[VALUE]", bp.Title);
					}
					else
					{
						bp.Title = Title.Replace("[VALUE]", string.Empty);
					}
				}
				if (! (string.IsNullOrEmpty(Description)))
				{
					Description = Description.Replace(System.Environment.NewLine, " ");
					if (! (string.IsNullOrEmpty(bp.Description)))
					{
						bp.Description = Description.Replace("[VALUE]", bp.Description);
					}
					else
					{
						bp.Description = Description.Replace("[VALUE]", string.Empty);
					}
				}
				if (! (string.IsNullOrEmpty(Keywords)))
				{
					if (! (string.IsNullOrEmpty(bp.KeyWords)))
					{
						string cKey = bp.KeyWords.Trim();
						if (cKey.StartsWith(","))
						{
							cKey = cKey.Substring(1);
						}
						else if (cKey.EndsWith(","))
						{
							cKey = cKey.Substring(0, cKey.Length - 1);
						}
						if (Keywords.StartsWith("[VALUE]"))
						{
							cKey += ",";
						}
						else if (Keywords.EndsWith("[VALUE]"))
						{
							cKey = "," + cKey;
						}
						bp.KeyWords = Keywords.Replace("[VALUE]", cKey);
					}
					else
					{
						bp.KeyWords = Keywords.Replace("[VALUE]", string.Empty);
					}
				}
			}
			catch (Exception ex)
			{
				return false;
			}

            return false;
		}
	}
}
