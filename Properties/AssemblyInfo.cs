//
// Active Forums - http://activeforums.org
// Copyright (c) 2015
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

using System.Reflection;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Security;

[assembly: SecurityRules(SecurityRuleSet.Level1)]

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

// Review the values of the assembly attributes
[assembly: AllowPartiallyTrustedCallers()]
[assembly: AssemblyTitle("Active Forums")]
[assembly: AssemblyDescription("Discussion Forum Module for DotNetNuke")]
[assembly: AssemblyCompany("activeforums.org")]
[assembly: AssemblyProduct("Active Forums")]
[assembly: AssemblyCopyright("Copyright © 2004-2015 activeforums.org")]
[assembly: AssemblyTrademark("")]


//The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("582CE2BF-FEA8-4CAF-802B-C7F04FBC3482")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:

[assembly: AssemblyVersion("06.02.08")]

[assembly: AssemblyFileVersionAttribute("06.02.08")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.cb.js", "text/javascript")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.scripts.afadmin.properties.js", "text/javascript")]

[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.datepicker.js", "text/javascript")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.Validation.js", "text/javascript")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.ActiveGrid.js", "text/javascript")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.MenuButton.js", "text/javascript")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.calendar.gif", "image/gif")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.cal_nextMonth.gif", "image/gif")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.cal_prevMonth.gif", "image/gif")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.asc.gif", "image/gif")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.desc.gif", "image/gif")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.CustomControls.Resources.spacer.gif", "image/gif")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.sql.FullTextInstallPart1.sql", "text/plain")]
[assembly: WebResource("DotNetNuke.Modules.ActiveForums.sql.FullTextInstallPart2.sql", "text/plain")]

[assembly: AssemblyConfiguration("")]
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
[assembly: ComVisibleAttribute(false)]
