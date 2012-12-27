using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web.UI.WebControls;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_polls : ForumBase
    {
        private int _pollId = -1;
        public int PollId
        {
            get
            {
                return _pollId;
            }
            set
            {
                _pollId = value;
            }
        }

        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (TopicId > 0)
            {
                try
                {
                    Polls Polls = new Polls();
                    bool ShowResults = false;
                    if (UserId > 0)
                    {
                        if (Polls.HasVoted(TopicId, UserId))
                        {
                            ShowResults = true;
                        }
                    }
                    else
                    {
                        ShowResults = true;
                    }
                    if (ShowResults)
                    {
                        Literal lit = new Literal();
                        lit.Text = Polls.PollResults(TopicId, ImagePath);
                        this.Controls.Add(lit);
                    }
                    else
                    {
                        //Show Questions
                        ForumBase ctl = (ForumBase)(this.LoadControl("~/DesktopModules/ActiveForums/controls/af_pollvote.ascx"));
                        ctl.ModuleConfiguration = this.ModuleConfiguration;
                        ctl.ForumId = this.ForumId;
                        ctl.TopicId = this.TopicId;
                        this.Controls.Add(ctl);
                    }
                }
                catch (Exception ex)
                {

                }



            }

        }
    }
}
