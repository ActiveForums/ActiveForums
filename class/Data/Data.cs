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

using System.Configuration;

namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class Connection
	{
#region Private Members
		private Framework.Providers.ProviderConfiguration _providerConfiguration = Framework.Providers.ProviderConfiguration.GetProviderConfiguration("data");
		internal string connectionString;
		internal string objectQualifier;
		internal string databaseOwner;
		internal string databaseObjectPrefix = "activeforums_";
		internal string dbPrefix;
#endregion
#region Constructors
		public Connection()
		{
			connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;
			var objProvider = (Framework.Providers.Provider)(_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);

			objectQualifier = objProvider.Attributes["objectQualifier"];
			if (objectQualifier != "" && objectQualifier.EndsWith("_") == false)
			{
				objectQualifier += "_";
			}

			databaseOwner = objProvider.Attributes["databaseOwner"];
			if (databaseOwner != "" && databaseOwner.EndsWith(".") == false)
			{
				databaseOwner += ".";
			}
			dbPrefix = databaseOwner + objectQualifier + databaseObjectPrefix;
		}
#endregion
	}
}

