using System.Collections.Generic;
using System.Collections.ObjectModel;
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



	}
}