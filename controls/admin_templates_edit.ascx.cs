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

namespace DotNetNuke.Modules.ActiveForums
{
	public partial class admin_templates_edit : ActiveAdminBase
	{
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cbAction.CallbackEvent += cbAction_Callback;
        }

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			Utilities.BindEnum(drpTemplateType, typeof(Templates.TemplateTypes), string.Empty, false, true, 0);
			drpTemplateType.Attributes.Add("onchange", "toggleTextTab()");
			if (! (Params == string.Empty) && ! (Params == "undefined"))
			{
				try
				{
					LoadForm(Convert.ToInt32(Params));
				}
				catch (Exception ex)
				{

				}
			}
			else
			{
				btnDelete.Visible = false;
			}
		}
		private void LoadForm(int TemplateId)
		{

			TemplateInfo ti = null;
			TemplateController tc = new TemplateController();
			ti = tc.Template_Get(TemplateId, PortalId, ModuleId);
			if (ti != null)
			{
				txtTitle.Text = ti.Title;
				txtSubject.Text = ti.Subject;
				txtPlainText.Text = ti.TemplateText;
				txtEditor.Text = Server.HtmlDecode(ti.TemplateHTML.Replace("[RESX:", "[TRESX:"));
				drpTemplateType.SelectedIndex = drpTemplateType.Items.IndexOf(drpTemplateType.Items.FindByValue(Convert.ToString(Convert.ToInt32(Enum.Parse(typeof(Templates.TemplateTypes), ti.TemplateType.ToString())))));
				hidTemplateId.Value = Convert.ToString(ti.TemplateId);
				if (ti.IsSystem)
				{
					btnDelete.Visible = false;
					txtTitle.ReadOnly = true;
					drpTemplateType.Enabled = false;
				}
			}
		}

		private void cbAction_Callback(object sender, Controls.CallBackEventArgs e)
		{
			string sMsg = "";
			switch (e.Parameters[0].ToLower())
			{
				case "save":
				{
					try
					{
						//save template
						TemplateInfo ti = null;
						TemplateController tc = new TemplateController();
						int templateId = 0;
						if (e.Parameters[1].ToString() != "")
						{
							templateId = Convert.ToInt32(e.Parameters[1]);
							ti = tc.Template_Get(templateId, PortalId, ModuleId);
						}
						else
						{
							ti = new TemplateInfo();
							ti.IsSystem = false;
							ti.TemplateType = (Templates.TemplateTypes)(Convert.ToInt32(e.Parameters[6]));
							ti.PortalId = PortalId;
							ti.ModuleId = ModuleId;
						}
						ti.Title = e.Parameters[2].ToString();
						ti.Subject = e.Parameters[3].ToString();
						if (ti.TemplateType == Templates.TemplateTypes.Email || ti.TemplateType == Templates.TemplateTypes.ModEmail)
						{
							ti.Template = "<template><html>" + Server.HtmlEncode(e.Parameters[4]) + "</html><plaintext>" + Utilities.StripHTMLTag(e.Parameters[5].ToString()) + "</plaintext></template>";
						}
						else
						{
							ti.Template = "<template><html>" + Server.HtmlEncode(e.Parameters[4]) + "</html><plaintext>" + string.Empty + "</plaintext></template>";
						}

						ti.Template = ti.Template.Replace("[TRESX:", "[RESX:");
						templateId = tc.Template_Save(ti);
						string ckey = ModuleId + templateId + Convert.ToString(Enum.Parse(typeof(Templates.TemplateTypes), ti.TemplateType.ToString()));
						DataCache.CacheClear(ckey);
						sMsg = "Template saved successfully!";
					}
					catch (Exception ex)
					{
						sMsg = "Error saving template.";

					}

					break;
				}
				case "delete":
				{
					try
					{
						//delete template
						TemplateInfo ti = null;
						TemplateController tc = new TemplateController();
						int templateid = 0;
						if (e.Parameters[1].ToString() != "")
						{
							templateid = Convert.ToInt32(e.Parameters[1]);
							ti = tc.Template_Get(templateid, PortalId, ModuleId);
							if (! (ti.IsSystem == true))
							{
								tc.Template_Delete(templateid, PortalId, ModuleId);
								sMsg = "Template deleted successfully!";
							}
							else
							{
								sMsg = "Enable to delete system templates";
							}
						}
					}
					catch (Exception ex)
					{
						sMsg = "Error deleting template.";
					}

					break;
				}
			}
			cbActionMessage.InnerText = sMsg;
			cbActionMessage.RenderControl(e.Output);
		}
	}
}