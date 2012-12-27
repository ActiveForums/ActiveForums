using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Web;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Text;

namespace DotNetNuke.Modules.ActiveForums
{
	public partial class Utilities
	{
		public class JSON
		{
			public static string Pair(string name, string value)
			{
				return Pair(name, value, false);
			}
			public static string Pair(string name, string value, bool isObject)
			{
				if (string.IsNullOrEmpty(value))
				{
					value = ((char)(34)).ToString() + ((char)(34)).ToString();
				}
				else if (! (SimulateIsNumeric.IsNumeric(value)) & ! (value.ToLower() == "true" || value.ToLower() == "false") && isObject == false)
				{
					value = ((char)(34)).ToString() + JSON.EscapeJsonString(value) + ((char)(34)).ToString();
				}
				else if (value.Trim().ToLowerInvariant() == "true" || value.Trim().ToLowerInvariant() == "false")
				{
					value = value.Trim().ToLower();
				}
				return ((char)(34)).ToString() + name + ((char)(34)).ToString() + ":" + value;
			}
			public static string ConvertToJSONAssociativeArray(Dictionary<string, string> dict)
			{
				List<string> elements = new List<string>();
				foreach (KeyValuePair<string, string> Pair in dict)
				{
					if (! (string.IsNullOrEmpty(Pair.Value)))
					{
						elements.Add(string.Format("\"{0}\":{2}{1}{2}", EscapeJsonString(Pair.Key), EscapeJsonString(Pair.Value), ((IsJSONArray(Pair.Value) || IsBoolean(Pair.Value)) ? string.Empty : "\"")));
					}
				}
				return "{" + string.Join(",", elements.ToArray()) + "}";
			}
			public static bool IsJSONArray(string test)
			{
				return test.StartsWith("{") && ! (test.StartsWith("{*")) || test.StartsWith("[");
			}
			public static bool IsBoolean(string test)
			{
				return test.Equals("false") || test.Equals("true");
			}
			public static string ConvertToJSONArray(List<string> list)
			{
				StringBuilder builder = new StringBuilder();
				builder.Append("[");
				foreach (string item in list)
				{
					builder.Append(string.Format("{0}{1}{0},", ((IsJSONArray(item) || IsBoolean(item)) ? string.Empty : "\""), EscapeJsonString(item)));
				}
				builder.Replace(",", "]", builder.Length - 1, 1);
				return builder.ToString();
			}
			public static object ConvertFromJSONToObject(string array, object InfoObject)
			{
				Dictionary<string, string> dict = ConvertFromJSONAssoicativeArray(array);
				if (dict.Count > 0)
				{
					Type myType = InfoObject.GetType();
					System.Reflection.PropertyInfo[] myProperties = myType.GetProperties((System.Reflection.BindingFlags.Public | BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance));
					string sKey = string.Empty;
					string sValue = string.Empty;
					foreach (PropertyInfo pItem in myProperties)
					{
						sValue = string.Empty;
						sKey = pItem.Name.ToLower();
						if (dict.ContainsKey(sKey))
						{
							sValue = dict[sKey];
						}
						if (! (string.IsNullOrEmpty(sValue)))
						{
							object obj = null;
							switch (pItem.PropertyType.ToString())
							{
								case "System.Int16":
									obj = Convert.ToInt32(sValue);
									break;
								case "System.Int32":
									obj = Convert.ToInt32(sValue);
									break;
								case "System.Int64":
									obj = Convert.ToInt64(sValue);
									break;
								case "System.Single":
									obj = Convert.ToSingle(sValue);
									break;
								case "System.Double":
									obj = Convert.ToDouble(sValue);
									break;
								case "System.Decimal":
									obj = Convert.ToDecimal(sValue);
									break;
								case "System.DateTime":
									obj = Convert.ToDateTime(sValue);
									break;
								case "System.String":
								case "System.Char":
									obj = Convert.ToString(sValue);
									break;
								case "System.Boolean":
									obj = Convert.ToBoolean(sValue);
									break;
								case "System.Guid":
									obj = new Guid(sValue);

									break;
							}
							if (obj != null)
							{
								InfoObject.GetType().GetProperty(pItem.Name).SetValue(InfoObject, obj, BindingFlags.Public | BindingFlags.NonPublic, null, null, null);
							}

						}
					}
				}
				return InfoObject;
			}
			public static List<string> ConvertFromJSONArray(string array)
			{
				if (! (string.IsNullOrEmpty(array)))
				{
					array = array.Replace("[", "").Replace("]", "").Replace("\"", "");
					return new List<string>(array.Split(','));
				}
				else
				{
					return new List<string>();
				}
			}
			public static Dictionary<string, string> ConvertFromJSONAssoicativeArray(string array)
			{
				Dictionary<string, string> dict = new Dictionary<string, string>();
				if (! (string.IsNullOrEmpty(array)))
				{
					if (array.StartsWith("{"))
					{
						array = array.Substring(1);
					}
					if (array.EndsWith("}"))
					{
						array = array.Substring(0, array.Length - 1);
					}
					array = array.Replace("\":", "|").Replace("\"", "").Replace("\\/", "/").Replace(", ", "#^");
					List<string> pairs = new List<string>(array.Split(','));
					foreach (string pair in pairs)
					{
						if (! (string.IsNullOrEmpty(pair)))
						{
							string[] pairArray = pair.Split('|');
							string val = string.Empty;
							if (pairArray.Length == 2)
							{
								val = pairArray[1];
							}
							val = val.Replace("#^", ", ");
							dict.Add(pairArray[0], HttpUtility.UrlDecode(val));
						}
					}
					return dict;
				}
				else
				{
					return new Dictionary<string, string>();
				}
			}
			public static Hashtable ConvertFromJSONAssoicativeArrayToHashTable(string array)
			{
				Hashtable ht = new Hashtable();
				if (! (string.IsNullOrEmpty(array)))
				{
					if (array.StartsWith("{"))
					{
						array = array.Substring(1);
					}
					if (array.EndsWith("}"))
					{
						array = array.Substring(0, array.Length - 1);
					}
					array = array.Replace("\":", "|").Replace("\"", "").Replace("\\/", "/").Replace(", ", "#^");
					List<string> pairs = new List<string>(array.Split(','));
					foreach (string pair in pairs)
					{
						if (! (string.IsNullOrEmpty(pair)))
						{
							string[] pairArray = pair.Split('|');
							string val = string.Empty;
							if (pairArray.Length == 2)
							{
								val = pairArray[1];
							}
							val = val.Replace("#^", ", ");
							ht.Add(pairArray[0], val);
						}
					}
					return ht;
				}
				else
				{
					return ht;
				}
			}
			/// <summary>
			/// Escape backslashes and double quotes
			/// </summary>
			/// <param name="originalString">string</param>
			/// <returns>string</returns>
			internal static string EscapeJsonString(string originalString)
			{
				Regex reg = new Regex("\\s+", RegexOptions.Multiline);
				originalString = reg.Replace(originalString, " ");
				return ((IsJSONArray(originalString)) ? originalString : originalString.Replace("\\/", "/").Replace("/", "\\/").Replace("\\\"", "\"").Replace("\"", "\\\"").Replace(System.Environment.NewLine, string.Empty));
			}
		}

	}
}

