using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:RequiredFieldValidator runat=server></{0}:RequiredFieldValidator>")]
    public class RequiredFieldValidator : WebControl
    {
        #region Delcarations
        private string _Text;
        private string _ControlToValidate;
        private string _ValidationGroup;
        private string _DefaultValue;
        #endregion
        #region Properties
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                _Text = value;
            }
        }
        public string ControlToValidate
        {
            get
            {
                return _ControlToValidate;
            }
            set
            {
                _ControlToValidate = value;
            }
        }
        public string ValidationGroup
        {
            get
            {
                return _ValidationGroup;
            }
            set
            {
                _ValidationGroup = value;
            }
        }
        public string DefaultValue
        {
            get
            {
                return _DefaultValue;
            }
            set
            {
                _DefaultValue = value;
            }
        }
        #endregion
        protected override void Render(System.Web.UI.HtmlTextWriter output)
        {
            if (Enabled)
            {
                output.AddAttribute("class", CssClass);
                output.AddAttribute("id", ClientID);
                output.RenderBeginTag(HtmlTextWriterTag.Span);
                output.RenderEndTag();
                StringBuilder sb = new StringBuilder();
                sb.Append("<script>");
                sb.Append("if(!window.AMPage){window.AMPage=new AMValidator();};");
                Control ctrl = new Control();
                ctrl = Parent.FindControl(ControlToValidate);
                //Text = Text.Replace("'", "\'")
                sb.Append("AMPage.Add('" + ClientID + "','" + ctrl.ClientID + "','" + ValidationGroup + "',null,null,null,'" + Text + "','" + DefaultValue + "');");
                sb.Append("</script>");
                output.Write(sb.ToString());
            }

        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (!(Page.ClientScript.IsClientScriptIncludeRegistered("AMValidation")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMValidation", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.Validation.js"));
            }
        }
    }
}
