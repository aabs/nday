using System;
using log4net;
using System.Collections.Generic;

namespace nday
{
	public interface IEntryReader
	{
		IEnumerable<Entry> ReadOutstandingEntries ();
	}
}

