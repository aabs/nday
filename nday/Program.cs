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
	class MainClass
	{
		static IContainer container ;

		static void BuildDependencyGraph (string path)
		{
			var builder = new ContainerBuilder();
			builder.RegisterInstance (new ConfigProvider (path)).As<IConfigProvider> ();
			builder.RegisterInstance(LogManager.GetLogger ("nday")).As<ILog>();
			builder.RegisterType<XmlEntryReader>().As<IEntryReader>();
			builder.RegisterType<MarkDownEntryWriter>().As<IEntryWriter>();
			builder.RegisterType<MarkDownEntryWriter>().As<IEntryWriter>();
			builder.RegisterType<DayOneWriter>().As<IDayOneWriter>();
			container = builder.Build();
		}

		public static void Main (string[] args)
		{
			XmlConfigurator.Configure ();
			BuildDependencyGraph (args[0]);
			var logger = container.Resolve<ILog> ();
			logger.Info ("NDay Running");

			if (args.Length != 1)
			{
				displayUsage ();
				return;
			}

			var dow = container.Resolve<IDayOneWriter> ();
			dow.ProcessOutstandingEntries ();
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

	}
}
