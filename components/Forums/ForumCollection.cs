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

