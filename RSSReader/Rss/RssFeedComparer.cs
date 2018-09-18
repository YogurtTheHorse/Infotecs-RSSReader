using System.Collections.Generic;

namespace RSSReader.Rss
{
	/// <summary>
	/// Compares two <see cref="IFeedSource"/> by theirs URIs
	/// </summary>
	public class RssFeedComparerByUri : IEqualityComparer<IFeedSource>
	{
		public bool Equals(IFeedSource x, IFeedSource y)
		{
			return x?.SourceUri != null && x.SourceUri == y.SourceUri;
		}

		public int GetHashCode(IFeedSource obj)
		{
			return obj?.SourceUri?.GetHashCode() ?? 0;
		}
	}
}