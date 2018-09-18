using RSSReader.Properties;

using System;

namespace RSSReader
{
	/// <summary>
	/// Tiny class for handling <see cref="Settings.Default"/> change
	/// </summary>
	public static class SettingsExtensions
	{
		public delegate void SettingsSaved(object sender, EventArgs e);

		/// <summary>
		/// Event that calls after settings saving when calling <see cref="Save(object)"/>
		/// </summary>
		public static event SettingsSaved OnSettingsSaved;

		/// <summary>
		/// Saves settings and invokes <see cref="OnSettingsSaved"/>
		/// </summary>
		/// <param name="sender">Author of the save</param>
		public static void Save(object sender)
		{
			Settings.Default.Save();
			OnSettingsSaved?.Invoke(sender, new EventArgs());
		}
	}
}
