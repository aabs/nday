using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace nday
{

	class Entry
	{
		public Entry (string origin, string date, string activity, string tags)
		{
			this.Origin = origin;
			DateTime dt;
			if (DateTime.TryParse(date, out dt))
			{
				this.ActivityDate = dt;
			}
			else
			{
				this.ActivityDate = DateTime.Now;
			}
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
