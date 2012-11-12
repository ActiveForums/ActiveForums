//© 2004 - 2008 ActiveModules, Inc. All Rights Reserved
using System;
//ORIGINAL LINE: Imports System.Web.HttpContext

namespace DotNetNuke.Modules.ActiveForums
{
	public class TopicBase : SettingsBase
	{
#region Private Members
		private int _TopicId = -1;
#endregion
#region Public Properties
		public int TopicId
		{
			get
			{
			    if (_TopicId == -1)
				{
				    if (Request.Params[ParamKeys.TopicId] != null)
					{
					    if (SimulateIsNumeric.IsNumeric(Request.Params[ParamKeys.TopicId]))
						{
							_TopicId = Convert.ToInt32(Request.Params[ParamKeys.TopicId]);
							return _TopicId;
						}
					    return _TopicId;
					}
				    return _TopicId;
				}
			    return _TopicId;
			}
		    set
			{
				_TopicId = value;
			}
		}
#endregion

	}
}
