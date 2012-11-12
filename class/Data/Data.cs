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

