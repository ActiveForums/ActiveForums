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
using System.Web.UI;
namespace DotNetNuke.Modules.ActiveForums.Controls
{
	[CLSCompliant(false), DefaultProperty("Text"), ToolboxData("<{0}:ImageButton runat=server></{0}:ImageButton>")]
	public class ImageButton : System.Web.UI.WebControls.WebControl, IPostBackEventHandler
	{

		public virtual string Text
		{
			get
			{
				object o = ViewState["Text"];
				if (o == null)
				{
					return string.Empty;
				}
			    return Convert.ToString(o);
			}
			set
			{
				ViewState["Text"] = value;
			}
		}

	    private int _hSpace;
		private int _vSpace;
		private string _imageAlign = "absmiddle";
		private bool _PostBack = true;
	    private string _ConfirmMessage = "";
	    private string _ValidationGroup = "";
		private string _imageLocation = "LEFT";
		private string _objectId = "";
		private string _PostBackScript;


	    [Bindable(true), Category("Appearance"), DefaultValue("")]
	    public string ImageUrl { get; set; }

	    [Bindable(true), Category("Appearance"), DefaultValue("")]
	    public string NavigateUrl { get; set; }

	    [Bindable(true), Category("Appearance"), DefaultValue("absmiddle")]
		public string ImageAlign
		{
			get
			{
				return _imageAlign;
			}

			set
			{
				_imageAlign = value;
			}
		}
		[Bindable(true), Category("Appearance"), DefaultValue("")]
		public string HSpace
		{
			get
			{
				return _hSpace.ToString();
			}

			set
			{
				_hSpace = Convert.ToInt32(value);
			}
		}
		[Bindable(true), Category("Appearance"), DefaultValue("")]
		public string VSpace
		{
			get
			{
				return _vSpace.ToString();
			}

			set
			{
				_vSpace = Convert.ToInt32(value);
			}
		}
		[Bindable(true), Category("Appearance"), DefaultValue("")]
		public bool PostBack
		{
			get
			{
				return _PostBack;
			}

			set
			{
				_PostBack = value;
			}
		}

	    [Bindable(true), Category("Appearance"), DefaultValue("")]
	    public string ClientSideScript { get; set; }

	    [Bindable(true), Category("Appearance"), DefaultValue("")]
	    public string Params { get; set; }

	    [Bindable(true), Category("Appearance"), DefaultValue("")]
	    public bool Confirm { get; set; }

	    [Bindable(true), Category("Appearance"), DefaultValue("")]
		public string ConfirmMessage
		{
			get
			{
				return _ConfirmMessage;
			}

			set
			{
				_ConfirmMessage = value;
			}
		}

	    [Bindable(true), Category("Appearance"), DefaultValue("")]
	    public bool EnableClientValidation { get; set; }

	    [Bindable(true), Category("Appearance"), DefaultValue("")]
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

		[Bindable(true), Category("Appearance"), DefaultValue("")]
		public string ImageLocation
		{
			get
			{
				return _imageLocation;
			}

			set
			{
				_imageLocation = value;
			}
		}
		[Bindable(true), Category("Appearance"), DefaultValue("")]
		public string ObjectId
		{
			get
			{
				return _objectId;
			}

			set
			{
				_objectId = value;
			}
		}
		[Bindable(true), Category("Appearance"), DefaultValue("")]
		public string PostBackScript
		{
			get
			{
				return _PostBackScript;
			}

		}

		// Defines the Click event.
		//
		public event EventHandler Click;

		// Invokes delegates registered with the Click event.
		//
		protected virtual void OnClick(EventArgs e)
		{
			if (Click != null)
				Click(this, e);
		}
		public void RaisePostBackEvent(string eventArgument)
		{
			OnClick(new EventArgs());
		}

