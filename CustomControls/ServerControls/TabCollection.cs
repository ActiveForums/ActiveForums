//© 2004 - 2007 ActiveModules, Inc. All Rights Reserved
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace DotNetNuke.Modules.ActiveForums.Controls
{
	public class TabCollection : IList<Tab>, IEnumerable<Tab>, ICollection<Tab>
	{
		private List<Tab> _contents = new List<Tab>();
		private int _count;
		private Tab _tab;
		public Tab Tab
		{
			get
			{
				return _tab;
			}
			set
			{
				_tab = value;
			}
		}

		public TabCollection()
		{
			_count = 0;
			//_contents = New ArrayList

		}

		public void Add(Tab item)
		{
			_contents.Add(item);
			_count = _count + 1;
		}

		public void Clear()
		{
			_contents = null;
			_count = 0;

		}

		public bool Contains(Tab item)
		{
			bool inList = false;

			int i = 0;
			for (i = 0; i <= Count; i++)
			{

				if (((Tab)(_contents[i])).Text.ToLower() == item.Text.ToLower())
				{
					inList = true;
					break;
				}
			}
			return inList;
		}

		public void CopyTo(Tab[] array, int arrayIndex)
		{
			int j = arrayIndex;
			int i = 0;
			for (i = 0; i <= Count; i++)
			{
				array.SetValue(_contents[i], j);
				j = j + 1;
			}

		}

		public int Count
		{
			get
			{
				return _count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(Tab item)
		{
			RemoveAt(IndexOf(item));
			return true;
		}

        IEnumerator<Tab> IEnumerable<Tab>.GetEnumerator()
		{
            return new TabEnum(_contents);
		}

		public int IndexOf(Tab item)
		{
			int itemIndex = -1;

			int i = 0;
			for (i = 0; i <= Count; i++)
			{

				if (((Tab)(_contents[i])).Text.ToLower() == item.Text.ToLower())
				{

					itemIndex = i;
					break;

				}

			}

			return itemIndex;

		}

		public void Insert(int index, Tab item)
		{
			_count = _count + 1;

			int i = 0;
			for (i = Count - 1; i <= index; i++)
			{
				_contents[i] = _contents[i - 1];
			}

			_contents[index] = item;

		}

		public Tab this[int index]
		{
			get
			{
				return (Tab)(_contents[index]);
			}
			set
			{
				_contents[index] = value;
			}
		}

		public void RemoveAt(int index)
		{
			if (index >= 0 && index < Count)
			{

				int i = 0;
				for (i = index; i < Count; i++)
				{

					_contents[i] = _contents[i + 1];
				}
				_count = _count - 1;

			}


		}

        IEnumerator IEnumerable.GetEnumerator()
		{
			return _contents.GetEnumerator();
		}
	}
	public class TabEnum : IEnumerator<Tab>
	{

		public List<Tab> _tabs;

		private int position = -1;

		public TabEnum(List<Tab> list)
		{
			_tabs = list;
		}

		public bool MoveNext()
		{
			position = position + 1;
			return (position < _tabs.Count);
		}

		public void Reset()
		{
			position = -1;
		}

		object IEnumerator.Current
		{
			get
			{
				try
				{
					return _tabs[position];
				}
				catch (IndexOutOfRangeException ex)
				{
					throw new InvalidOperationException();
				}
			}
		}

		Tab IEnumerator<Tab>.Current
		{
			get
			{
				return _tabs[position];
			}
		}

		public void Dispose()
		{
			//this.Dispose();
		}
	}

}
