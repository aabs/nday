using System;
using log4net;
using System.Collections.Generic;

namespace nday
{
	public interface IEntryWriter
	{
		void WriteEntries(DateTime entryDate, Dictionary<string, List<Entry>> entries);
	}

}

