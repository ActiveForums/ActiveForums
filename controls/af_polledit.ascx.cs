using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
    public partial class af_polledit : ForumBase
    {
        private string _PollQuestion = "";
        private string _PollType = "";
        private string _PollOptions = "";
        public string PollQuestion
        {
            get
            {
                return _PollQuestion;
            }
            set
            {
                _PollQuestion = value;
            }
        }
        public string PollType
        {
            get
            {
                return _PollType;
            }
            set
            {
                _PollType = value;
            }
        }
        public string PollOptions
        {
            get
            {
                return _PollOptions;
            }
            set
            {
                _PollOptions = value;
            }
        }
        protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                txtPollQuestion.Text = PollQuestion;
                rdPollType.SelectedIndex = rdPollType.Items.IndexOf(rdPollType.Items.FindByValue(PollType));
                txtPollOptions.Text = PollOptions;
            }
        }
    }
}
