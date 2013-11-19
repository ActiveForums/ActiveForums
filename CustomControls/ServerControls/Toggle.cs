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

using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [ParseChildren(true, ""), ToolboxData("<{0}:toggle runat=server></{0}:toggle>")]
    public class Toggle : WebControl
    {
        private string _key;
        private string _imagePath;
        private string _cssClassOn;
        private string _cssClassOff;
        private int _toggleBehavior;
        private bool _isVisible = true;
        public int ToggleBehavior
        {
            get
            {
                return _toggleBehavior;
            }
            set
            {
                _toggleBehavior = value;
            }
        }
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                _imagePath = value;
            }
        }
        public string CssClassOn
        {
            get
            {
                return _cssClassOn;
            }
            set
            {
                _cssClassOn = value;
            }
        }
        public string CssClassOff
        {
            get
            {
                return _cssClassOff;
            }
            set
            {
                _cssClassOff = value;
            }
        }
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
            }
        }
        protected override void Render(HtmlTextWriter writer)
        {
            if (IsVisible)
            {
                writer.Write("<div id=\"imgGroup" + Key + "\" class=\"" + CssClassOn + "\" onclick=\"toggleGroup('" + Key + "','" + CssClassOn + "','" + CssClassOff + "');\"></div>");
            }
            else
            {
                writer.Write("<div id=\"imgGroup" + Key + "\" class=\"" + CssClassOff + "\" onclick=\"toggleGroup('" + Key + "','" + CssClassOn + "','" + CssClassOff + "');\"></div>");
            }
        }

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            EnableViewState = false;
        }
    }
    [ParseChildren(true, ""), ToolboxData("<{0}:toggledisplay runat=server></{0}:toggledisplay>")]
    public class ToggleDisplay : CompositeControl
    {
        private ToggleContent _content;
        private string _key;
        private bool _isVisible = true;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                _isVisible = value;
            }
        }
        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }
        public override System.Web.UI.ControlCollection Controls
        {
            get
            {
                EnsureChildControls();
                return base.Controls;
            }
        }
        protected override void CreateChildControls()
        {
            if (Content != null)
            {
                Controls.Clear();
                this.Controls.Add(Content);
            }
        }
        [Description("Initial content to render."), DefaultValue(null, ""), Browsable(false), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
        public ToggleContent Content
        {
            get
            {
                EnsureChildControls();
                return _content;
            }
            set
            {
                _content = value;
            }
        }


        protected override void Render(HtmlTextWriter writer)
        {
            writer.Write("<div id=\"group" + Key + "\" class=\"" + CssClass + "\" style=\"display:");
            if (IsVisible)
            {
                writer.Write("block");
            }
            else
            {
                writer.Write("none");
            }
            writer.Write(";\">");
            try
            {
                Content.RenderControl(writer);
            }
            catch (Exception ex)
            {

            }

            writer.Write("</div>");

            //  writer.Write(Text)
        }

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            if (Context == null || Page == null)
            {
                return;
            }
            if (Content != null)
            {
                this.Controls.Add(Content);
            }
            //EnableViewState = False
        }
    }
    [ToolboxItem(false)]
    public class ToggleContent : Control
    {

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);

            //EnableViewState = False
        }
    }
}
