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
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
    [DefaultProperty("Text"), ValidationProperty("SelectedDate"), ToolboxData("<{0}:datepicker runat=server></{0}:datepicker>")]
    public class DatePicker : CompositeControl
    {

        #region Declarations

        private Label CalLabel;
        private Image CalImage;
        private System.Web.UI.HtmlControls.HtmlGenericControl div;
        private HiddenField labelHidden = new HiddenField();
        private string _dateFormat;
        private string _timeFormat;
        private string _selectedDate = "";
        private string _weekendstyle = "amothermonthday";
        private string _weekdaystyle = "amothermonthday";
        private string _monthstyle = "amcaltitle";
        private string _calendarstyle = "amcalendar";
        private string _selecteddaystyle = "amselectedday";
        private string _currentdaystyle = "amcurrentday";
        private string _dayheaderstyle = "amdayheader";
        private string _currentmonthdaystyle = "amcurrentmonthday";
        private string _othermonthdaystyle = "amothermonthday";
        private string _calwidth = "150";
        private string _calheight = "150";
        private string _imageUrl = "";
        private string _nullDate = "01/01/1900";
        private string _defaultTime = "08:00 AM";
        private string _selectedTime = "";
        private bool _showDateBox = true;
        private string _callbackFlag = string.Empty;
        private System.Globalization.Calendar cal;
        private System.Globalization.DateTimeFormatInfo dtFI;

        #endregion

        #region Properties

        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string Text { get; set; }

        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string SelectedDate
        {
            get
            {
                if (_selectedDate != "")
                {
                    try
                    {
                        if (Convert.ToDateTime(_selectedDate) <= Convert.ToDateTime(NullDate))
                        {
                            return "";
                        }
                        return _selectedDate;
                    }
                    catch (Exception ex)
                    {
                        return string.Empty;
                    }

                }
                return "";
            }

            set
            {
                _selectedDate = value;
            }
        }
        /// <summary>
        /// Default 1/1/1900.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string NullDate
        {
            get
            {
                return _nullDate;
            }

            set
            {
                _nullDate = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CalendarWidth
        {
            get
            {
                return _calwidth;
            }

            set
            {
                _calwidth = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CalendarHeight
        {
            get
            {
                return _calheight;
            }

            set
            {
                _calheight = value;
            }
        }
        /// <summary>
        /// Default amDayHeader.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CssDayHeaderStyle
        {
            get
            {
                return _dayheaderstyle;
            }

            set
            {
                _dayheaderstyle = value;
            }
        }
        /// <summary>
        /// Default amOtherMonthDay.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CssWeekendStyle
        {
            get
            {
                return _weekendstyle;
            }

            set
            {
                _weekendstyle = value;
            }
        }
        /// <summary>
        /// Default amOtherMonthDay.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CssWeekdayStyle
        {
            get
            {
                return _weekdaystyle;
            }

            set
            {
                _weekdaystyle = value;
            }
        }
        /// <summary>
        /// Default amCalTitle.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CssMonthStyle
        {
            get
            {
                return _monthstyle;
            }

            set
            {
                _monthstyle = value;
            }
        }
        /// <summary>
        /// Default amCalendar.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CssCalendarStyle
        {
            get
            {
                return _calendarstyle;
            }

            set
            {
                _calendarstyle = value;
            }
        }
        /// <summary>
        /// Default amSelectedDay.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CssSelectedDayStyle
        {
            get
            {
                return _selecteddaystyle;
            }

            set
            {
                _selecteddaystyle = value;
            }
        }
        /// <summary>
        /// Default amCurrentDay.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CssCurrentDayStyle
        {
            get
            {
                return _currentdaystyle;
            }

            set
            {
                _currentdaystyle = value;
            }
        }
        /// <summary>
        /// Default amCurrentMonthDay.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CssCurrentMonthDayStyle
        {
            get
            {
                return _currentmonthdaystyle;
            }

            set
            {
                _currentmonthdaystyle = value;
            }
        }
        /// <summary>
        /// Default amOtherMonthDay.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CssOtherMonthDayStyle
        {
            get
            {
                return _othermonthdaystyle;
            }

            set
            {
                _othermonthdaystyle = value;
            }
        }
        /// <summary>
        /// Default MM/dd/yyyy.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string DateFormat
        {
            get
            {
                if (_dateFormat == "")
                {
                    return "MM/dd/yyyy";
                }
                return _dateFormat;
            }

            set
            {
                _dateFormat = value;
            }
        }
        /// <summary>
        /// Default MM/dd/yyyy.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string TimeFormat
        {
            get
            {
                if (_dateFormat == "")
                {
                    return "h:nn tt";
                }
                return _timeFormat;
            }

            set
            {
                _timeFormat = value;
            }
        }
        /// <summary>
        /// URL to Calendar image if not using the built in one.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string ImageUrl
        {
            get
            {
                return _imageUrl;
            }

            set
            {
                _imageUrl = value;
            }
        }

        /// <summary>
        /// URL for the next month image if not using the built in one.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string ImgNext { get; set; }

        /// <summary>
        /// URL for the previous month image if not using the built in one.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string ImgPrev { get; set; }

        /// <summary>
        /// Option to display time in date picker
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public bool ShowTime { get; set; }

        /// <summary>
        /// Default Time.
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string DefaultTime
        {
            get
            {
                return _defaultTime;
            }

            set
            {
                _defaultTime = value;
            }
        }
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string SelectedTime
        {
            get
            {
                if (_selectedTime == "")
                {
                    if (SelectedDate == "")
                    {
                        return _defaultTime;
                    }
                    if (Convert.ToDateTime(SelectedDate).Year == 1900)
                    {
                        return string.Empty;
                    }
                    return Convert.ToDateTime(SelectedDate).ToString(TimeFormat);
                }
                return _selectedTime;
            }

            set
            {
                _selectedTime = value;
            }
        }

        /// <summary>
        /// Option to display Textbox in date picker
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public bool ShowDateBox
        {
            get
            {
                return _showDateBox;
            }

            set
            {
                _showDateBox = value;
            }
        }

        /// <summary>
        /// Option to get CallbackFlag control
        /// </summary>
        [Bindable(true), Category("Appearance"), DefaultValue(""), Localizable(true)]
        public string CallbackFlag
        {
            get
            {
                return _callbackFlag;
            }

            set
            {
                _callbackFlag = value;
            }
        }

        public bool TimeRequired { get; set; }

        public string RelatedControl { get; set; }

        public bool IsEndDate { get; set; }

        #endregion

        #region Subs
        protected override void Render(HtmlTextWriter writer)
        {
            try
            {
                DateTime tmpDate;
                try
                {
                    tmpDate = this.SelectedDate == "" ? DateTime.Now : Convert.ToDateTime(SelectedDate);
                }
                catch (Exception ex)
                {
                    tmpDate = DateTime.Now;
                }


                string temp = CssClass;
                CssClass = "";
                if (temp == "")
                {
                    temp = "ampicker";
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Cellpadding, "0");
                writer.AddAttribute(HtmlTextWriterAttribute.Cellspacing, "0");
                writer.AddAttribute(HtmlTextWriterAttribute.Width, Width.ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Table);
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                if (Text != "")
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Style, "white-space:nowrap");
                    writer.RenderBeginTag(HtmlTextWriterTag.Td);
                    writer.Write(Text);
                    writer.RenderEndTag();
                }
                writer.AddAttribute(HtmlTextWriterAttribute.Width, Width.ToString());
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.AddAttribute("class", temp);
                writer.AddAttribute("id", ClientID);
                writer.AddAttribute("name", ClientID);
                writer.AddAttribute("onblur", "return window." + ClientID + ".onblur(this);");
                writer.AddAttribute("onkeypress", "return window." + ClientID + ".onlyDateChars(event);");
                //writer.AddAttribute("onkeydown", "return window." & Me.ClientID & ".KeyPress(event);")
                //writer.AddAttribute("onclick", "return window." & Me.ClientID & ".Click(event);showalert();")
                if (Enabled == false)
                {
                    writer.AddAttribute("disabled", "disabled");
                }
                if (ShowDateBox)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();

                }
                dtFI = Thread.CurrentThread.CurrentCulture.DateTimeFormat;
                if (!(string.IsNullOrEmpty(SelectedDate)))
                {
                    DateTime dte = DateTime.Parse(SelectedDate);
                    SelectedDate = dte.ToString(dtFI.ShortDatePattern + " " + dtFI.ShortTimePattern);
                }
                writer.AddAttribute("type", "hidden");
                writer.AddAttribute("id", "hid_" + ClientID);
                writer.AddAttribute("name", "hid_" + ClientID);
                writer.AddAttribute("value", SelectedDate);
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();
                writer.AddAttribute("id", "cal_" + ClientID);
                writer.AddAttribute("style", "display:none;position:absolute;");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                if (ImageUrl == string.Empty)
                {
                    ImageUrl = Page.ClientScript.GetWebResourceUrl(GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.calendar.gif");
                }
                if (Enabled)
                {
                    writer.AddAttribute("src", ImageUrl);
                    writer.AddAttribute("onclick", "window." + ClientID + ".Toggle(event);");
                    writer.AddAttribute("id", "img_" + ClientID);
                    writer.RenderBeginTag(HtmlTextWriterTag.Img);
                    writer.RenderEndTag();
                }
                writer.RenderEndTag();
                writer.RenderEndTag();
                writer.RenderEndTag();
                var str = new StringBuilder();
                str.Append("<script type=\"text/javascript\">");

                cal = new System.Globalization.GregorianCalendar();
                if (Thread.CurrentThread.CurrentCulture != null)
                {
                    cal = Thread.CurrentThread.CurrentCulture.Calendar;
                }
                DateFormat = dtFI.ShortDatePattern;
                TimeFormat = dtFI.ShortTimePattern;
                str.Append("window." + ClientID + "=new asDatePicker('" + ClientID + "');");
                str.Append("window." + ClientID + ".Locale='" + Context.Request.UserLanguages[0].Substring(0, 2).ToUpper() + "';");
                str.Append("window." + ClientID + ".SelectedDate='" + SelectedDate + "';");
                str.Append("window." + ClientID + ".Width='" + CalendarWidth + "';");
                str.Append("window." + ClientID + ".Height='" + CalendarHeight + "';");
                str.Append("window." + ClientID + ".DateFormat='" + dtFI.ShortDatePattern + "';");
                str.Append("window." + ClientID + ".TimeFormat='" + dtFI.ShortTimePattern + "';");
                str.Append("window." + ClientID + ".Year=" + tmpDate.Year + ";");
                str.Append("window." + ClientID + ".Month=" + (tmpDate.Month - 1) + ";");
                str.Append("window." + ClientID + ".Day=" + tmpDate.Day + ";");
                str.Append("window." + ClientID + ".SelectedYear=" + tmpDate.Year + ";");
                str.Append("window." + ClientID + ".SelectedMonth=" + (tmpDate.Month - 1) + ";");
                str.Append("window." + ClientID + ".SelectedDay=" + tmpDate.Day + ";");
                str.Append("window." + ClientID + ".ShowTime=" + ShowTime.ToString().ToLower() + ";");
                str.Append("window." + ClientID + ".DefaultTime='" + DefaultTime + "';");
                str.Append("window." + ClientID + ".CallbackFlag='" + CallbackFlag + "';");
                if (!(string.IsNullOrEmpty(RelatedControl)))
                {
                    Control ctl = Parent.FindControl(RelatedControl);
                    if (ctl == null)
                    {
                        ctl = Page.FindControl(RelatedControl);
                    }
                    if (ctl == null)
                    {
                        RelatedControl = string.Empty;
                    }
                    else
                    {
                        RelatedControl = ctl.ClientID;
                    }
                }
                str.Append("window." + ClientID + ".linkedControl='" + RelatedControl + "';");
                if (IsEndDate)
                {
                    str.Append("window." + ClientID + ".isEndDate=true;");
                }
                else
                {
                    str.Append("window." + ClientID + ".isEndDate=false;");
                }

                string sTime = string.Empty;
                SelectedTime = tmpDate.ToString(TimeFormat);
                if (ShowTime)
                {
                    if (SelectedTime != "12:00 AM")
                    {
                        sTime = SelectedTime;
                    }
                    if (TimeRequired)
                    {
                        str.Append("window." + ClientID + ".RequireTime=true;");
                    }
                    else
                    {
                        str.Append("window." + ClientID + ".RequireTime=false;");
                    }
                }
                else
                {
                    str.Append("window." + ClientID + ".RequireTime=false;");
                }

                str.Append("window." + ClientID + ".SelectedTime='" + sTime + "';");
                if (string.IsNullOrEmpty(ImgNext))
                {
                    str.Append("window." + ClientID + ".ImgNext='" + Page.ClientScript.GetWebResourceUrl(GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.cal_nextMonth.gif") + "';");
                }
                else
                {
                    str.Append("window." + ClientID + ".ImgNext='" + Page.ResolveUrl(ImgNext) + "';");
                }
                if (string.IsNullOrEmpty(ImgPrev) )
                {
                    str.Append("window." + ClientID + ".ImgPrev='" + Page.ClientScript.GetWebResourceUrl(GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.cal_prevMonth.gif") + "';");
                }
                else
                {
                    str.Append("window." + ClientID + ".ImgPrev='" + Page.ResolveUrl(ImgPrev) + "';");
                }
                if (SelectedDate != "")
                {
                    try
                    {
                        if (ShowTime == false && sTime == string.Empty)
                        {
                            str.Append("window." + ClientID + ".textbox.value=new Date(" + tmpDate.Year + "," + (tmpDate.Month - 1) + "," + tmpDate.Day + ").formatDP('" + DateFormat + "','" + ClientID + "');");
                            str.Append("window." + ClientID + ".dateSel = new Date(" + tmpDate.Year + "," + (tmpDate.Month - 1) + "," + tmpDate.Day + ",0,0,0,0);");
                        }
                        else
                        {
                            str.Append("window." + ClientID + ".textbox.value=new Date(" + tmpDate.Year + "," + (tmpDate.Month - 1) + "," + tmpDate.Day + "," + tmpDate.Hour + "," + tmpDate.Minute + ",0).formatDP('" + DateFormat + " " + TimeFormat + "','" + ClientID + "');");
                            str.Append("window." + ClientID + ".dateSel = new Date(" + tmpDate.Year + "," + (tmpDate.Month - 1) + "," + tmpDate.Day + "," + tmpDate.Hour + "," + tmpDate.Minute + ",0);");
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                }
                int xMonths = cal.GetMonthsInYear(cal.GetYear(tmpDate), cal.GetEra(tmpDate));
                int currMonth = cal.GetMonth(tmpDate);
                int currYear = cal.GetYear(tmpDate);
                int currDay = cal.GetDayOfMonth(tmpDate);

                str.Append("window." + ClientID + ".MonthDays = new Array(");
                for (int i = 0; i < xMonths; i++)
                {
                    str.Append(cal.GetDaysInMonth(currYear, i + 1));
                    if (i < (xMonths - 1))
                    {
                        str.Append(",");
                    }
                }
                str.Append(");");
                str.AppendLine();

                string[] mNames = dtFI.MonthNames;
                str.Append("window." + ClientID + ".MonthNames = new Array(");
                for (int i = 0; i < xMonths; i++)
                {

                    str.Append("'" + mNames[i] + "'");
                    if (i < (xMonths - 1))
                    {
                        str.Append(",");
                    }
                }
                str.Append(");");
                str.AppendLine();
                str.Append("window." + ClientID + ".ShortMonthNames = new Array(");
                string[] mAbbr = dtFI.AbbreviatedMonthNames;
                for (int i = 0; i < xMonths; i++)
                {
                    str.Append("'" + mAbbr[i] + "'");
                    if (i < (xMonths - 1))
                    {
                        str.Append(",");
                    }
                }
                str.Append(");");
                str.AppendLine();
                str.Append("window." + ClientID + ".ShortDayNames = new Array(");
                string[] dAbbr = dtFI.AbbreviatedDayNames;
                for (int i = 0; i <= 6; i++)
                {
                    str.Append("'" + dAbbr[i] + "'");
                    if (i < 6)
                    {
                        str.Append(",");
                    }
                }
                str.Append(");");
                str.AppendLine();

                str.Append("window." + ClientID + ".Class={");
                str.Append("CssCalendarStyle:'" + CssCalendarStyle + "',");
                str.Append("CssMonthStyle:'" + CssMonthStyle + "',");
                str.Append("CssWeekendStyle:'" + CssWeekendStyle + "',");
                str.Append("CssWeekdayStyle:'" + CssWeekdayStyle + "',");
                str.Append("CssSelectedDayStyle:'" + CssSelectedDayStyle + "',");
                str.Append("CssCurrentMonthDayStyle:'" + CssCurrentMonthDayStyle + "',");
                str.Append("CssOtherMonthDayStyle:'" + CssOtherMonthDayStyle + "',");
                str.Append("CssDayHeaderStyle:'" + CssDayHeaderStyle + "',");
                str.Append("CssCurrentDayStyle:'" + CssCurrentDayStyle + "'};");
                str.Append("window." + ClientID + ".selectedDate=window." + ClientID + ".textbox.value;");
                str.Append("window." + ClientID + ".timeLabel='[RESX:Time]';");



                str.Append("</script>");
                writer.Write(str);
            }
            catch (Exception ex)
            {

            }
        }

        protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);


        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (!(Page.ClientScript.IsClientScriptIncludeRegistered("AMDatePicker")))
            {
#if DEBUG
				Page.ClientScript.RegisterClientScriptInclude("AMDatePicker", Page.ResolveUrl("~/DesktopModules/activeforums/customcontrols/resources/datepicker.js"));

#else
                Page.ClientScript.RegisterClientScriptInclude("AMDatePicker", Page.ClientScript.GetWebResourceUrl(this.GetType(), "DotNetNuke.Modules.ActiveForums.CustomControls.Resources.datepicker.js"));
#endif

            }
            try
            {
                if (Page.IsPostBack)
                {
                    SelectedDate = Context.Request.Form[ClientID];
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

    }
}
