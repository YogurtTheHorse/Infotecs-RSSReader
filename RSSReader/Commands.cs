using System;
using System.Windows;
using System.Windows.Input;

namespace RSSReader
{
	/// <summary>
	/// Static class for custom commands
	/// </summary>
	/// <seealso cref="ICommand"/>
	public static class Commands
	{
		private static readonly ICommand _applicationCloseCommand = new ApplicationCloseCommand();
		private static readonly ICommand _openSettingsCommand = new OpenSettingsCommand();

		/// <summary>
		/// Command for app closing
		/// </summary>
		public static ICommand ApplicationCloseCommand => _applicationCloseCommand;
		/// <summary>
		/// Command for opening setings window
		/// </summary>
		public static ICommand OpenSettingsCommand => _openSettingsCommand;
	}

	/// <summary>
	/// Implementation of <see cref="ICommand"/> that closes application
	/// </summary>
	public class ApplicationCloseCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Allows execution only when no magic happens with current application
		/// Probably it useless
		/// </summary>
		public bool CanExecute(object parameter)
		{
			return Application.Current != null && Application.Current.MainWindow != null;
		}

		/// <summary>
		/// Cloeses app
		/// </summary>
		/// <param name="parameter"></param>
		public void Execute(object parameter)
		{
			Application.Current.MainWindow.Close();
		}
	}

	/// <summary>
	/// Implementation of <see cref="ICommand"/> that opens settings window
	/// </summary>
	public class OpenSettingsCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		/// <summary>
		/// Allows execution only when no magic happens with current application
		/// Probably it useless
		/// </summary>
		public bool CanExecute(object parameter)
		{
			return Application.Current != null && Application.Current.MainWindow != null;
		}

		/// <summary>
		/// Opens Settings window as dialog (blocks main window while settings window is open)
		/// </summary>
		public void Execute(object parameter)
		{
			SettingsWindow settingsWindow = new SettingsWindow
			{
				ShowInTaskbar = false,
				Owner = Application.Current.MainWindow
			};
			settingsWindow.ShowDialog();

		}
	}
}
