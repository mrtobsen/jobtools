using System.Windows;
using Microsoft.Win32;
using PowerLogViewer.Controller;
using PowerLogViewer.EventArgsClasses;

namespace PowerLogViewer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private ApplicationCacheController _AppCache;

		public MainWindow()
		{
			InitializeComponent();
		}


		private void ImportData()
		{

			FileImportController im = new FileImportController();
			im.OpenFileDialog += OnOpenFileDialog;

			var tempList = im.ImportRawLogfiles();

			if (tempList == null)
			{
				return;
			}
			_AppCache = new ApplicationCacheController( tempList );
			dgLogentries.ItemsSource = _AppCache.FullLogEntryList;

		}

		// Opens file dialog when needed for Import (no implementation of Winforms into class)
		private void OnOpenFileDialog(object sender, ImportFileDialogEventArgs e)
		{
			FileImportController im =  (FileImportController)sender;
			OpenFileDialog dialog =  new OpenFileDialog();
			dialog.Filter = e.Filter;
			dialog.Multiselect = e.Multiselect;

			if (dialog.ShowDialog() != false)
			{
				im.Files = dialog.FileNames;
			}
		}

		private void miImportFile_Click(object sender, RoutedEventArgs e)
		{
			ImportData();
		}

		private void btDoSearch_Click(object sender, RoutedEventArgs e)
		{
			dgLogentries.ItemsSource = _AppCache.GetEntriesByDynamicLinqQuery( txtSearch.Text );
		}

		private void btResetSearch_Click(object sender, RoutedEventArgs e)
		{
			dgLogentries.ItemsSource = _AppCache.GetCompleteEntryList();
		}


	}
}
