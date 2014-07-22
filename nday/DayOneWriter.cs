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
using Autofac;

namespace nday
{
	public class DayOneWriter : IDayOneWriter
	{

		public ILog logger;
		public IEntryReader Reader
		{
			get;
			set;
		}
		public IEntryWriter Writer
		{
			get;
			set;
		}
		public IConfigProvider Config
		{
			get;
			set;
		}

		public DayOneWriter (ILog logger, IEntryReader reader, IEntryWriter writer, IConfigProvider config)
		{
			this.Config = config;
			if (writer == null)
				throw new ArgumentNullException ("writer");
			if (reader == null)
				throw new ArgumentNullException ("reader");
			if (logger == null)
				throw new ArgumentNullException ("logger");
			this.Writer = writer;
			this.Reader = reader;
			this.logger = logger;
		}

		public void ProcessOutstandingEntries(){
			logger.InfoFormat ("Checking data file at '{0}'", Config.DataFilePath);
			var entries = Reader.ReadOutstandingEntries ();

			var entriesByDate = entries
				.GroupBy (x => x.ActivityDate)
				.ToDictionary (grp => grp.Key, grp => grp.ToList ());

			logger.DebugFormat ("Found entries relating to {0} day(s)", entriesByDate.Count);

			foreach (var entryDate in entriesByDate.Keys)
			{
				logger.DebugFormat ("processing entries for {0}", entryDate);

				Dictionary<string, List<Entry>> entriesByOrigin = entriesByDate [entryDate]
					.GroupBy (x => x.Origin)
					.ToDictionary (grp => grp.Key, grp => grp.ToList ());
				logger.DebugFormat ("Found entries from {0} origins for {1}", entriesByOrigin.Count, entryDate);
				Writer.WriteEntries (entryDate, entriesByOrigin);

			}

			BackupAndEraseDataFile ();

		}

		void BackupAndEraseDataFile ()
		{
			var bak = Config.DataFilePath + "." + DateTime.Now.ToString ("s").Replace (':', '_') + ".txt";
			logger.InfoFormat ("Backing up data file to '{0}'", bak);
			File.Move (Config.DataFilePath, bak);
		}
	}
}

