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
    [DefaultProperty("Text"), ToolboxData("<{0}:MenuButton runat=server></{0}:MenuButton>")]
    public class MenuButton : WebControl
    {
        #region Declarations
        private string _Text;
        private MenuContent _Menu;
        private string _MenuCss;
        private int _MenuWidth = 100;
        private int _MenuHeight = 100;
        private string _MenuOverflow = "hidden";
        private int _OffsetLeft = 0;
        private int _OffsetTop = 16;
        private int _AnimationSteps = 5;
        private int _AnimationDelay = 20;
        public enum ExpandDirections : int
        {
            DownRight,
            DownLeft,
            UpRight,
            UpLeft
        }
        private ExpandDirections _ExpandDirection = ExpandDirections.DownRight;
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
        public MenuContent Menu
        {
            get
            {
                EnsureChildControls();
                if (_Menu == null)
                {
                    _Menu = new MenuContent();
                }
                return _Menu;
            }
            set
            {
                _Menu = value;
            }
        }
        public string MenuCss
        {
            get
            {
                return _MenuCss;
            }
            set
            {
                _MenuCss = value;
            }
        }
        public int MenuWidth
        {
            get
            {
                return _MenuWidth;
            }
            set
            {
                _MenuWidth = value;
            }
        }
        public int MenuHeight
        {
            get
            {
                return _MenuHeight;
            }
            set
            {
                _MenuHeight = value;
            }
        }
        public string MenuOverflow
        {
            get
            {
                return _MenuOverflow;
            }
            set
            {
                _MenuOverflow = value;
            }
        }
        public int AnimationSteps
        {
            get
            {
                return _AnimationSteps;
            }
            set
            {
                _AnimationSteps = value;
            }
        }
        public int AnimationDelay
        {
            get
            {
                return _AnimationDelay;
            }
            set
            {
                _AnimationDelay = value;
            }
        }
        public int OffsetLeft
        {
            get
            {
                return _OffsetLeft;
            }
            set
            {
                _OffsetLeft = value;
            }
        }
        public int OffsetTop
        {
            get
            {
                return _OffsetTop;
            }
            set
            {
                _OffsetTop = value;
            }
        }
        public ExpandDirections ExpandDirection
        {
            get
            {
                return _ExpandDirection;
            }
            set
            {
                _ExpandDirection = value;
            }
        }
        #endregion
        protected override void CreateChildControls()
        {
            //If Not Menu Is Nothing Then
            //    Controls.Clear()
            //    Controls.Add(Menu)
            //End If
        }
        protected override void Render(HtmlTextWriter output)
        {
            output.AddAttribute("class", CssClass);
            output.AddAttribute("onclick", "window." + ClientID + ".Toggle()");
            output.AddAttribute("id", ClientID);
            output.RenderBeginTag(HtmlTextWriterTag.Div);
            output.Write(Text);
            output.RenderEndTag();

            output.AddAttribute("class", MenuCss);
            output.AddStyleAttribute("position", "absolute");
            output.AddStyleAttribute("display", "none");
            output.AddStyleAttribute("overflow", MenuOverflow);
            output.AddAttribute("id", ClientID + "_div");
            output.RenderBeginTag(HtmlTextWriterTag.Div);
            if (Menu != null)
            {
                Menu.RenderControl(output);
            }
            output.RenderEndTag();

            string script = "<script type=\"text/javascript\">window." + ClientID + "=new ActiveMenuButton('" + ClientID + "'," + MenuWidth + "," + MenuHeight + "," + AnimationSteps + "," + AnimationDelay + "," + OffsetTop + "," + OffsetLeft + "," + ExpandDirection + ");</script>";
            output.Write(script);
        }

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            if (Menu != null)
            {
                Controls.Clear();
                Controls.Add(Menu);
            }
        }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (!(Page.ClientScript.IsClientScriptIncludeRegistered("AMMenu")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMMenu", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.MenuButton.js"));
            }
        }
    }

    public class MenuContent : Control
    {
    }
}