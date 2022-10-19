using System;
using System.Security.Cryptography;
using System.Text;

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
		public string ProcessThread
		{ get
			{
				return String.Concat( Process.ToString(), "-", Thread.ToString() );
			}  
		}
		public string Session { get; set; }
		public string Message { get; set; }
		public string Attributes { get; set; }
		public int RunNumber { get; set; }
		public string FileName { get; set; }
		public string Hash
		{
			get
			{
				// Use input string to calculate MD5 hash
				using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
				{
					string input = String.Concat(TimeStamp.Ticks.ToString(),ProcessThread,Message,Attributes);
					byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
					byte[] hashBytes = md5.ComputeHash(inputBytes);
				
					return BitConverter.ToString( hashBytes ); 		
				}
			}
		}
		public LogEntry()
		{
			RunNumber = 0;
		}


	}
}