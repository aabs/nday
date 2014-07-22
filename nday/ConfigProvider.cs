using System;
using log4net;

namespace nday
{
	
	public class ConfigProvider:IConfigProvider
	{
		public ConfigProvider (string path)
		{
			DataFilePath = path;
		}

		#region IConfigProvider implementation
		public string DataFilePath
		{
			get;
			set;
		}
		#endregion
		
	}
}
