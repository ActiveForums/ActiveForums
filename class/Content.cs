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

