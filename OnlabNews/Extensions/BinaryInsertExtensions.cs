using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlabNews.Extensions
{
	public static class BinaryInsertExtensions
	{
		public static int BinaryInsert<T>(this ObservableCollection<T> collection, T itemToInsert) where T : IComparable
		{
			int insertIndex = 0;
			int lowerBound = 0;
			int upperBound = collection.Count - 1;
			while (lowerBound <= upperBound)
			{
				insertIndex = (lowerBound + upperBound) >> 1;
				int comparisonResult = itemToInsert.CompareTo(collection.ElementAt(insertIndex));
				if (comparisonResult < 0)
					upperBound = insertIndex - 1;
				else if (comparisonResult > 0)
				{
					lowerBound = insertIndex + 1;
					if (lowerBound > upperBound)
						insertIndex++;
				}
				else if (comparisonResult == 0)
					return -1;
			}
			collection.Insert(insertIndex, itemToInsert);
			return insertIndex;
		}
	}
}
