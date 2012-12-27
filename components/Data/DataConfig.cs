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