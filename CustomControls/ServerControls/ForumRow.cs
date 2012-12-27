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
    [ParseChildren(true, ""), ToolboxData("<{0}:ForumRow runat=server></{0}:ForumRow>")]
    public class ForumRow : CompositeControl
    {
        protected Controls.Link hypForumName = new Controls.Link();
        protected PlaceHolder plhLastPost = new PlaceHolder();
        protected Controls.Link hypLastPostSubject = new Controls.Link();
        private int _forumId;
        private string _forumIcon;
        private ForumRowControl _rowTemplate;
        private string _viewRoles;
        private string _readRoles;
        private string _userRoles;
        private bool _hidden;
        public override System.Web.UI.ControlCollection Controls
        {
            get
            {
                EnsureChildControls();
                return base.Controls;
            }
        }
        [Description("Initial content to render."), DefaultValue(null, ""), Browsable(false), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
        public ForumRowControl Content
        {
            get
            {
                EnsureChildControls();
                return _rowTemplate;
            }
            set
            {
                _rowTemplate = value;
            }
        }
        public int ForumId
        {
            get
            {
                return _forumId;
            }
            set
            {
                _forumId = value;
            }
        }
        public string ForumIcon
        {
            get
            {
                return _forumIcon;
            }
            set
            {
                _forumIcon = value;
            }
        }
        public string ReadRoles
        {
            get
            {
                return _readRoles;
            }
            set
            {
                _readRoles = value;
            }
        }
        public string ViewRoles
        {
            get
            {
                return _viewRoles;
            }
            set
            {
                _viewRoles = value;
            }
        }
        public string UserRoles
        {
            get
            {
                return _userRoles;
            }
            set
            {
                _userRoles = value;
            }
        }
        public bool Hidden
        {
            get
            {
                return _hidden;
            }
            set
            {
                _hidden = value;
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

        protected override void Render(HtmlTextWriter writer)
        {
            bool canView = Permissions.HasPerm(ViewRoles, UserRoles);
            bool canRead = Permissions.HasPerm(ReadRoles, UserRoles);

            if (Content != null)
            {
                hypForumName = (Link)(Content.FindControl("hypForumName" + ForumId));
                hypForumName.Enabled = canView;

                plhLastPost = (PlaceHolder)(Content.FindControl("plhLastPost" + ForumId));
                if (plhLastPost != null)
                {
                    plhLastPost.Visible = canView;
                }

                hypLastPostSubject = (Link)(Content.FindControl("hypLastPostSubject" + ForumId));
                if (hypLastPostSubject != null)
                {
                    hypLastPostSubject.Enabled = canView;
                }

            }
            if (canView)
            {
                Content.RenderControl(writer);
            }
            else if (Hidden == false)
            {
                Content.RenderControl(writer);
            }

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
        }
    }

    [ToolboxItem(false)]
    public class ForumRowControl : Control
    {
    }
}
