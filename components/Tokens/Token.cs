using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
	internal class Token
	{
		private string _group;
		private string _tokenTag;
		private string _tokenReplace;
		private string _permissions;
		internal string Group
		{
			get
			{
				return _group;
			}
			set
			{
				_group = value;
			}
		}
		internal string TokenTag
		{
			get
			{
				return _tokenTag;
			}
			set
			{
				_tokenTag = value;
			}
		}
		internal string TokenReplace
		{
			get
			{
				return _tokenReplace;
			}
			set
			{
				_tokenReplace = value;
			}
		}
		internal string Permissions
		{
			get
			{
				return _permissions;
			}
			set
			{
				_permissions = value;
			}
		}
	}
}
