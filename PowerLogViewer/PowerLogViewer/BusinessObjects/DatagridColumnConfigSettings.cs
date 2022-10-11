using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerLogViewer.BusinessObjects
{
	public class DatagridColumnConfigSettings
	{
		public int Index { get; set; }
		public int DisplayIndex { get; set; }
		public string Header { get; set; }
		public bool? IsChecked { get; set; }		
	}
}
