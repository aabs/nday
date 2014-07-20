using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace nday
{

	class Entry
	{
		public Entry (string origin, DateTime date, DateTime time, string activity, string tags)
		{
			this.Origin = origin;
			this.ActivityDate = date;
			this.PreciseTime = time;
			this.Activity = activity;
			this.Tags = tags;
		}
		
		public string Origin {
			get;
			set;
		}

		public DateTime ActivityDate {
			get;
			set;
		}

		public DateTime PreciseTime {
			get;
			set;
		}

		public string Activity {
			get;
			set;
		}

		public string Tags {
			get;
			set;
		}
	}
}
