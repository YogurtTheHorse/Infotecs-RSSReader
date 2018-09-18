using System;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;

namespace RSSReader.Rss
{
	/// <summary>
	/// Class that handles info about entry in <see cref="RssFeed"/>
	/// </summary>
	public sealed class RssEntry : IComparable<RssEntry>
	{
		/// <summary>
		/// Ttile of post
		/// </summary>
		public string Title { get; }
		/// <summary>
		/// Summary of post
		/// </summary>
		public string Description { get; }
		/// <summary>
		/// Link to open in browser
		/// </summary>
		public string Link { get; }
		/// <summary>
		/// Date and time of post publishing
		/// </summary>
		public DateTime PublishingDateTime { get; }

		/// <summary>
		/// Refenrece to <see cref="RssFeed"/> that owns this entry
		/// </summary>
		public RssFeed Feed { get; }

		/// <summary>
		/// Retuns formatted <see cref="PublishingDateTime"/>. Simply removes date if post was made today
		/// </summary>
		public string FormatedPublushingDateTime
		{
			get
			{
				if (PublishingDateTime.Date == DateTime.Today)
				{
					return PublishingDateTime.TimeOfDay.ToString();
				}
				else
				{
					return PublishingDateTime.ToString();
				}
			}
		}

		/// <summary>
		/// <see cref="Description"/> without images and formatting, just text
		/// </summary>
		public string ShortDescription => GetPlainTextFromHtml(Description);

		/// <summary>
		/// Creates new rss entry
		/// </summary>
		/// <param name="title">Title of entry</param>
		/// <param name="description">Summary of entry. HTML or simple text</param>
		/// <param name="link">Link to entry in browser</param>
		/// <param name="publishingDateTime">Publishing date and time</param>
		/// <param name="feed">Parent rss feed</param>
		private RssEntry(string title, string description, string link, DateTime publishingDateTime, RssFeed feed)
		{
			Title = title;
			Description = description;
			Link = link;
			PublishingDateTime = publishingDateTime;
			Feed = feed;
		}

		/// <summary>
		/// Compares two rss entries by theirs publishing times
		/// </summary>
		/// <param name="other">RssEntry to cpmpare with</param>
		/// <returns>Returns 1 if <see cref="other"/> is older or equals to null. 
		/// Retuns 0 if both entries were published at the same time. 
		/// Otherwise -1</returns>
		public int CompareTo(RssEntry other)
		{
			return PublishingDateTime.CompareTo(other?.PublishingDateTime);
		}

		/// <summary>
		/// Removes any html tags from text
		/// </summary>
		/// <param name="htmlString">HTML string to clean up</param>
		/// <returns>Cleaned up string</returns>
		private string GetPlainTextFromHtml(string htmlString)
		{
			var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
			htmlString = regexCss.Replace(htmlString, string.Empty);
			htmlString = Regex.Replace(htmlString, "<.*?>", string.Empty);
			htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
			htmlString = htmlString.Replace("&nbsp;", string.Empty);

			return htmlString;
		}

		/// <summary>
		/// Generates rss entry from <see cref="SyndicationItem"/>.
		/// </summary>
		/// <param name="feed">Feed that owns entry</param>
		/// <param name="item"></param>
		/// <returns>new rss entry</returns>
		public static RssEntry FromSyndicationItem(RssFeed feed, SyndicationItem item)
		{
			return new RssEntry(
				item.Title?.Text ?? string.Empty,
				item.Summary?.Text ?? string.Empty,
				item.Links?.FirstOrDefault()?.Uri.ToString() ?? string.Empty,
				item.PublishDate.DateTime,
				feed
			);
		}
	}
}