		protected override void Render(HtmlTextWriter writer)
		{
			string sConfirm = "";
			string sOnClick;
			if (CssClass == "")
			{
				CssClass = "amtoolbaritem";
			}
			var outerWriter = new HtmlTextWriter(writer);
			string sVoid = "javascript:void(0);";
			string sStatusOver = "";
			string sStatusOff = "";
			if (Confirm)
			{
				sConfirm = "if (confirm('" + ConfirmMessage + "')){ [FUNCTIONS] };";
			}
			sOnClick = ClientSideScript;
			if (Attributes["onclick"] != null)
			{
				sOnClick += Attributes["onclick"];
			}
			if (sConfirm != "")
			{
				sOnClick = sConfirm.Replace("[FUNCTIONS]", sOnClick);
			}
			if (EnableClientValidation)
			{
				sOnClick = "if (typeof(Page_ClientValidate) == 'function'){ if (Page_ClientValidate('" + ValidationGroup + "')){" + sOnClick + "};};";
			}
			string sPostBack = Page.ClientScript.GetPostBackEventReference(this, string.Empty);
			_PostBackScript = sPostBack;
			if (Enabled)
			{
				if (PostBack)
				{
					if (!string.IsNullOrEmpty(sConfirm))
					{
						sPostBack = sConfirm.Replace("[FUNCTIONS]", sPostBack);
					}
					if (EnableClientValidation)
					{
						sPostBack = "if (typeof(Page_ClientValidate) == 'function'){ if (Page_ClientValidate('" + ValidationGroup + "')){" + sPostBack + "};};";
					}
                    if (!string.IsNullOrEmpty(ClientSideScript))
					{
						sPostBack = ClientSideScript + sPostBack;
					}
					outerWriter.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + sPostBack);
				}
                else if (!string.IsNullOrEmpty(NavigateUrl))
				{
					outerWriter.AddAttribute(HtmlTextWriterAttribute.Href, Page.ResolveUrl(NavigateUrl));
				}

                if (!string.IsNullOrEmpty(sOnClick))
				{
                    if (!string.IsNullOrEmpty(ObjectId))
					{
						outerWriter.AddAttribute(HtmlTextWriterAttribute.Id, ObjectId);
					}
					outerWriter.AddAttribute(HtmlTextWriterAttribute.Href, sVoid);
					outerWriter.AddAttribute(HtmlTextWriterAttribute.Onclick, sOnClick);
				}
                if (!string.IsNullOrEmpty(NavigateUrl))
				{
					outerWriter.RenderBeginTag(HtmlTextWriterTag.A);
				}

			}
            if ((!string.IsNullOrEmpty(sOnClick) || !string.IsNullOrEmpty(sPostBack)) && string.IsNullOrEmpty(NavigateUrl))
			{
                if (!string.IsNullOrEmpty(ObjectId))
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Id, ObjectId);
				}
				if (PostBack && Enabled)
				{
					writer.AddAttribute("onclick", sPostBack);
				}
                else if (!string.IsNullOrEmpty(sOnClick) && Enabled)
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Onclick, sOnClick);
				}
			}
			if (!Width.IsEmpty)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Width, Width.ToString());
			}

			if (!Height.IsEmpty)
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, Height.ToString());
			}
            else if (string.IsNullOrEmpty(CssClass))
			{
				writer.AddStyleAttribute(HtmlTextWriterStyle.Height, "22px");
			}
            if (!string.IsNullOrEmpty(CssClass))
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
			}
			if (Enabled)
			{
				writer.AddAttribute("onmouseover", "this.className='" + CssClass + "_over';");
				writer.AddAttribute("onmouseout", "this.className='" + CssClass + "';");
			}

			writer.RenderBeginTag(HtmlTextWriterTag.Div);

			if (ImageLocation.ToUpper() == "LEFT")
			{
				var innerWriter = new HtmlTextWriter(writer);
				if (ImageUrl != "")
				{
					var imageWriter = new HtmlTextWriter(innerWriter);
					imageWriter.AddAttribute(HtmlTextWriterAttribute.Src, Page.ResolveUrl(ImageUrl));
					imageWriter.AddAttribute("hspace", HSpace);
					imageWriter.AddAttribute("vspace", VSpace);
					imageWriter.AddAttribute("border", "0");
					//If [PostBack] Then

					//    imageWriter.AddAttribute("onclick", sPostBack)
					//End If
					imageWriter.AddAttribute("align", ImageAlign);
					imageWriter.RenderBeginTag(HtmlTextWriterTag.Img);
					imageWriter.RenderEndTag();
				}

				innerWriter.Write("<span>" + Text + "</span>");
			}
			else if (ImageLocation.ToUpper() == "RIGHT")
			{
				var innerWriter = new HtmlTextWriter(writer);
				innerWriter.Write("<span>" + Text + "</span>");
				if (ImageUrl != "")
				{
					var imageWriter = new HtmlTextWriter(innerWriter);
					imageWriter.AddAttribute(HtmlTextWriterAttribute.Src, Page.ResolveUrl(ImageUrl));
					imageWriter.AddAttribute("hspace", HSpace);
					imageWriter.AddAttribute("vspace", VSpace);
					imageWriter.AddAttribute("border", "0");
					//If [PostBack] Then
					//    imageWriter.AddAttribute("onclick", sPostBack)
					//End If
					imageWriter.AddAttribute("align", ImageAlign);
					imageWriter.RenderBeginTag(HtmlTextWriterTag.Img);
					imageWriter.RenderEndTag();
				}
			}
			else if (ImageLocation.ToUpper() == "TOP")
			{
				var innerWriter = new HtmlTextWriter(writer);
				if (ImageUrl != "")
				{
					var imageWriter = new HtmlTextWriter(innerWriter);
					imageWriter.AddAttribute(HtmlTextWriterAttribute.Src, Page.ResolveUrl(ImageUrl));
					imageWriter.AddAttribute("hspace", HSpace);
					imageWriter.AddAttribute("vspace", VSpace);
					imageWriter.AddAttribute("border", "0");
					//If [PostBack] Then
					//    imageWriter.AddAttribute("onclick", sPostBack)
					//End If
					imageWriter.AddAttribute("align", ImageAlign);
					imageWriter.RenderBeginTag(HtmlTextWriterTag.Img);
					imageWriter.RenderEndTag();
				}
				innerWriter.Write("<br />");
				innerWriter.Write("<span>" + Text + "</span>");

			}

			//innerWriter.RenderEndTag()
			writer.RenderEndTag();
			if (!string.IsNullOrEmpty(NavigateUrl))
			{
				if (Enabled)
				{
					outerWriter.RenderEndTag();
				}
			}


		}


	}
}