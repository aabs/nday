using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using log4net;
using log4net.Config;
using System.Text;
using System.Xml;
using System.Xml.Linq;


namespace nday
{
	class MainClass
	{
		static ILog logger = LogManager.GetLogger (typeof(MainClass));

		static MainClass ()
		{
			XmlConfigurator.Configure ();
		}

		public static void Main (string[] args)
		{
			logger.Info ("NDay Running");
			logger.InfoFormat ("Checking data file at '{0}'", args [0]);

			if (args.Length != 1)
			{
				displayUsage ();
			}

			logger.Debug ("Searching for new entries");
			var entries = getEntriesXml (args [0]);
			logger.DebugFormat ("Found {0} entries for insertion", entries.Count ());

			var entriesByDate = entries
				.GroupBy (x => x.ActivityDate)
				.ToDictionary (grp => grp.Key, grp => grp.ToList ());

			logger.DebugFormat ("Found entries relating to {0} day(s)", entriesByDate.Count);

			foreach (var entryDate in entriesByDate.Keys)
			{
				logger.DebugFormat ("processing entries for {0}", entryDate);

				var entriesByOrigin = entriesByDate [entryDate]
					.GroupBy (x => x.Origin)
					.ToDictionary (grp => grp.Key, grp => grp.ToList ());
				logger.DebugFormat ("Found entries from {0} origins for {1}", entriesByOrigin.Count, entryDate);

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
					foreach (var o in entriesByOrigin.Keys)
					{
						log.WriteLine ("\n### {0} activity\n", o);
						foreach (var e in entriesByOrigin[o])
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

			var bak = args [0] + "." + DateTime.Now.ToString ("s").Replace (':', '_') + ".txt";
			logger.InfoFormat ("Backing up data file to '{0}'", bak);
			File.Move (args [0], bak);
			logger.Info ("Done");
		}

		static void displayUsage ()
		{
			Console.WriteLine (@"
N Day 
======================

A structured import utility for creating Day One entries from text files.

How it works
----------------------

d-day is intended to work in batch mode.  Every now and then, invoke it 
and it will look for the file passed as parameter.

Usage:  nday datafile

It only expects a single file, since it is able to work out where the file contents
came from.

Entries should follow this format:

{origin}{date}{activity}{tags}

one entry per line.  Any lines that do not match this pattern are ignored.

Once the file has been read and the entries created in dayone, the file will be 
renamed to show when the import was done. So if your file is called ""activity.txt""
then it will be renamed to ""activity.txt-imported20140718.txt"".
");
		}
			
		static IEnumerable<Entry> getEntriesXml(string pathToActivityRecord){
			var s = File.ReadAllText (pathToActivityRecord);
			var xml = "<Entries>"+s+"</Entries>";
			var doc = XDocument.Parse (xml);
			foreach(var xe in doc.Root.Elements("Entry")){
				string origin = xe.Attribute ("origin").Value;
				string date = xe.Attribute ("date").Value;
				string tags = xe.Attribute ("tags").Value;
				string activity = xe.Value;
				yield return new Entry (origin, date, activity, tags);
			}
		}
	}
}
