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
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Data.SqlClient;
using Microsoft.ApplicationBlocks.Data;
using System.Configuration;

namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class DataConfig
	{
#region Private Members
		internal string _connectionString;
		internal string _objectQualifier;
		internal string _databaseOwner;
		internal string dbPrefix;
		private DotNetNuke.Framework.Providers.ProviderConfiguration _providerConfiguration = DotNetNuke.Framework.Providers.ProviderConfiguration.GetProviderConfiguration("data");
#endregion
#region Constructors
		public DataConfig()
		{
			_connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString;

			_objectQualifier = ObjectQualifier;
			if (_objectQualifier != "" && _objectQualifier.EndsWith("_") == false)
			{
				_objectQualifier += "_";
			}

			_databaseOwner = DataBaseOwner;
			if (_databaseOwner != "" && _databaseOwner.EndsWith(".") == false)
			{
				_databaseOwner += ".";
			}
			dbPrefix = _databaseOwner + _objectQualifier + "activeforums_";
		}
#endregion
		public static object GetNull(object Field)
		{
			return Null.GetNull(Field, DBNull.Value);
		}
		public string ObjectQualifier
		{
			get
			{
				DotNetNuke.Framework.Providers.Provider objProvider = (DotNetNuke.Framework.Providers.Provider)(_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);
				_objectQualifier = objProvider.Attributes["objectQualifier"];
				if (_objectQualifier != "" && _objectQualifier.EndsWith("_") == false)
				{
					_objectQualifier += "_";
				}
				return _objectQualifier;
			}
			set
			{

			}
		}
		public string DataBaseOwner
		{
			get
			{
				DotNetNuke.Framework.Providers.Provider objProvider = (DotNetNuke.Framework.Providers.Provider)(_providerConfiguration.Providers[_providerConfiguration.DefaultProvider]);
				_databaseOwner = objProvider.Attributes["databaseOwner"];
				if (_databaseOwner != "" && _databaseOwner.EndsWith(".") == false)
				{
					_databaseOwner += ".";
				}
				return _databaseOwner;
			}
			set
			{

			}
		}
	}
}