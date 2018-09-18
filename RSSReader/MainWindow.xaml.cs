using RSSReader.Properties;
using RSSReader.Rss;

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace RSSReader
{
	/// <summary>
	/// Interaction logic for Main window
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Main rss reader controller
		/// </summary>
		private RssReader _rssReader;

		public MainWindow()
		{
			InitializeComponent();

			_rssReader = new RssReader(
				Settings.Default.UpdateInterval,
				GetFeedSources()
			);
			SettingsExtensions.OnSettingsSaved += SettingsUpdated;
			DataContext = _rssReader;

			ICollectionView view = CollectionViewSource.GetDefaultView(_rssReader.Entries);
			view.Filter = entry => (entry as RssEntry).Feed.Source?.IsEnabled ?? false;
		}

		/// <summary>
		/// Converts <see cref="Settings.Default.Feeds"/> from <see cref="StringCollection"/> to <see cref="List{IFeedSource}"/>
		/// </summary>
		/// <returns>Converted list</returns>
		private IList<IFeedSource> GetFeedSources()
		{
			return Settings.Default.Feeds.Cast<string>().
										  Select(s => new WpfFeedSource(s.Trim())).
										  Cast<IFeedSource>().
										  ToList();
		}

		/// <summary>
		/// Handler of <see cref="SettingsExtensions.OnSettingsSaved"/>. Restarts <see cref="_rssReader"/> dispatcher and updates other values.
		/// </summary>
		private async void SettingsUpdated(object sender, EventArgs e)
		{
			_rssReader.Sources = new HashSet<IFeedSource>(GetFeedSources(), new RssFeedComparerByUri());
			_rssReader.StopDispatcher();
			_rssReader.UpdateInterval = Settings.Default.UpdateInterval;
			await _rssReader.StartDispatcher();
		}

		private async void OnWindowLoaded(object sender, RoutedEventArgs e)
		{
			await _rssReader.StartDispatcher();
		}

		/// <summary>
		/// Handles when user enables or disables some feed sources. Refreshes view of <see cref="listBoxFeedItems"/>.
		/// </summary>
		private void FeedCheckChanged(object sender, RoutedEventArgs e)
		{
			CollectionViewSource.GetDefaultView(_rssReader.Entries).Refresh();
		}


		/// <summary>
		/// Hanfles when user double clicks on rss entry. Opens entry in default browser
		/// </summary>s
		private void FeedItemsDoubleClick(object sender, MouseButtonEventArgs e)
		{
			var listBox = sender as ListBox;
			var entry = listBox?.SelectedItem as RssEntry;

			if (entry?.Link != null)
			{
				Process.Start(entry.Link);
			}
		}
	}
}
