using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using System.Reflection;
namespace DotNetNuke.Modules.ActiveForums.Data
{
	public class Null
	{
		// define application encoded null values 
		public static short NullShort
		{
			get
			{
				return -1;
			}
		}
		public static int NullInteger
		{
			get
			{
				return -1;
			}
		}
		public static byte NullByte
		{
			get
			{
				return 255;
			}
		}
		public static float NullSingle
		{
			get
			{
				return float.MinValue;
			}
		}
		public static double NullDouble
		{
			get
			{
				return double.MinValue;
			}
		}
		public static decimal NullDecimal
		{
			get
			{
				return decimal.MinValue;
			}
		}
		public static DateTime NullDate
		{
			get
			{
				return DateTime.MinValue;
			}
		}
		public static string NullString
		{
			get
			{
				return "";
			}
		}
		public static bool NullBoolean
		{
			get
			{
				return false;
			}
		}
		public static Guid NullGuid
		{
			get
			{
				return Guid.Empty;
			}
		}

		// sets a field to an application encoded null value ( used in BLL layer )
		public static object SetNull(object objValue, object objField)
		{
			object tempSetNull = null;
			if (Convert.IsDBNull(objValue))
			{
				if (objField is short)
				{
					tempSetNull = NullShort;
				}
				else if (objField is byte)
				{
					tempSetNull = NullByte;
				}
				else if (objField is int)
				{
					tempSetNull = NullInteger;
				}
				else if (objField is float)
				{
					tempSetNull = NullSingle;
				}
				else if (objField is double)
				{
					tempSetNull = NullDouble;
				}
				else if (objField is decimal)
				{
					tempSetNull = NullDecimal;
				}
				else if (objField is DateTime)
				{
					tempSetNull = NullDate;
				}
				else if (objField is string)
				{
					tempSetNull = NullString;
				}
				else if (objField is bool)
				{
					tempSetNull = NullBoolean;
				}
				else if (objField is Guid)
				{
					tempSetNull = NullGuid;
				}
				else // complex object
				{
					tempSetNull = null;
				}
			}
			else // return value
			{
				tempSetNull = objValue;
			}
			return tempSetNull;
		}

		// sets a field to an application encoded null value ( used in BLL layer )
		public static object SetNull(PropertyInfo objPropertyInfo)
		{
			object tempSetNull = null;
			switch (objPropertyInfo.PropertyType.ToString())
			{
				case "System.Int16":
					tempSetNull = NullShort;
					break;
				case "System.Int32":
				case "System.Int64":
					tempSetNull = NullInteger;
					break;
				case "system.Byte":
					tempSetNull = NullByte;
					break;
				case "System.Single":
					tempSetNull = NullSingle;
					break;
				case "System.Double":
					tempSetNull = NullDouble;
					break;
				case "System.Decimal":
					tempSetNull = NullDecimal;
					break;
				case "System.DateTime":
					tempSetNull = NullDate;
					break;
				case "System.String":
				case "System.Char":
					tempSetNull = NullString;
					break;
				case "System.Boolean":
					tempSetNull = NullBoolean;
					break;
				case "System.Guid":
					tempSetNull = NullGuid;
					break;
				default:
					// Enumerations default to the first entry
					Type pType = objPropertyInfo.PropertyType;
					if (pType.BaseType.Equals(typeof(System.Enum)))
					{
						System.Array objEnumValues = System.Enum.GetValues(pType);
						Array.Sort(objEnumValues);
						tempSetNull = System.Enum.ToObject(pType, objEnumValues.GetValue(0));
					}
					else // complex object
					{
						tempSetNull = null;
					}
					break;
			}
			return tempSetNull;
		}

		// convert an application encoded null value to a database null value ( used in DAL )
		public static object GetNull(object objField, object objDBNull)
		{
			object tempGetNull = null;
			tempGetNull = objField;
			if (objField == null)
			{
				tempGetNull = objDBNull;
			}
			else if (objField is byte)
			{
				if (Convert.ToByte(objField) == NullByte)
				{
					tempGetNull = objDBNull;
				}
			}
			else if (objField is short)
			{
				if (Convert.ToInt16(objField) == NullShort)
				{
					tempGetNull = objDBNull;
				}
			}
			else if (objField is int)
			{
				if (Convert.ToInt32(objField) == NullInteger)
				{
					tempGetNull = objDBNull;
				}
			}
			else if (objField is float)
			{
				if (Convert.ToSingle(objField) == NullSingle)
				{
					tempGetNull = objDBNull;
				}
			}
			else if (objField is double)
			{
				if (Convert.ToDouble(objField) == NullDouble)
				{
					tempGetNull = objDBNull;
				}
			}
			else if (objField is decimal)
			{
				if (Convert.ToDecimal(objField) == NullDecimal)
				{
					tempGetNull = objDBNull;
				}
			}
			else if (objField is DateTime)
			{
				// compare the Date part of the DateTime with the DatePart of the NullDate ( this avoids subtle time differences )
				if (Convert.ToDateTime(objField).Date == NullDate.Date)
				{
					tempGetNull = objDBNull;
				}
			}
			else if (objField is string)
			{
				if (objField == null)
				{
					tempGetNull = objDBNull;
				}
				else
				{
					if (objField.ToString() == NullString)
					{
						tempGetNull = objDBNull;
					}
				}
			}
			else if (objField is bool)
			{
				if (Convert.ToBoolean(objField) == NullBoolean)
				{
					tempGetNull = objDBNull;
				}
			}
			else if (objField is Guid)
			{
				if (((System.Guid)objField).Equals(NullGuid))
				{
					tempGetNull = objDBNull;
				}
			}
			return tempGetNull;
		}

		// checks if a field contains an application encoded null value
		public static bool IsNull(object objField)
		{
			bool tempIsNull = false;
			if (objField != null)
			{
				if (objField is int)
				{
					tempIsNull = objField.Equals(NullInteger);
				}
				else if (objField is short)
				{
					tempIsNull = objField.Equals(NullShort);
				}
				else if (objField is byte)
				{
					tempIsNull = objField.Equals(NullByte);
				}
				else if (objField is float)
				{
					tempIsNull = objField.Equals(NullSingle);
				}
				else if (objField is double)
				{
					tempIsNull = objField.Equals(NullDouble);
				}
				else if (objField is decimal)
				{
					tempIsNull = objField.Equals(NullDecimal);
				}
				else if (objField is DateTime)
				{
					DateTime objDate = Convert.ToDateTime(objField);
					tempIsNull = objDate.Date.Equals(NullDate.Date);
				}
				else if (objField is string)
				{
					tempIsNull = objField.Equals(NullString);
				}
				else if (objField is bool)
				{
					tempIsNull = objField.Equals(NullBoolean);
				}
				else if (objField is Guid)
				{
					tempIsNull = objField.Equals(NullGuid);
				}
				else // complex object
				{
					tempIsNull = false;
				}
			}
			else
			{
				tempIsNull = true;
			}
			return tempIsNull;
		}

	}


}

