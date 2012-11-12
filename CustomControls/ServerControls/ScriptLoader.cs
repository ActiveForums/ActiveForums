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
    [DefaultProperty("Text"), ToolboxData("<{0}:ScriptLoader runat=server></{0}:ScriptLoader>")]
    public class ScriptLoader : Control
    {

        #region Declarations

        private bool _TextSuggest = false;
        private bool _ActiveGrid = false;
        private bool _Callback = false;
        private bool _DatePicker = false;
        private bool _RequiredFieldValidator = false;
        private bool _NumberSpinner = false;


        #endregion

        #region Properties

        public bool TextSuggest
        {
            get
            {
                return _TextSuggest;
            }
            set
            {
                _TextSuggest = value;
            }
        }
        public bool ActiveGrid
        {
            get
            {
                return _ActiveGrid;
            }
            set
            {
                _ActiveGrid = value;
            }
        }
        public bool Callback
        {
            get
            {
                return _Callback;
            }
            set
            {
                _Callback = value;
            }
        }
        public bool DatePicker
        {
            get
            {
                return _DatePicker;
            }
            set
            {
                _DatePicker = value;
            }
        }
        public bool RequiredFieldValidator
        {
            get
            {
                return _RequiredFieldValidator;
            }
            set
            {
                _RequiredFieldValidator = value;
            }
        }

        public bool NumberSpinner
        {
            get
            {
                return _NumberSpinner;
            }
            set
            {
                _NumberSpinner = value;
            }
        }

        #endregion

        #region Subs/Functions

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (TextSuggest == true && !(Page.ClientScript.IsClientScriptIncludeRegistered("AMTextSuggest")))
            {
                //  Page.ClientScript.RegisterClientScriptInclude("AMTextSuggest", Page.ClientScript.GetWebResourceUrl(Me.GetType, "TextSuggest.js"))
            }
            if (ActiveGrid == true && !(Page.ClientScript.IsClientScriptIncludeRegistered("AMActiveGrid")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMActiveGrid", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.ActiveGrid.js"));
            }
            if (Callback == true && !(Page.ClientScript.IsClientScriptIncludeRegistered("AMCallback")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMCallback", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.cb.js"));
            }
            if (DatePicker == true && !(Page.ClientScript.IsClientScriptIncludeRegistered("AMDatePicker")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMDatePicker", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.DatePicker.js"));
            }
            if (RequiredFieldValidator == true && !(Page.ClientScript.IsClientScriptIncludeRegistered("AMValidation")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMValidation", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.Validation.js"));
            }
            if (!(Page.ClientScript.IsClientScriptIncludeRegistered("AMNumberSpinner")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMNumberSpinner", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.NumberSpinner.js"));
            }
            if (!(Page.ClientScript.IsClientScriptIncludeRegistered("AMMenu")))
            {
                Page.ClientScript.RegisterClientScriptInclude("AMMenu", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.MenuButton.js"));
            }
        }

        #endregion

    }
}
