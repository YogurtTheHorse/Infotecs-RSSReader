using RSSReader.Properties;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RSSReader
{
	/// <summary>
	/// Interaction logic for Settings window
	/// </summary>
	public partial class SettingsWindow : Window
	{
		public SettingsWindow()
		{
			InitializeComponent();

			FeedsTextBox.Text = string.Join("\n", Settings.Default.Feeds.Cast<string>().Where(s => s != string.Empty));
		}

		private void SaveSettings(object sender, RoutedEventArgs e)
		{
			IEnumerable<string> newFeeds = FeedsTextBox.Text.Split('\n', '\r').Where(s => s != string.Empty);
			string invalidUri = newFeeds.FirstOrDefault(s => !Uri.IsWellFormedUriString(s, UriKind.Absolute));

			if (invalidUri != null)
			{
				MessageBox.Show(string.Format(Properties.Resources.Settings_InvalidUri, invalidUri),
								Properties.Resources.Settings_Error,
								MessageBoxButton.OK,
								MessageBoxImage.Error);
				return;
			}

			if (!UpdateIntervalControl.Value.HasValue || UpdateIntervalControl.Value.Value < new TimeSpan(0, 0, 1))
			{
				MessageBox.Show(Properties.Resources.Settings_InvalidUpdateInterval,
								Properties.Resources.Settings_Error,
								MessageBoxButton.OK,
								MessageBoxImage.Error);
				return;
			}

			Settings.Default.Feeds.Clear();
			Settings.Default.Feeds.AddRange(newFeeds.ToArray());
			for (int i = Settings.Default.DisabledFeeds.Count - 1; i >= 0; i--)
			{
				if (!newFeeds.Contains(Settings.Default.DisabledFeeds[i]))
				{
					Settings.Default.DisabledFeeds.RemoveAt(i);
				}
			}
			Settings.Default.UpdateInterval = UpdateIntervalControl.Value.Value;
			SettingsExtensions.Save(this);
			Close();
		}
	}
}
