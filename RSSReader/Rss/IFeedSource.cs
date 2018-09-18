using System;

namespace RSSReader.Rss
{
	/// <summary>
	/// Interface that describes rss feed source
	/// </summary>
	public interface IFeedSource
	{
		/// <summary>
		/// Uri of rss feed
		/// </summary>
		/// <seealso cref="RssFeed"/>
		Uri SourceUri { get; set; }

		/// <summary>
		/// Indicates should be entries of feed be visible 
		/// </summary>
		bool IsEnabled { get; set; }
	}
}