using System;
using log4net;
using System.IO;
using System.Xml.Linq;

namespace nday
{
	
	public class XmlEntryReader : IEntryReader
	{

		IConfigProvider config;
		ILog logger;

		public XmlEntryReader (IConfigProvider config, ILog logger)
		{
			this.logger = logger;
			this.config = config;
		}
		#region IEntryReader implementation

		public System.Collections.Generic.IEnumerable<Entry> ReadOutstandingEntries ()
		{
			logger.DebugFormat ("Searching for new entries (in {0})", config.DataFilePath);
			var s = File.ReadAllText (config.DataFilePath);
			// the replace here is needed to get around IFTTT's XSS controls that disallow XML
			var xml = "<Entries>"+s.Replace('~', '<').Replace('`', '>')+"</Entries>";
			var doc = XDocument.Parse (xml);
			foreach(var xe in doc.Root.Elements("Entry")){
				string origin = xe.Attribute ("origin").Value;
				string date = xe.Attribute ("date").Value;
				string tags = xe.Attribute ("tags").Value;
				string activity = xe.Value;
				yield return new Entry (origin, date, activity, tags);
			}
		}

		#endregion


	}
}
