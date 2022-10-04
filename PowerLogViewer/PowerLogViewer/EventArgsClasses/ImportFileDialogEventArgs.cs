using System;

namespace PowerLogViewer.EventArgsClasses
{
	public class ImportFileDialogEventArgs : EventArgs
	{
		public string Filter { get; set; }
		public bool Multiselect { get; set; }


	}
}