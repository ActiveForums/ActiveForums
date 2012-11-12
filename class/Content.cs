//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;

namespace DotNetNuke.Modules.ActiveForums
{
#region Content Info
	public class Content
	{
#region Private Members

	    #endregion
#region Public Properties

	    public int ContentId { get; set; }

	    public string Subject { get; set; }

	    public string Summary { get; set; }

	    public string Body { get; set; }

	    public DateTime DateCreated { get; set; }

	    public DateTime DateUpdated { get; set; }

	    public int AuthorId { get; set; }

	    public string AuthorName { get; set; }

	    public bool IsDeleted { get; set; }

	    public string IPAddress { get; set; }

	    #endregion

	}
#endregion

}

