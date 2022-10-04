using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Dynamic.Core;
using PowerLogViewer.BusinessObjects;

namespace PowerLogViewer.Controller
{
	class ApplicationCacheController
	{
		public ObservableCollection<LogEntry> FullLogEntryList { get; private set; }
		private List<LogEntry> _CompleteLogList;



		public ApplicationCacheController(List<LogEntry> logEntrys)
		{

			_CompleteLogList = logEntrys;
			FullLogEntryList = new ObservableCollection<LogEntry>( logEntrys );
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
	}
}