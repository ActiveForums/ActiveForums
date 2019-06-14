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

using System.Web.UI;
using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
	public partial class af_pollvote : ForumBase
	{
		private int PollId = -1;
		private string PollType = "S";
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            btnVote.Click += new System.EventHandler(btnVote_Click);

			if (TopicId > 0)
			{
				BindPoll();
			}
		}
		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			System.IO.StringWriter stringWriter = new System.IO.StringWriter();
			HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);
			base.Render(htmlWriter);
			string html = stringWriter.ToString();
			html = Utilities.LocalizeControl(html);
			writer.Write(html);
		}
		private void BindPoll()
		{

			try
			{
				DataSet ds = DataProvider.Instance().Poll_Get(TopicId);
				if (ds.Tables.Count > 0)
				{
					DataTable dtPoll = ds.Tables[0];
					DataTable dtOptions = ds.Tables[1];
					if (dtPoll.Rows.Count > 0)
					{
						lblQuestion.Text = dtPoll.Rows[0]["Question"].ToString();
						PollType = dtPoll.Rows[0]["PollType"].ToString();
						PollId = Convert.ToInt32(dtPoll.Rows[0]["PollId"]);
						if (PollType == "S")
						{
							rdbtnOptions.DataTextField = "OptionName";
							rdbtnOptions.DataValueField = "PollOptionsID";
							rdbtnOptions.DataSource = dtOptions;
							rdbtnOptions.DataBind();
							rdbtnOptions.Visible = true;
							cblstOptions.Visible = false;
						}
						else
						{
							cblstOptions.DataTextField = "OptionName";
							cblstOptions.DataValueField = "PollOptionsID";
							cblstOptions.DataSource = dtOptions;
							cblstOptions.DataBind();
							rdbtnOptions.Visible = false;
							cblstOptions.Visible = true;
						}


					}


				}
			}
			catch (Exception ex)
			{
			}
		}

		private void btnVote_Click(object sender, System.EventArgs e)
		{
			try
			{
				int optionId = -1;
				if (rdbtnOptions.Visible == true)
				{
					if (rdbtnOptions.SelectedIndex > -1)
					{
						optionId = Convert.ToInt32(rdbtnOptions.SelectedItem.Value);
					}
					if (PollId > 0 & optionId > 0)
					{
						DataProvider.Instance().Poll_Vote(PollId, optionId, string.Empty, Request.UserHostAddress, this.UserId);
					}
				}
				else if (cblstOptions.Visible == true)
				{
					if (cblstOptions.SelectedIndex > -1)
					{
						foreach (ListItem item in cblstOptions.Items)
						{
							if (item.Selected)
							{
								optionId = Convert.ToInt32(item.Value);
								DataProvider.Instance().Poll_Vote(PollId, optionId, string.Empty, Request.UserHostAddress, this.UserId);
							}
						}
					}
				}

				Response.Redirect(Request.RawUrl);

			}
			catch (Exception ex)
			{

			}
		}
	}
}
