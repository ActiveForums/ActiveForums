using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
	public class PropertiesInfo
	{
		public int PropertyId {get; set;}
		public int PortalId {get; set;}
		public int ObjectType {get; set;}
		public int ObjectOwnerId {get; set;}
		public string Name {get; set;}
		public string DataType {get; set;}
		public int DefaultAccessControl {get; set;}
		public bool IsHidden {get; set;}
		public bool IsRequired {get; set;}
		public bool IsReadOnly {get; set;}
		private string _ValidationExpression = string.Empty;
		public string ValidationExpression
		{
			get
			{
				return _ValidationExpression;
			}
			set
			{
				_ValidationExpression = value;
			}
		}
		private string _EditTemplate = string.Empty;
		public string EditTemplate
		{
			get
			{
				return _EditTemplate;
			}
			set
			{
				_EditTemplate = value;
			}
		}
		private string _ViewTemplate = string.Empty;
		public string ViewTemplate
		{
			get
			{
				return _ViewTemplate;
			}
			set
			{
				_ViewTemplate = value;
			}
		}
		public int SortOrder {get; set;}
		public string DefaultValue {get; set;}
	}
}
