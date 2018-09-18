using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Xml;

namespace RSSReader.Rss
{
	/// <summary>
	/// Class that represents rss feed 
	/// </summary>
	public sealed class RssFeed
	{
		/// <summary>
		/// Entries of feed
		/// </summary>
		private List<RssEntry> _entries;

		/// <summary>
		/// Feed title
		/// </summary>
		public string Title { get; }
		/// <summary>
		/// Link to original site
		/// </summary>
		public string Link { get; }
		/// <summary>
		/// Description of feed
		/// </summary>
		public string Description { get; }
		/// <summary>
		/// Logo of feed
		/// </summary>
		public BitmapImage Image { get; }
		/// <summary>
		/// Feed source description
		/// </summary>
		public IFeedSource Source { get; }

		/// <summary>
		/// View of feed entries
		/// </summary>
		public IReadOnlyCollection<RssEntry> Items => new List<RssEntry>(_entries);

		private RssFeed(string title, string link, string description, BitmapImage image, IFeedSource source)
		{
			Title = title;
			Link = link;
			Description = description;
			Image = image;
			Source = source;
			_entries = new List<RssEntry>();
		}

		/// <summary>
		/// Downloads feed from feed source
		/// </summary>
		/// <param name="source">Description of feed source</param>
		/// <returns>New RssFeed</returns>
		public static async Task<RssFeed> Load(IFeedSource source)
		{
			WebClient webClient = new WebClient
			{
				Encoding = Encoding.UTF8
			};
			string feedString = await webClient.DownloadStringTaskAsync(source.SourceUri);

			return Load(feedString, source);
		}

		/// <summary>
		/// Parses xml string into RssFeed
		/// </summary>
		/// <param name="feedXml">XML of feed</param>
		/// <param name="source">Description of feed source</param>
		/// <returns>Parsed feed</returns>
		public static RssFeed Load(string feedXml, IFeedSource source = null)
		{
			XmlReader xmlFeedReader = XmlReader.Create(new StringReader(feedXml));
			SyndicationFeed syndicationFeed = SyndicationFeed.Load(xmlFeedReader);

			return Load(syndicationFeed, source);
		}

		/// <summary>
		/// Converts syndication feed to rss feed
		/// </summary>
		/// <param name="syndicationFeed">Feed to converts</param>
		/// <param name="source">Description of feed source</param>
		/// <returns></returns>
		public static RssFeed Load(SyndicationFeed syndicationFeed, IFeedSource source = null)
		{
			RssFeed feed = new RssFeed(
					syndicationFeed.Title?.Text,
					syndicationFeed.Links.FirstOrDefault()?.Uri?.ToString(),
					syndicationFeed.Description?.Text,
					syndicationFeed.ImageUrl == null ? null : new BitmapImage(syndicationFeed.ImageUrl),
					source);

			feed._entries.AddRange(syndicationFeed.Items.Select(e => RssEntry.FromSyndicationItem(feed, e)));

			return feed;
		}
	}
}