using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PowerLogViewer.BusinessObjects;

namespace PowerLogViewer
{
	/// <summary>
	/// Interaction logic for DatagridColumnConfig.xaml
	/// </summary>
	public partial class DatagridColumnConfig : Window
	{
		public ObservableCollection<DatagridColumnConfigSettings> SettingsList { get; private set; }
		public DatagridColumnConfig()
		{
			InitializeComponent();

		}

		public ObservableCollection<DatagridColumnConfigSettings> ShowColumnConfigDialog(ObservableCollection<DatagridColumnConfigSettings> settingsList)
		{
			SettingsList = settingsList;
			lbxColumnVisibility.ItemsSource = SettingsList;
			UpdateCheckUncheckAllCheckbox();
			this.ShowDialog();
			return settingsList;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (SettingsList.Count( x => x.IsChecked == true ) == 0)
			{
				MessageBox.Show( "There must be at least one column visible!", "Invalid selection", MessageBoxButton.OK, MessageBoxImage.Hand );
				e.Cancel = true;
			}
		}

		private void CheckBox_ValueChange(object sender, RoutedEventArgs e)
		{
			UpdateCheckUncheckAllCheckbox();
		}

		private void UpdateCheckUncheckAllCheckbox()
		{
			var elementsCount =  SettingsList.Count;
			var checkedElements = SettingsList.Count( x => x.IsChecked == true );
			// All checked
			if (elementsCount == checkedElements)
			{
				ckbxCheckUncheckAll.IsChecked = true;
				return;
			}

			// Nothing checked
			if (checkedElements == 0)
			{
				ckbxCheckUncheckAll.IsChecked = false;
				return;
			}
			// Mixed check states
			ckbxCheckUncheckAll.IsChecked = null;
		}

		private void ckbxCheckUncheckAll_Checked(object sender, RoutedEventArgs e)
		{
			var ckbx =  (CheckBox) sender;

			switch (ckbx.IsChecked	)
			{
				case null:
				case true:
					lbxColumnVisibility.ItemsSource = new ObservableCollection<DatagridColumnConfigSettings>( SettingsList.Select( x => { x.IsChecked = true; return x; } ).ToList() );
					break;
				case false:
					lbxColumnVisibility.ItemsSource = new ObservableCollection<DatagridColumnConfigSettings>( SettingsList.Select( x => { x.IsChecked = false; return x; } ).ToList() );
					break;
				default:
					break;
			}
		}

		private void btSave_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
