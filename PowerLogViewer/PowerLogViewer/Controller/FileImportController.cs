using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using PowerLogViewer.BusinessObjects;
using PowerLogViewer.EventArgsClasses;

namespace PowerLogViewer.Controller
{
	public class FileImportController
	{
		public event EventHandler<ImportFileDialogEventArgs> OpenFileDialog; // No implementation of Filedialog in this class
		public string[] Files { get; set; }

		public string DialogFilter { get; set; }
		public bool DialogMultiselect { get; set; }



		protected virtual void OnOpenFileDialog()
		{
			if (OpenFileDialog != null)
			{
				DialogFilter = "EVServer logs( *.xml, *.rxml) | *.xml; *.rxml";
				DialogMultiselect = true;
				OpenFileDialog( this, new ImportFileDialogEventArgs() { Filter = DialogFilter, Multiselect = DialogMultiselect } );
			}
		}
		public List<LogEntry> ImportRawLogfiles()
		{
			var logEntryList= new List<LogEntry>();
			OnOpenFileDialog();

			string[] repFilePathes =  ReplaceWhitespaces( Files );
			logEntryList = GetDataFromRepairedLogFiles( repFilePathes );

			return logEntryList;
		}


		private List<LogEntry> GetDataFromRepairedLogFiles(string[] files)
		{
			ConcurrentBag < List < LogEntry > >tempBag = new ConcurrentBag<List<LogEntry>>();

			Parallel.ForEach( files, file => {
				tempBag.Add( ParseXML( file ) );
			} );

			var tempList = new List<LogEntry>();
			foreach (var list in tempBag)
			{
				tempList.AddRange( list );
			}

			return tempList;
		}


		/// <summary>
		/// Repairs invalid XML. Caused by invalid format, when some kind of SVG informations are logged.
		/// </summary>
		/// <param name="sourceFilePath"></param>
		/// <param name="repairedFilePath"></param>
		private string[] ReplaceWhitespaces(string[] sourceFiles)
		{
			string[]repFilesPathes = new string[sourceFiles.Length];
			int i = 0;


			foreach (var file in sourceFiles) // ToDo: Evtl. auch Parallelisieren
			{
				if (Path.GetExtension( file ) == ".rxml")
				{
					// Already repaired => skip repair procedure
					repFilesPathes[i] = file;
					i++;
					continue;
				}

				// Create a new list with "rxml" extension => needed later for parllel importing
				string repairedFilePath = file;
				repairedFilePath = repairedFilePath.Replace( ".xml", ".rxml" );
				repFilesPathes[i] = repairedFilePath;
				i++;

				if (File.Exists( repairedFilePath ))
				{

					if (MessageBox.Show( "There is already a repaired file of this log. Do you want to overwrite it? ", "File already exists", MessageBoxButton.YesNo ) == MessageBoxResult.No) // Todo: Richtige stelle hier? Braucht winforms
					{
						continue;
					}
				}

				var lines = File.ReadAllLines(file);

				string[] newString = new string[lines.Length];

				String pattern = @"(?=<[^=]+?>)(?=</?\w+\s+\w+)(<.*?)(\s+)(.*?>)";
				Regex reg = new Regex(pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase| RegexOptions.Compiled ); //https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices

				Parallel.ForEach( lines, (line, pls, index) => {
					while (Regex.IsMatch( line, pattern ))
					{
						line = reg.Replace( line, @"$1_$3" );
					}
					newString[index] = line;
				} );

				File.WriteAllLines( repairedFilePath, newString );
			}

			return repFilesPathes;

		}

		/// <summary>
		/// Parse Logfiles and return them as a list.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static List<LogEntry> ParseXML(string path)
		{
			XmlTextReader reader = null;
			reader = new XmlTextReader( path );
			reader.WhitespaceHandling = WhitespaceHandling.None;
			List<LogEntry> list = new List<LogEntry>();
			string[]dateFormats = {"d-M-yyyy","M/d/yyyy", "dd/MM/yyyy", "dd.MM.yyyy" }; // ToDo: in Config
			int runNumber = 0; // used to indicate multiple server runs
			string fileName = Path.GetFileName(path);
			// Let's rock
			while (reader.Read())
			{
				if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "logEntry"))
				{
					LogEntry temp = new LogEntry();
					temp.Level = reader.GetAttribute( "level" );
					temp.Category = reader.GetAttribute( "category" );
					temp.Component = reader.GetAttribute( "component" );


					// Combine date and time to one field
					temp.TimeStamp = DateTime.ParseExact( reader.GetAttribute( "date" ), dateFormats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None );
					temp.TimeStamp = temp.TimeStamp.Add( TimeSpan.Parse( reader.GetAttribute( "time" ) ) );

					temp.Thread = int.Parse( reader.GetAttribute( "thread" ) );
					temp.Process = int.Parse( reader.GetAttribute( "process" ) );
					temp.Session = reader.GetAttribute( "session" );
					temp.Attributes = string.Empty;
					// Message and attributes
					while (reader.Read())
					{
						if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == "logEntry"))
						{
							temp.FileName = fileName;
							list.Add( temp );
							break;
						}

						// Increase runnumber as a service restart message was found
						if ((reader.NodeType == XmlNodeType.Element) && (reader.Name == "message"))
						{
							temp.Message = reader.ReadString();
							if (temp.Message.Contains( " (64-bit)" ))
							{
								runNumber++;
							}
							temp.RunNumber = runNumber;

						}
						if ((reader.NodeType == XmlNodeType.Element) && (reader.Name != "attributes"))
						{
							//temp.Attributes += reader.Name + ": " + reader.ReadString() ;
							if (true)
							{
								if (!String.IsNullOrEmpty( temp.Attributes ))
								{
									temp.Attributes += Environment.NewLine;
								}
								temp.Attributes = reader.Name + ": " + reader.ReadString();
							}

						}
					}
				}
			}

			return list;
		}
	}
}
