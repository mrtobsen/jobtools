using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace PowerLogViewer.BusinessObjects
{
	public class Bookmark
	{
		public string Hash { get; set; }
		public string Group { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public LogEntry LogEntryObject { get; set; }	
	}
}
