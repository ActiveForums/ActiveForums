using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web;
using System.Web.Services;
using System.Text;

namespace DotNetNuke.Modules.ActiveForums.Handlers
{
	public class adminhelper : HandlerBase
	{
		public enum Actions: int
		{
			None,
			PropertySave = 2,
			PropertyDelete = 3,
			PropertyList = 4,
			PropertySortSwitch = 5,
			ListOfLists = 6,
			LoadView = 7,
			RankGet = 8,
			RankSave = 9,
			RankDelete = 10,
			FilterGet = 11,
			FilterSave = 12,
			FilterDelete = 13



		}
		public override void ProcessRequest(HttpContext context)
		{
			AdminRequired = true;
			base.AdminRequired = true;
			base.ProcessRequest(context);
			string sOut = string.Empty;
			Actions action = Actions.None;
			if (IsValid)
			{
				if (Params != null)
				{
					if (Params["action"] != null && SimulateIsNumeric.IsNumeric(Params["action"]))
					{
						action = (Actions)(Convert.ToInt32(Params["action"].ToString()));
					}
				}
				try
				{
					sOut = "{\"result\":\"success\"}";
					switch (action)
					{
						case Actions.PropertySave:
							PropertySave();
							break;
						case Actions.PropertyDelete:
							PropertyDelete();
							break;
						case Actions.PropertyList:
							sOut = "[" + Utilities.LocalizeControl(PropertyList()) + "]";
							break;
						case Actions.PropertySortSwitch:
							UpdateSort();
							break;
						case Actions.ListOfLists:
							sOut = GetLists();
							break;
						case Actions.LoadView:
							sOut = LoadView();
							break;
						case Actions.RankGet:
							sOut = GetRank();
							break;
						case Actions.RankSave:
							RankSave();
							break;
						case Actions.RankDelete:
							RankDelete();
							break;
						case Actions.FilterGet:
							sOut = FilterGet();
							break;
						case Actions.FilterSave:
							FilterSave();
							break;
						case Actions.FilterDelete:
							FilterDelete();




							break;
					}
				}
				catch (Exception ex)
				{
					Exceptions.LogException(ex);
					sOut = "{";
					sOut += Utilities.JSON.Pair("result", "failed");
					sOut += ",";
					sOut += Utilities.JSON.Pair("message", ex.Message);
					sOut += "}";
				}

			}
			else
			{
				sOut = "{";
				sOut += Utilities.JSON.Pair("result", "failed");
				sOut += ",";
				sOut += Utilities.JSON.Pair("message", "Invalid Request");
				sOut += "}";
			}
			context.Response.ContentType = "text/plain";
			context.Response.Write(sOut);
		}
		private string FilterGet()
		{
			int FilterId = -1;
			if (Params.ContainsKey("FilterId"))
			{
				FilterId = Convert.ToInt32(Params["FilterId"]);
			}
			FilterController fc = new FilterController();
			FilterInfo filter = fc.Filter_Get(PortalId, ModuleId, FilterId);
			string sOut = "{";
			sOut += Utilities.JSON.Pair("FilterId", filter.FilterId.ToString());
			sOut += ",";
			sOut += Utilities.JSON.Pair("FilterType", filter.FilterType.ToString());
			sOut += ",";
			sOut += Utilities.JSON.Pair("Find", HttpUtility.UrlEncode(filter.Find.Replace(" ", "-|-")));
			sOut += ",";
			sOut += Utilities.JSON.Pair("Replacement", HttpUtility.UrlEncode(filter.Replace.Replace(" ", "-|-")));
			sOut += "}";
			return sOut;
		}
		private void FilterSave()
		{
			FilterInfo filter = new FilterInfo();
			filter.FilterId = -1;
			filter.ModuleId = ModuleId;
			filter.PortalId = PortalId;
			if (Params.ContainsKey("FilterId"))
			{
				filter.FilterId = Convert.ToInt32(Params["FilterId"]);
			}
			if (Params.ContainsKey("Find"))
			{
				filter.Find = Params["Find"].ToString();
			}
			if (Params.ContainsKey("Replacement"))
			{
				filter.Replace = Params["Replacement"].ToString();
			}
			if (Params.ContainsKey("FilterType"))
			{
				filter.FilterType = Params["FilterType"].ToString();
			}

			FilterController fc = new FilterController();
			filter = fc.Filter_Save(filter);
		}
		private void FilterDelete()
		{
			int FilterId = -1;
			if (Params.ContainsKey("FilterId"))
			{
				FilterId = Convert.ToInt32(Params["FilterId"]);
			}
			if (FilterId == -1)
			{
				return;
			}
			FilterController fc = new FilterController();
			fc.Filter_Delete(PortalId, ModuleId, FilterId);

		}
		private string GetRank()
		{
			int RankId = -1;
			if (Params.ContainsKey("RankId"))
			{
				RankId = Convert.ToInt32(Params["RankId"]);
			}
			RewardController rc = new RewardController();
			RewardInfo rank = rc.Reward_Get(PortalId, ModuleId, RankId);
			string sOut = "{";
			sOut += Utilities.JSON.Pair("RankId", rank.RankId.ToString());
			sOut += ",";
			sOut += Utilities.JSON.Pair("RankName", rank.RankName);
			sOut += ",";
			sOut += Utilities.JSON.Pair("MinPosts", rank.MinPosts.ToString());
			sOut += ",";
			sOut += Utilities.JSON.Pair("MaxPosts", rank.MaxPosts.ToString());
			sOut += ",";
			sOut += Utilities.JSON.Pair("Display", rank.Display.ToLowerInvariant().Replace("activeforums/ranks", "activeforums/images/ranks"));
			sOut += "}";
			return sOut;
		}
		private void RankSave()
		{
			RewardInfo rank = new RewardInfo();
			rank.RankId = -1;
			rank.ModuleId = ModuleId;
			rank.PortalId = PortalId;
			if (Params.ContainsKey("RankId"))
			{
				rank.RankId = Convert.ToInt32(Params["RankId"]);
			}
			if (Params.ContainsKey("RankName"))
			{
				rank.RankName = Params["RankName"].ToString();
			}
			if (Params.ContainsKey("MinPosts"))
			{
				rank.MinPosts = Convert.ToInt32(Params["MinPosts"]);
			}
			if (Params.ContainsKey("MaxPosts"))
			{
				rank.MaxPosts = Convert.ToInt32(Params["MaxPosts"]);
			}
			if (Params.ContainsKey("Display"))
			{
				rank.Display = Params["Display"].ToString();
			}
			RewardController rc = new RewardController();
			rank = rc.Reward_Save(rank);
		}
		private void RankDelete()
		{
			int RankId = -1;
			if (Params.ContainsKey("RankId"))
			{
				RankId = Convert.ToInt32(Params["RankId"]);
			}
			if (RankId == -1)
			{
				return;
			}
			RewardController rc = new RewardController();
			rc.Reward_Delete(PortalId, ModuleId, RankId);

		}
		private void PropertySave()
		{

			PropertiesController pc = new PropertiesController();
			PropertiesInfo pi = new PropertiesInfo();
			pi.PropertyId = -1;
			pi.PortalId = PortalId;
			pi = (PropertiesInfo)(Utilities.ConvertFromHashTableToObject(Params, pi));
			pi.Name = Utilities.CleanName(pi.Name);
			if (! (string.IsNullOrEmpty(pi.ValidationExpression)))
			{
				pi.ValidationExpression = HttpUtility.UrlDecode(HttpUtility.HtmlDecode(pi.ValidationExpression));
			}
			if (pi.PropertyId == -1)
			{
				string lbl = Params["Label"].ToString();
				LocalizationUtils lcUtils = new LocalizationUtils();
				lcUtils.SaveResource("[RESX:" + pi.Name + "].Text", lbl, PortalId);
			}
			else
			{
				if (Utilities.GetSharedResource("[RESX:" + pi.Name + "]").ToLowerInvariant().Trim() != Params["Label"].ToString().ToLowerInvariant().Trim())
				{
					LocalizationUtils lcUtils = new LocalizationUtils();
					lcUtils.SaveResource("[RESX:" + pi.Name + "].Text", Params["Label"].ToString(), PortalId);
				}

			}
			pc.SaveProperty(pi);
			ForumController fc = new ForumController();
			Forum fi = fc.GetForum(PortalId, ModuleId, pi.ObjectOwnerId, true);
			fi.HasProperties = true;
			fc.Forums_Save(PortalId, fi, false, false);

		}
		private string PropertyList()
		{

			StringBuilder sb = new StringBuilder();
			PropertiesController pc = new PropertiesController();
			if (! (string.IsNullOrEmpty(Params["ObjectOwnerId"].ToString())))
			{
				return pc.ListPropertiesJSON(PortalId, Convert.ToInt32(Params["ObjectType"]), Convert.ToInt32(Params["ObjectOwnerId"]));
			}
			else
			{
				return string.Empty;
			}


		}
		private void UpdateSort()
		{

			int propertyId = -1;
			int sortOrder = -1;
			PropertiesController pc = new PropertiesController();
			string props = Params["props"].ToString();
			props = props.Remove(props.LastIndexOf("^"));
			foreach (string s in props.Split('^'))
			{
				if (! (string.IsNullOrEmpty(props)))
				{
					propertyId = Convert.ToInt32(s.Split('|')[0]);
					sortOrder = Convert.ToInt32(s.Split('|')[1]);
					PropertiesInfo pi = pc.GetProperty(propertyId, PortalId);
					if (pi != null)
					{
						pi.SortOrder = sortOrder;
						pc.SaveProperty(pi);
					}
				}
			}
		}
		internal void PropertyDelete()
		{
			PropertiesController pc = new PropertiesController();
			PropertiesInfo prop = pc.GetProperty(Convert.ToInt32(Params["propertyid"]), PortalId);
			if (prop != null)
			{
				pc.DeleteProperty(PortalId, Convert.ToInt32(Params["propertyid"]));
				if (! (pc.ListProperties(PortalId, prop.ObjectType, prop.ObjectOwnerId).Count > 0))
				{
					ForumController fc = new ForumController();
					Forum fi = fc.GetForum(PortalId, ModuleId, prop.ObjectOwnerId, true);
					fi.HasProperties = false;
					fc.Forums_Save(PortalId, fi, false, false);
				}
			}


		}
		private string GetLists()
		{
			StringBuilder sb = new StringBuilder();
			DotNetNuke.Common.Lists.ListController lists = new DotNetNuke.Common.Lists.ListController();
			//Dim list As DotNetNuke
			DotNetNuke.Common.Lists.ListInfoCollection lc = lists.GetListInfoCollection(string.Empty, string.Empty, PortalId);
			foreach (DotNetNuke.Common.Lists.ListInfo l in lc)
			{
				if (l.PortalID == PortalId)
				{
					sb.Append("{");
					sb.Append(Utilities.JSON.Pair("listname", l.Name));
					sb.Append(",");
					sb.Append(Utilities.JSON.Pair("listid", l.Key));
					sb.Append("},");
				}
			}
			string sOut = sb.ToString();
			if (sOut.EndsWith(","))
			{
				sOut = sOut.Substring(0, sOut.Length - 1);
				sOut = "[" + sOut + "]";
			}
			return sOut;
		}
		private string LoadView()
		{
			StringBuilder sb = new StringBuilder();
			string view = "home";
			if (Params["view"] != null)
			{
				view = Params["view"].ToString().ToLowerInvariant();
			}
			string sPath = HttpContext.Current.Server.MapPath("~/desktopmodules/activeforums/");
			string sFile = string.Empty;
			switch (view)
			{
				case "forumeditor":
					sFile = Utilities.GetFile(sPath + "\\admin\\forumeditor.ascx");
					break;
			}
			Controls.ControlPanel cpControls = new Controls.ControlPanel(PortalId, ModuleId);
			sFile = sFile.Replace("[AF:CONTROLS:SELECTTOPICSTEMPLATES]", cpControls.TemplatesOptions(Templates.TemplateTypes.TopicsView));
			sFile = sFile.Replace("[AF:CONTROLS:SELECTTOPICTEMPLATES]", cpControls.TemplatesOptions(Templates.TemplateTypes.TopicView));
			sFile = sFile.Replace("[AF:CONTROLS:SELECTTOPICFORMTEMPLATES]", cpControls.TemplatesOptions(Templates.TemplateTypes.TopicForm));
			sFile = sFile.Replace("[AF:CONTROLS:SELECTREPLYFORMTEMPLATES]", cpControls.TemplatesOptions(Templates.TemplateTypes.ReplyForm));
			sFile = sFile.Replace("[AF:CONTROLS:SELECTPROFILETEMPLATES]", cpControls.TemplatesOptions(Templates.TemplateTypes.Profile));
			sFile = sFile.Replace("[AF:CONTROLS:SELECTEMAILTEMPLATES]", cpControls.TemplatesOptions(Templates.TemplateTypes.Email));
			sFile = sFile.Replace("[AF:CONTROLS:SELECTMODEMAILTEMPLATES]", cpControls.TemplatesOptions(Templates.TemplateTypes.ModEmail));
			sFile = sFile.Replace("[AF:CONTROLS:GROUPFORUMS]", cpControls.ForumGroupOptions());
			sFile = sFile.Replace("[AF:CONTROLS:SECGRID:ROLES]", cpControls.BindRolesForSecurityGrid(HttpContext.Current.Server.MapPath("~/")));

			sFile = Utilities.LocalizeControl(sFile, true);
			return sFile;
		}




	}
}
