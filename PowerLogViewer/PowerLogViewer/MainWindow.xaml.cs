using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;
using Microsoft.Win32;
using PowerLogViewer.BusinessObjects;
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
			this.DataContext = _AppCache;

			dgLogentries.ItemsSource = _AppCache.FullLogEntryList;			
			
			btDoSearch.IsEnabled = true;
			btReset.IsEnabled = true;
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

		private void DataGridColumnHeader_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			ObservableCollection< DatagridColumnConfigSettings > colConfigs= new ObservableCollection<DatagridColumnConfigSettings>();
			int i = 0;
			foreach (var col in dgLogentries.Columns)
			{
				var tempConf = new DatagridColumnConfigSettings();
				tempConf.Index = i;
				i++;
				tempConf.DisplayIndex = col.DisplayIndex;
				tempConf.Header = col.Header.ToString();
				switch (col.Visibility)
				{
					case Visibility.Visible:
						tempConf.IsChecked = true;
						break;
					case Visibility.Hidden:
					case Visibility.Collapsed:
						tempConf.IsChecked = false;
						break;
					default:
						break;
				}
				colConfigs.Add( tempConf );
			}
			// Order list to be align with the order in the Datagrid
			colConfigs = new ObservableCollection<DatagridColumnConfigSettings>( colConfigs.OrderBy( x => x.DisplayIndex ).ToList() );
			var colSettingsDialog = new DatagridColumnConfig();
			var newCollSettings = colSettingsDialog.ShowColumnConfigDialog( colConfigs );

			// Update visibilty
			foreach (var colSetting in newCollSettings)
			{
				dgLogentries.Columns[colSetting.Index].Visibility = colSetting.IsChecked == true ? Visibility.Visible : Visibility.Hidden;
			}
		}

		private void AddBookmark_Click(object sender, RoutedEventArgs e)
		{
			if (dgLogentries.CurrentItem != null)
			{
				var currentItem = (LogEntry)dgLogentries.CurrentItem;
				_AppCache.AddBookmark( currentItem.Hash, currentItem.TimeStamp.ToString() + ": " + currentItem.Message.Replace( Environment.NewLine, " " ), currentItem.Message, "User" );
			}
		}

		private void JumpToHash(string hash)
		{
			LogEntry itemToSelect = _AppCache.FindItemByHash(hash );
			dgLogentries.SelectedItem = itemToSelect;
			dgLogentries.ScrollIntoView( itemToSelect );
			dgLogentries.UpdateLayout();
		}

		private void trvwBookmarks_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			var treeView = (TreeView) sender;
			if (treeView.SelectedItem.GetType() == typeof(Bookmark))
			{
				var bookmark =(Bookmark) treeView.SelectedItem;
				if (bookmark != null)
				{
					JumpToHash( bookmark.Hash );
				}
			}
		}
	}
}
