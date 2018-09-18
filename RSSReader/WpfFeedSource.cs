using RSSReader.Properties;
using RSSReader.Rss;

using System;
using System.Collections.Specialized;

namespace RSSReader
{
	/// <summary>
	/// Implementation of <see cref="IFeedSource"/> which handles IsEnabled from <see cref="Settings.Default"/>
	/// </summary>
	public class WpfFeedSource : IFeedSource
	{
		private string _originalUri;

		/// <summary>
		/// Creates new <see cref="WpfFeedSource"/>
		/// </summary>
		/// <param name="originalUri">Original URL to RSS feed that will be saved</param>
		public WpfFeedSource(string originalUri)
		{
			_originalUri = originalUri;
		}

		public Uri SourceUri
		{
			get => new Uri(_originalUri);
			set => _originalUri = value.ToString();
		}

		/// <summary>
		/// Equals to true <see cref="Settings.Default.DisabledFeeds"/> equals to null or doesn't contains 
		/// <see cref="_originalUri"/>. Otherwise equals to false
		/// 
		/// When settinng firstly checks is new value differents from curernt than manipuldates with <see cref="Settings.Default.DisabledFeeds"/>
		/// </summary>
		public bool IsEnabled
		{
			get => !(Settings.Default.DisabledFeeds?.Contains(_originalUri) ?? false);
			set
			{
				if (value == IsEnabled)
				{
					return;
				}
				else if (Settings.Default.DisabledFeeds == null)
				{
					Settings.Default.DisabledFeeds = new StringCollection();
				}

				if (value)
				{
					Settings.Default.DisabledFeeds.Remove(_originalUri);
				}
				else
				{
					Settings.Default.DisabledFeeds.Add(_originalUri);
				}

				SettingsExtensions.Save(this);
			}
		}
	}
}
