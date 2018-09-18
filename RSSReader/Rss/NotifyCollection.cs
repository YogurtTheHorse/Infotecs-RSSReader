using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace RSSReader.Rss
{
	/// <summary>
	/// Custom interface for hiding methods that edits collection, so in public API noone will see them
	/// </summary>
	public interface IReadOnlyNotifyCollection<T> : INotifyCollectionChanged, INotifyPropertyChanged, IReadOnlyCollection<T> { }

	/// <summary>
	/// Just <see cref="ObservableCollection{T}"/> but with <see cref="IReadOnlyNotifyCollection{T}"/> implementing (meh)
	/// </summary>
	public class NotifyCollection<T> : ObservableCollection<T>, IReadOnlyNotifyCollection<T> { }
}
