using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using PowerLogViewer.BusinessObjects;

namespace PowerLogViewer.Controller
{
	public class ApplicationCacheController
	{
		public ObservableCollection<LogEntry> FullLogEntryList { get; private set; }
		private List<LogEntry> _CompleteLogList;
		public ObservableCollection<Bookmark> BookmarkList { get; private set; }


		public ApplicationCacheController()
		{
			FullLogEntryList = new ObservableCollection<LogEntry>();
			BookmarkList = new ObservableCollection<Bookmark>();
		}

		public ApplicationCacheController(List<LogEntry> logEntrys)
		{
			_CompleteLogList = logEntrys;
			FullLogEntryList = new ObservableCollection<LogEntry>( logEntrys );
			BookmarkList = new ObservableCollection<Bookmark>();
		}

		public ObservableCollection<LogEntry> GetEntriesByDynamicLinqQuery(string query)
		{
			List<LogEntry> filtred;

			filtred = _CompleteLogList.AsQueryable().Where( query ).ToList();

			var tempObCol = new ObservableCollection<LogEntry>(filtred);
			return tempObCol;
		}

		public ObservableCollection<LogEntry> GetCompleteEntryList()
		{
			return new ObservableCollection<LogEntry>( _CompleteLogList );
		}

		public void AddBookmark(string hash, string titel, string description, string group)
		{
			var logEntry = FindItemByHash(hash);

			var bookmark = new Bookmark();
			bookmark.Hash = logEntry.Hash;
			bookmark.Title = titel;
			bookmark.Description = description;
			bookmark.LogEntryObject = logEntry;
			bookmark.Group = group;

			BookmarkList.Add( bookmark );
		}

		public ObservableCollection<Bookmark> RemoveBookmark(Bookmark bookmark)
		{
			BookmarkList.Remove( bookmark );
			return BookmarkList;
		}

		public LogEntry FindItemByHash(string hash)
		{
			return _CompleteLogList.Single( x => x.Hash == hash );
		}
	}
}