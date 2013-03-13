using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ToolboxData("<{0}:MenuButton runat=server></{0}:MenuButton>")]
    public class MenuButton : WebControl
    {
        public enum ExpandDirections
        {
            DownRight,
            DownLeft,
            UpRight,
            UpLeft
        }

        #region Private Member Variables

        private MenuContent _menu;

        #endregion

        #region Properties

        public MenuContent Menu
        {
            get
            {
                EnsureChildControls();
                return _menu ?? (_menu = new MenuContent());
            }
            set
            {
                _menu = value;
            }
        }

        public string Text { get; set; }

        public string MenuCss { get; set; }

        public int MenuWidth { get; set; }

        public int MenuHeight { get; set; }

        public string MenuOverflow { get; set; }

        public int AnimationSteps { get; set; }

        public int AnimationDelay { get; set; }

        public int OffsetLeft { get; set; }

        public int OffsetTop { get; set; }

        public ExpandDirections ExpandDirection { get; set; }

        #endregion

        public MenuButton()
        {
            MenuCss = null;
            MenuWidth = 100;
            MenuHeight = 100;
            MenuOverflow = "hidden";
            OffsetTop = 16;
            OffsetLeft = 0;
            AnimationSteps = 5;
            AnimationDelay = 20;
            ExpandDirection = ExpandDirections.DownRight;
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

            var script = "<script type=\"text/javascript\">window." + ClientID + "=new ActiveMenuButton('" + ClientID + "'," + MenuWidth + "," + MenuHeight + "," + AnimationSteps + "," + AnimationDelay + "," + OffsetTop + "," + OffsetLeft + "," + (int)ExpandDirection + ");</script>";
            output.Write(script);
        }

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            if (Menu == null) return;

            Controls.Clear();
            Controls.Add(Menu);
		}

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (!(Page.ClientScript.IsClientScriptIncludeRegistered("AMMenu")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMMenu", Page.ClientScript.GetWebResourceUrl(GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.MenuButton.js"));
            }
        }

        public class MenuContent : Control { }
    }


}