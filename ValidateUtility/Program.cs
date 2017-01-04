using System;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Reflection;

namespace ValidateUtility
{
	public class Program
	{
		public static int Main(String[] args)
		{
			// Error codes:
			// 0 - Document is valid.
			// 1 - Document is not valid.
			// 2 - Command line argument parsing error.
			// 3 - Specified XML file not found.

			Options options = null;
			try
			{
				options = ParseCommandLine(args);
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine("*** {0}", ex.Message);
				ShowHelp();
				return 2;
			}

			XmlValidator validator = null;
			try
			{
				validator = new XmlValidator(options.File, options.AdditionalSchemaPaths);
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine("*** Unable to find specified file: {0}", ex.FileName);
				ShowHelp();
				return 3;
			}

			if (validator.Validate())
			{
				Console.WriteLine("Success: Document is valid.");
				return 0;
			}
			Console.WriteLine("*** Validation errors in document:");
			foreach (string error in validator.ValidationErrors)
			{
				Console.WriteLine("* {0}", error);
			}
			return 1;
		}

		public static Options ParseCommandLine(string[] args)
		{
			if (args.Length < 1)
			{
				throw new ArgumentException("Incorrect number of arguments.");
			}
			Options options = new Options();
			options.File = new FileInfo(args[0]);
			if (args.Length > 1)
			{
				var addlPaths = new string[args.Length - 1];
				for (int i = 1; i < args.Length; i++)
				{
					addlPaths[i - 1] = args[i];
				}
				options.AdditionalSchemaPaths = addlPaths;
			}
			return options;
		}

		public static void ShowHelp()
		{
			Console.WriteLine("XML Validation Utility");
			Console.WriteLine("Usage: " + Path.GetFileName(Assembly.GetExecutingAssembly().CodeBase) + " filename [schema1.xsd schema2.xsd *.xsd ...]");
			Console.WriteLine("Declare your schema by marking up your root element:");
			Console.WriteLine("<root xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:noNamespaceSchemaLocation=\"Schema.xsd\">");
		}
	}
}
