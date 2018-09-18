using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml;

namespace RSSReader.Rss
{
	/// <summary>
	/// Automatically downloads feeds from specified sources
	/// </summary>
	public sealed class RssReader
	{
		/// <summary>
		/// Lock for not letting update of <see cref="FeedsInfos"/> and <see cref="Entries"/> in different threads in the same time
		/// </summary>
		private object _updateLock = new object();

		private NotifyCollection<RssEntry> _entries;
		private NotifyCollection<RssFeed> _feedsInfos;

		/// <summary>
		/// Dispatcher that calls updating of feeds and entries
		/// </summary>
		private DispatcherTimer _updatesDispatcher;
		/// <summary>
		/// Calculated hash of entries from last udpate, so it will be easier to find out should we call notifications of update for views
		/// </summary>
		private long _lastUpdateHashCode;

		/// <summary>
		/// Editable of feeds sources
		/// </summary>
		public ISet<IFeedSource> Sources;
		/// <summary>
		/// Interval between checking of new feeds
		/// </summary>
		public TimeSpan UpdateInterval
		{
			get => _updatesDispatcher.Interval;
			set => _updatesDispatcher.Interval = value;
		}

		/// <summary>
		/// Read-only collection of all entries
		/// </summary>
		public IReadOnlyNotifyCollection<RssFeed> FeedsInfos => _feedsInfos;
		/// <summary>
		/// Read-only collection of all feeds
		/// </summary>
		public IReadOnlyNotifyCollection<RssEntry> Entries => _entries;

		/// <summary>
		/// Creates new rss reader but don't starts dispatcher
		/// </summary>
		/// <param name="updatesInterval">Interval between two updates</param>
		/// <param name="sources">Feeds sources</param>
		public RssReader(TimeSpan updatesInterval, IEnumerable<IFeedSource> sources)
		{
			if (sources == null)
			{
				throw new ArgumentNullException(nameof(sources));
			}
			
			Sources = new HashSet<IFeedSource>(sources, new RssFeedComparerByUri());

			_entries = new NotifyCollection<RssEntry>();
			_feedsInfos = new NotifyCollection<RssFeed>();

			_updatesDispatcher = new DispatcherTimer
			{
				Interval = updatesInterval
			};
			_updatesDispatcher.Tick += ReloadFeed;
			_lastUpdateHashCode = 0;
		}

		/// <summary>
		/// Asynchronolly begins to update feeds every <see cref="UpdateInterval"/>
		/// </summary>
		public async Task StartDispatcher()
		{
			if (!_updatesDispatcher.IsEnabled)
			{
				_updatesDispatcher.Start();
				await LoadFeed();
			}
		}

		/// <summary>
		/// Stops dipatcher
		/// </summary>
		public void StopDispatcher()
		{
			_updatesDispatcher.Stop();
		}

		/// <summary>
		/// Handler of dipsatcher tick
		/// </summary>
		private async void ReloadFeed(object sender, EventArgs e)
		{
			await LoadFeed();
		}

		/// <summary>
		/// Asynchronosly loads new feed
		/// </summary>
		public async Task LoadFeed()
		{
			SortedSet<RssEntry> sortedEntries = new SortedSet<RssEntry>();

			var feedsInfos = new List<RssFeed>();
			foreach (IFeedSource source in Sources)
			{
				RssFeed feed;
				try
				{
					feed = await RssFeed.Load(source);
				}
				catch (WebException)
				{ // we will just ignore some connection problems, but it's bad
					continue;
				}
				catch (XmlException)
				{
					continue;
				}

				foreach (RssEntry entry in feed.Items)
				{
					sortedEntries.Add(entry);
				}
				feedsInfos.Add(feed);
			}

			// If user will set very small update time and have slow connection this lock will be helpful
			lock (_updateLock)
			{
				long entriesHashcode = GetEntriesHashCodes(sortedEntries);
				if (entriesHashcode != _lastUpdateHashCode)
				{
					// maybe add lock there?
					// it won't be a transaction, so there may be some perfomance issues bevause of recalling Change event
					// but i think it's not so bad, so fix isn't necessarry

					_entries.Clear();
					sortedEntries.Reverse().ToList().ForEach(e => _entries.Add(e));

					_feedsInfos.Clear();
					feedsInfos.ForEach(f => _feedsInfos.Add(f));

					_lastUpdateHashCode = entriesHashcode;
				}
			}
		}

		/// <summary>
		/// Calculates hash code of entries
		/// </summary>
		/// <param name="entries">Entires to work with</param>
		/// <returns>Unique (not really, nut good for our task) long for some collection of rss entries</returns>
		private long GetEntriesHashCodes(IEnumerable<RssEntry> entries)
		{
			long hashcode = 0;

			foreach (RssEntry entry in entries)
			{
				hashcode = hashcode * 31 + 1 + entry.Title.GetHashCode();
			}

			return hashcode;
		}
	}
}
