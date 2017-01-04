using System;
using System.IO;
using System.Xml;

namespace WellformedUtility
{
	public class Program
	{
		public static int Main(String[] args)
		{
			// Error codes:
			// 0 - Document is well-formed.
			// 1 - Document is not well-formed.
			// 2 - Command line argument parsing error.
			// 3 - Specified XML file not found.

			FileInfo file = null;
			try
			{
				file = ParseCommandLine(args);
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine("*** {0}", ex.Message);
				ShowHelp();
				return 2;
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine("*** Unable to find specified file: {0}", ex.FileName);
				ShowHelp();
				return 3;
			}

			XmlReaderSettings settings = BuildValidatorSettings();
			using (XmlReader reader = XmlReader.Create(file.FullName, settings))
			{
				try
				{
					while (reader.Read()) ;
					reader.Close();
					Console.WriteLine("Success: Document is well-formed.");
					return 0;
				}
				catch (XmlException e)
				{
					Console.WriteLine("### Error: " + e.Message);
					return 1;
				}
			}
		}

		public static void ShowHelp()
		{
			Console.WriteLine("XML Well-Formedness Checker");
			Console.WriteLine("Usage: " + Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + " filename");
		}

		public static FileInfo ParseCommandLine(string[] args)
		{
			if (args.Length != 1)
			{
				throw new ArgumentException("Incorrect number of arguments.");
			}
			FileInfo file = new FileInfo(args[0]);
			if (!file.Exists)
			{
				throw new FileNotFoundException("Unable to find specified XML file.", file.FullName);
			}
			return file;
		}

		public static XmlReaderSettings BuildValidatorSettings()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ConformanceLevel = ConformanceLevel.Auto;
			settings.IgnoreComments = false;
			settings.IgnoreProcessingInstructions = false;
			settings.IgnoreWhitespace = false;
			settings.ValidationType = ValidationType.None;
			settings.ValidationFlags = System.Xml.Schema.XmlSchemaValidationFlags.None;
			return settings;
		}
	}
}
