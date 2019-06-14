//
// Active Forums - http://activeforums.org/
// Copyright (c) 2019
// by Active Forums Community
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
using System.Web.UI.Design;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), Designer("DotNetNuke.Modules.ActiveForums.Controls.ActiveGridDesigner"), ToolboxData("<{0}:activegrid runat=server><headertemplate></headertemplate><itemtemplate></itemtemplate><footertemplate></footertemplate></{0}:activegrid>")]
    public class ActiveGrid : CompositeControl
    {

        #region Declarations
        private Literal lit;
        private System.Web.UI.HtmlControls.HtmlGenericControl div;
        protected Callback cb;
        private ItemTemplateContents _ItemTemplate;
        private ItemTemplateContents _HeaderTemplate;
        private ItemTemplateContents _FooterTemplate;
        private IDataReader _datasource;
        private DataView _dataView;
        private string _rowDelimiter = "^";
        private string _colDelimiter = "|";
        private int _PageSize = 10;
        private string _ItemStyle = "amdatarow";
        private string _AltItemStyle = "amrowalternating";
        private string _SelectedStyle = "amrowselected";
        private bool _ShowPager = true;
        private string _PagerText = "Page {0} of {1}";
        private string _ImagePath = "";
        private int _PagerPages = 10;
        private string _DefaultColumn;
        private string _DefaultParams = "";
        private Sort _DefaultSort = Sort.Descending;
        private string _CssPagerInfo = "ampagerinfo";
        private string _CssPagerItem = "amPageritem";
        private string _CssPagerItem2 = "ampagertem2";
        private string _CssPagerCurrentNumber = "ampagernumber_current";
        private string _CssPagerNumber = "ampagernumber";
        private string _SpacerImage = "";
        private string _AscImage = "";
        private string _DescImage = "";
        private bool _LoadOnRender = true;
        private string _OnComplete;
        public enum Sort : int
        {
            Ascending,
            Descending
        }
        public event ItemBoundEventHandler ItemBound;
        public delegate void ItemBoundEventHandler(object sender, ItemBoundEventArgs e);
        #endregion

        #region Properties
        /// <summary>
        /// Item Template - Must include a table row.
        /// </summary>
        [Description("Initial content to render."), DefaultValue(null, ""), Browsable(false), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public ItemTemplateContents ItemTemplate
        {
            get
            {
                return _ItemTemplate;
            }
            set
            {
                _ItemTemplate = value;
            }
        }
        /// <summary>
        /// Header Template - Must include opening table tag and first table row.
        /// </summary>
        /// , DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)
        [Description("Initial content to render."), DefaultValue(null, ""), Browsable(false), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public ItemTemplateContents HeaderTemplate
        {
            get
            {
                return _HeaderTemplate;
            }
            set
            {
                _HeaderTemplate = value;
            }
        }
        /// <summary>
        /// Footer Template - Must include closing table tag.
        /// </summary>
        /// , DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)
        [Description("Initial content to render."), DefaultValue(null, ""), Browsable(false), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty)]
        public ItemTemplateContents FooterTemplate
        {
            get
            {
                return _FooterTemplate;
            }
            set
            {
                _FooterTemplate = value;
            }
        }
        /// <summary>
        /// String delimiter to separate row data in callback.  Ensure none of your data will include this string.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string RowDelimiter
        {
            get
            {
                return _rowDelimiter;
            }

            set
            {
                _rowDelimiter = value;
            }
        }
        /// <summary>
        /// String delimiter to separate column data in callback.  Ensure none of your data will include this string.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string ColDelimiter
        {
            get
            {
                return _colDelimiter;
            }

            set
            {
                _colDelimiter = value;
            }
        }
        /// <summary>
        /// Must be an IDataReader.  First SELECT is total row count.  Second is the data we display.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public IDataReader Datasource
        {
            get
            {
                return _datasource;
            }

            set
            {
                _datasource = value;
            }
        }
        /// <summary>
        /// Number of records to show per page.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public int PageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {
                _PageSize = value;
            }
        }
        /// <summary>
        /// CSS class name added to Item Template table row.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string ItemStyle
        {
            get
            {
                return _ItemStyle;
            }

            set
            {
                _ItemStyle = value;
            }
        }
        /// <summary>
        /// CSS class name added to Item Template every other table row.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string AltItemStyle
        {
            get
            {
                return _AltItemStyle;
            }

            set
            {
                _AltItemStyle = value;
            }
        }
        /// <summary>
        /// CSS class name added to Item Template table row when moused over.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string SelectedStyle
        {
            get
            {
                return _SelectedStyle;
            }

            set
            {
                _SelectedStyle = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public bool ShowPager
        {
            get
            {
                return _ShowPager;
            }

            set
            {
                _ShowPager = value;
            }
        }
        /// <summary>
        /// Fires a callback immediately after grid is rendered.  True by default.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public bool LoadOnRender
        {
            get
            {
                return _LoadOnRender;
            }

            set
            {
                _LoadOnRender = value;
            }
        }
        /// <summary>
        /// "Page {0} of {1}".
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string PagerText
        {
            get
            {
                return _PagerText;
            }

            set
            {
                _PagerText = value;
            }
        }
        /// <summary>
        /// Path to directory for Pager Images.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string ImagePath
        {
            get
            {
                return _ImagePath;
            }

            set
            {
                _ImagePath = value;
            }
        }
        /// <summary>
        /// Number of pages.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public int PagerPages
        {
            get
            {
                return _PagerPages;
            }

            set
            {
                _PagerPages = value;
            }
        }
        /// <summary>
        /// Default Sort for first load.  Default Descending.
        /// </summary>
        public Sort DefaultSort
        {
            get
            {
                return _DefaultSort;
            }
            set
            {
                _DefaultSort = value;
            }
        }
        /// <summary>
        /// Default Column to sort by.
        /// </summary>
        public string DefaultColumn
        {
            get
            {
                return _DefaultColumn;
            }
            set
            {
                _DefaultColumn = value;
            }
        }
        /// <summary>
        /// Default Parameters to pass in to the callback on first load.
        /// </summary>
        public string DefaultParams
        {
            get
            {
                return _DefaultParams;
            }
            set
            {
                _DefaultParams = value;
            }
        }
        /// <summary>
        /// CSS class for the pager info.
        /// </summary>
        public string CssPagerInfo
        {
            get
            {
                return _CssPagerInfo;
            }
            set
            {
                _CssPagerInfo = value;
            }
        }
        /// <summary>
        /// CSS class for every page number except the first.
        /// </summary>
        public string CssPagerItem
        {
            get
            {
                return _CssPagerItem;
            }
            set
            {
                _CssPagerItem = value;
            }
        }
        /// <summary>
        /// CSS class for the first page number.
        /// </summary>
        public string CssPagerItem2
        {
            get
            {
                return _CssPagerItem2;
            }
            set
            {
                _CssPagerItem2 = value;
            }
        }
        /// <summary>
        /// CSS class for the current page.
        /// </summary>
        public string CssPagerCurrentNumber
        {
            get
            {
                return _CssPagerCurrentNumber;
            }
            set
            {
                _CssPagerCurrentNumber = value;
            }
        }
        /// <summary>
        /// CSS class for the page numbers.
        /// </summary>
        public string CssPagerNumber
        {
            get
            {
                return _CssPagerNumber;
            }
            set
            {
                _CssPagerNumber = value;
            }
        }
        /// <summary>
        /// URL to spacer image if not using the built in one.
        /// </summary>
        public string SpacerImage
        {
            get
            {
                return _SpacerImage;
            }
            set
            {
                _SpacerImage = value;
            }
        }
        /// <summary>
        /// URL to the Ascending sort image if not using the built in one.
        /// </summary>
        public string AscImage
        {
            get
            {
                return _AscImage;
            }
            set
            {
                _AscImage = value;
            }
        }
        /// <summary>
        /// URL to the Descending sort image if not using the built in one.
        /// </summary>
        public string DescImage
        {
            get
            {
                return _DescImage;
            }
            set
            {
                _DescImage = value;
            }
        }
        public string OnComplete
        {
            get
            {
                return cb.OnCallbackComplete;
            }
            set
            {
                EnsureChildControls();
                cb.OnCallbackComplete = value;
            }
        }
        #endregion

        #region Subs/Functions
        protected override void CreateChildControls()
        {
            div = new System.Web.UI.HtmlControls.HtmlGenericControl();
            div.ID = this.ClientID;
            lit = new Literal();
            lit.ID = "data_" + this.ClientID;
            cb = new Callback();
            if (HttpContext.Current.Request.Params["amagdebug"] == "true" || HttpContext.Current.Request.Params["amdebug"] == "true")
            {
                cb.Debug = true;
            }
            else
            {
                cb.Debug = false;
            }
            cb.ID = "CB_" + this.ClientID;
            cb.Attributes.Add("style", "display:none;");
            cb.OnCallbackComplete = "function(){" + this.ClientID + ".Build();}";
            cb.CallbackEvent += new Callback.CallbackEventHandler(RaiseCallback);
            ChildControlsCreated = true;
        }
        protected override void Render(HtmlTextWriter output)
        {
            try
            {
                output = new HtmlTextWriter(output, string.Empty);
                string sOutput = this.HeaderTemplate.Text + this.ItemTemplate.Text + this.FooterTemplate.Text;
                div.Controls.Add(Page.ParseControl(sOutput));
                div.RenderControl(output);
                if (this.ShowPager)
                {
                    output.AddAttribute("id", "pager_" + this.ClientID);
                    output.RenderBeginTag(HtmlTextWriterTag.Div);
                    output.RenderEndTag();
                }
                cb.RenderControl(output);
                StringBuilder str = new StringBuilder();
                str.Append("<script>");
                str.Append("window." + this.ClientID + "=new ActiveGrid('" + this.ClientID + "');");
                if (this.SpacerImage == "")
                {
                    this.SpacerImage = Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.spacer.gif");
                }
                if (this.AscImage == "")
                {
                    this.AscImage = Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.asc.gif");
                }
                if (this.DescImage == "")
                {
                    this.DescImage = Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.desc.gif");
                }
                str.Append("window." + this.ClientID + ".SpacerImage='" + this.SpacerImage + "';");
                str.Append("window." + this.ClientID + ".AscImage='" + this.AscImage + "';");
                str.Append("window." + this.ClientID + ".DescImage='" + this.DescImage + "';");
                str.Append("window." + this.ClientID + ".SetColumns();");
                if (this.ShowPager)
                {
                    str.Append("window." + this.ClientID + ".Pager=document.getElementById('pager_" + this.ClientID + "');");
                    str.Append("window." + this.ClientID + ".PagerText='" + this.PagerText + "';");
                    str.Append("window." + this.ClientID + ".PagerPages=" + this.PagerPages + ";");
                    str.Append("window." + this.ClientID + ".CssPagerInfo='" + this.CssPagerInfo + "';");
                    str.Append("window." + this.ClientID + ".CssPagerItem='" + this.CssPagerItem + "';");
                    str.Append("window." + this.ClientID + ".CssPagerItem2='" + this.CssPagerItem2 + "';");
                    str.Append("window." + this.ClientID + ".CssPagerCurrentNumber='" + this.CssPagerCurrentNumber + "';");
                    str.Append("window." + this.ClientID + ".CssPagerNumber='" + this.CssPagerNumber + "';");
                }
                str.Append("window." + this.ClientID + ".Column='" + this.DefaultColumn + "';");
                if (this.DefaultSort == Sort.Ascending)
                {
                    str.Append("window." + this.ClientID + ".Sort='ASC';");
                    str.Append("window." + this.ClientID + ".DefaultSort='ASC';");
                }
                str.Append("window." + this.ClientID + ".Params='" + this.DefaultParams + "';");
                str.Append("window." + this.ClientID + ".Width='" + this.Width.ToString() + "';");
                str.Append("window." + this.ClientID + ".ImagePath='" + Page.ResolveUrl(this.ImagePath) + "';");
                str.Append("window." + this.ClientID + ".ItemStyle='" + this.ItemStyle + "';");
                str.Append("window." + this.ClientID + ".AltItemStyle='" + this.AltItemStyle + "';");
                str.Append("window." + this.ClientID + ".SelectedStyle='" + this.SelectedStyle + "';");
                str.Append("window." + this.ClientID + ".PageSize=" + this.PageSize.ToString() + ";");
                str.Append("window." + this.ClientID + ".RowDelimiter='" + this.RowDelimiter + "';");
                str.Append("window." + this.ClientID + ".ColDelimiter='" + this.ColDelimiter + "';");
                str.Append("window." + this.ClientID + ".ShowPager=" + this.ShowPager.ToString().ToLower() + ";");
                str.Append("window." + this.ClientID + ".CB='" + cb.ClientID + "';");
                if (this.LoadOnRender == true)
                {
                    str.Append("window." + this.ClientID + ".Callback();");
                }
                str.Append("</script>");
                output.Write(str.ToString());
            }
            catch (Exception ex)
            {
            }
        }
        /// <summary>
        /// Call in a callback to refresh the grid.  Pass in e.Output as the parameter.
        /// </summary>
        public void Refresh(HtmlTextWriter data)
        {
            lit.Text = HandleText();
            lit.RenderControl(data);
        }
        private string HandleText()
        {
            ArrayList order = new ArrayList();
            string template = this.ItemTemplate.Text;
            while (template.IndexOf("##DataItem(") > -1)
            {
                int EndIndex = template.IndexOf("')##");
                int StartIndex = template.IndexOf("##DataItem('") + 12;
                order.Add(template.Substring(StartIndex, (EndIndex - StartIndex)));
                template = template.Substring(EndIndex + 4);
            }
            if (!Page.IsPostBack)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("");
                IDataReader dr = Datasource;
                dr.Read();
                int iCount = Convert.ToInt32(dr[0]);
                int pageCount = Convert.ToInt32(Math.Ceiling(iCount / (double)this.PageSize));
                dr.NextResult();
                while (dr.Read())
                {
                    ArrayList ItemArray = new ArrayList();
                    foreach (string item in order)
                    {
                        ItemArray.Add(dr[item].ToString());
                    }
                    ItemBoundEventArgs e = new ItemBoundEventArgs(ItemArray);
                    if (ItemBound != null)
                        ItemBound(this, e);
                    foreach (string item in e.Item)
                    {
                        try
                        {
                            sb.Append(item.ToString().Replace("#", string.Empty).Replace("\"", string.Empty).Replace("\\", "&#92;").Replace("|", "&#124;").Replace("^", "&#94;").Replace("'", "\\'") + this.ColDelimiter);
                        }
                        catch
                        {
                        }
                        
                    }
                    sb.Remove((sb.Length - this.ColDelimiter.Length), this.ColDelimiter.Length);
                    sb.Append(this.RowDelimiter);
                }
                dr.Close();
                return "<script>window." + this.ClientID + ".data='" + sb.ToString() + "';window." + this.ClientID + ".PageCount=" + pageCount + ";</script>";
            }
            else
            {
                return string.Empty;
            }
        }
        public delegate void CallbackEventHandler(object sender, CallBackEventArgs e);
        public event CallbackEventHandler Callback;
        public void RaiseCallback(object sender, CallBackEventArgs e) // Implements ICallbackEventHandler.RaiseCallback
        {
            OnCallback(e);
        }
        protected virtual void OnCallback(CallBackEventArgs e)
        {
            if (Callback != null)
                Callback(cb, e);
        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (!(Page.ClientScript.IsClientScriptIncludeRegistered("AMActiveGrid")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMActiveGrid", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.ActiveGrid.js"));
            }
        }
        #endregion


    }

    public class ItemBoundEventArgs : EventArgs
    {
        public ArrayList Item;
        internal ItemBoundEventArgs(ArrayList sParam)
        {
            Item = sParam;
        }
    }

    [ToolboxItem(false)]
    public class ItemTemplateContents : System.Web.UI.Control
    {
        [DefaultValue(""), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Text
        {
            get
            {
                if (this.Controls.Count > 0 && this.Controls[0] is LiteralControl)
                {
                    return ((LiteralControl)(this.Controls[0])).Text;
                }
                return "";
            }
            set
            {
                this.Controls.Clear();
                this.Controls.Add(new LiteralControl(value));
            }
        }
    }
    public class ActiveGridDesigner : ControlDesigner
    {

        public override string GetDesignTimeHtml()
        {
            return "<table cellpadding=\"0\" cellspacing=\"0\" width=\"100%\"><tr><td>Active Forums Grid Control<br />Please use source view to make changes</td></tr></table>";
        }

    }

}
