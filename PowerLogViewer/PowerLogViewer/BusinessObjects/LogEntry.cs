using System;

namespace PowerLogViewer.BusinessObjects
{
	public class LogEntry
	{
		public string Level { get; set; }
		public string Category { get; set; }
		public string Component { get; set; }
		public DateTime TimeStamp { get; set; }
		public int Thread { get; set; }
		public int Process { get; set; }
		public string Session { get; set; }
		public string Message { get; set; }
		public string Attributes { get; set; }
		public int RunNumber { get; set; }
		public string FileName { get; set; }

		public LogEntry()
		{
			RunNumber = 0;
		}


	}
}