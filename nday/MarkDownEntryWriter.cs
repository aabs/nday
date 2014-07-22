using System;
using System.Linq;
using log4net;
using System.Collections.Generic;
using System.Diagnostics;

namespace nday
{
	
	public class MarkDownEntryWriter : IEntryWriter
	{
		ILog logger;

		public MarkDownEntryWriter (ILog logger)
		{
			this.logger = logger;
		}		

		public void WriteEntries(DateTime entryDate, Dictionary<string, List<Entry>> entries)
		{
			Process ps = new Process ();
			ps.StartInfo.UseShellExecute = false;
			ps.StartInfo.FileName = "/usr/local/bin/dayone";
			ps.StartInfo.CreateNoWindow = true;
			//var processArguments = "--date='" + entryDate.ToString ("d") + "' new";
			ps.StartInfo.Arguments = "new";
			ps.StartInfo.RedirectStandardInput = true;
			ps.Start ();
			var log = ps.StandardInput;
			try
			{
				logger.Debug ("Writing Entries");
				foreach (var o in entries.Keys)
				{
					log.WriteLine ("\n### {0} activity\n", o);
					foreach (var e in entries[o])
					{
						log.WriteLine ("**{0}:**\n{1}\nTags: {2}", 
							entryDate.ToString ("t"), 
							e.Activity, 
							string.Join(", ", e.Tags.Select((arg) => "#" + arg.Replace("#",""))));
					}
				}
			} catch (Exception ex)
			{
				logger.Error ("error while writing entries", ex);
			}
		}
	}

}
