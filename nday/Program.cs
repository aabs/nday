using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace nday
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			if (args.Length != 1)
			{
				displayUsage ();
			}

			var entries = getEntries (args [0]);

			var entriesByDate = entries
				.GroupBy (x => x.ActivityDate)
				.ToDictionary (grp => grp.Key, grp => grp.ToList ());

			foreach (var entryDate in entriesByDate.Keys)
			{
				var entriesByOrigin = entriesByDate [entryDate]
					.GroupBy (x => x.Origin)
					.ToDictionary (grp => grp.Key, grp => grp.ToList ());

				Process ps = new Process ();
				ps.StartInfo.UseShellExecute = false;
				ps.StartInfo.FileName = "/usr/local/bin/dayone";
				ps.StartInfo.CreateNoWindow = true;
				//var processArguments = "--date='" + entryDate.ToString ("d") + "' new";
				ps.StartInfo.Arguments = "new";
				ps.StartInfo.RedirectStandardInput = true;
				ps.Start();
				var log = ps.StandardInput;
				try
				{
					foreach (var o in entriesByOrigin.Keys)
					{
						log.WriteLine ("\n## Activity on {0}\n", o);
						foreach (var e in entriesByOrigin[o])
						{
							log.WriteLine ("* {0} {1} @done", entryDate.ToString ("t"), e.Activity);
						}
					}
				} catch (Exception ex)
				{
					Console.WriteLine (ex.Message);					
				}
				//var invocation = "-c /usr/local/bin/dayone new < " + args [0] + ".ingest.txt";
				//Console.WriteLine ("Running " + invocation);
			}

			File.Move (args [0], args [0] + DateTime.Now.ToString ("s").Replace (':', '_') + ".bak");
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

		static IEnumerable<Entry> getEntries (string pathToActivityRecord)
		{
			var re = new Regex (@"\((?<origin>[^\)]+)\)\((?<date>[^\)]+)\)\((?<activity>[^\)]+)\)\((?<tags>[^\)]+)\)");
			var lines = File.ReadAllLines (pathToActivityRecord);
			foreach (var line in lines)
			{
				if (re.IsMatch (line))
				{
					MatchCollection matches = re.Matches (line);
					foreach (Match match in matches)
					{
						GroupCollection groups = match.Groups;
						var origin = groups ["origin"].Value;
						var date = groups ["date"].Value;
						var activity = groups ["activity"].Value;
						var tags = groups ["tags"].Value;
						DateTime dt = DateTime.Today;
						DateTime.TryParse (date, out dt);
						yield return new Entry (origin, dt.Date, dt, activity, tags);
					}
				}
			}
		}

	}

}
