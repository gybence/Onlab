using OnlabNews.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Models
{
	public class MutableGrouping<TKey, TElement> : RangeObservableCollection<TElement>, IGrouping<TKey, TElement>, IEquatable<MutableGrouping<TKey, TElement>>, IComparable where TElement : IComparable
	{
		#region properties, constructors

		public TKey Key { get; private set; }

		public MutableGrouping(TKey key)
		{
			Key = key;
		}

		public MutableGrouping(IGrouping<TKey, TElement> grouping)
		{
			if (grouping == null)
				throw new ArgumentNullException("grouping");

			if (!grouping.Any()) return;

			Key = grouping.Key;
			AddRange(grouping);
		}

		#endregion

		public void InsertToGrouping(IGrouping<TKey, TElement> grouping)
		{
			if (grouping == null)
				throw new ArgumentNullException("groupingToInsert");

			if (!grouping.Any()) return;

			if (!Key.Equals(grouping.Key))
				throw new ArgumentException("keys not matching");

			foreach (TElement x in grouping)
				this.BinaryInsert(x);
		}

		public void AddToGrouping(IGrouping<TKey, TElement> grouping)
		{
			if (grouping == null)
				throw new ArgumentNullException("groupingToInsert");

			if (!grouping.Any()) return;

			if (!Key.Equals(grouping.Key))
				throw new ArgumentException("keys not matching");

			AddRange(grouping);
		}

		public void InsertOneItemIntoGrouping(TKey key, TElement element)
		{
			if (!Key.Equals(key))
				throw new ArgumentException("keys not matching");

			this.BinaryInsert(element);
		}

		public void AddOneItemIntoGrouping(TKey key, TElement element)
		{
			if (!Key.Equals(key))
				throw new ArgumentException("keys not matching");

			this.Add(element);
		}

		public bool Equals(MutableGrouping<TKey, TElement> other)
		{
			return char.Equals(this.Key, other.Key);
		}


		public int CompareTo(object obj)
		{
			return string.Compare(Key.ToString(), (obj as MutableGrouping<TKey,TElement>).Key.ToString());
		}
		
	}
}
