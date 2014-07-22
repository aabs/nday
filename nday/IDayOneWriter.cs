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
	public interface IDayOneWriter
	{
		void ProcessOutstandingEntries ();
	}

}

