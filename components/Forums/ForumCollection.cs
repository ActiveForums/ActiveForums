using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums
{
	public class ForumCollection : CollectionBase, ICollection, IList
	{


		private Forum _Item;

		public void CopyTo(System.Array array, int index)
		{
			List.CopyTo(array, index);
		}

		public bool IsSynchronized
		{
			get
			{
				return List.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return List.SyncRoot;
			}
		}


		public int Add(Forum value)
		{
			return List.Add(value);
		}


		public bool Contains(Forum value)
		{
			return List.Contains(value);
		}

		public int IndexOf(Forum value)
		{
			return List.IndexOf(value);
		}

		public void Insert(int index, Forum value)
		{
			List.Insert(index, value);
		}

		public bool IsFixedSize
		{
			get
			{
				return List.IsFixedSize;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return List.IsReadOnly;
			}
		}

		public Forum this[int index]
		{
			get
			{
				return _Item;
			}
			set
			{
				_Item = value;
			}
		}

		public void Remove(object value)
		{
			List.Remove(value);
		}


	}
}

