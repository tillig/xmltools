using System;
using System.IO;
using System.Collections.Generic;

namespace XPathUtility
{
	public class Program
	{
		public static int Main(String[] args)
		{
			// Error codes:
			// 0 - XPath expression found.
			// 1 - XPath expression not found.
			// 2 - Command line argument parsing error.
			// 3 - Specified XML file not found.

			if (args.Length != 2)
			{
				ShowHelp();
				return 2;
			}

			XPathEvaluator evaluator = null;
			try
			{
				FileInfo file = new FileInfo(args[0]);
				evaluator = new XPathEvaluator(file);
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine("*** {0}", ex.Message);
				ShowHelp();
				return 2;
			}
			catch (FileNotFoundException ex)
			{
				Console.WriteLine("*** {0}", ex.Message);
				ShowHelp();
				return 3;
			}

			IList<string> results = null;
			try
			{
				results = evaluator.Evaluate(args[1]);
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine("*** {0}", ex.Message);
				ShowHelp();
				return 2;
			}

			if (results.Count == 0)
			{
				Console.WriteLine("*** XPath expression not found in document.");
				return 1;
			}

			if(results.Count == 1)
			{
				Console.WriteLine("*** Found 1 match:");
				Console.WriteLine(results[0]);
			}
			else
			{
				Console.WriteLine("*** Found {0} matches:", results.Count);
				Console.WriteLine();
				for (int i = 0; i < results.Count; i++)
				{
					Console.WriteLine("----- {0} -----", i);
					Console.WriteLine();
					Console.WriteLine(results[i]);
					Console.WriteLine();
				}
				Console.WriteLine("----- end -----");
			}

			return 0;
		}

		public static void ShowHelp()
		{
			Console.WriteLine("XPath Expression Evaluator");
			Console.WriteLine("Usage: " + Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + " filename xpathexpr");
		}
	}
}





