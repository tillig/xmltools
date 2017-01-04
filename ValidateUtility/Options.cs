using System;
using System.IO;
using System.Collections.Generic;

namespace ValidateUtility
{
	public class Options
	{
		public FileInfo File { get; set; }
		public string[] AdditionalSchemaPaths { get; set; }
	}
}
