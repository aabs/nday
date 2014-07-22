using System;
using log4net;

namespace nday
{
	
	public interface IConfigProvider
	{
		string DataFilePath {
			get;
			set;
		}
	}
}
