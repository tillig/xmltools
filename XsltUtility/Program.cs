using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace XsltUtility
{
	public class Program
	{
		public static int Main(String[] args)
		{
			// Error codes:
			// 0 - Transformation success.
			// 1 - Transformation error.
			// 2 - Command line argument parsing error.
			// 3 - Specified XML/XSLT file not found.

			if (args.Length < 1 || args.Length > 3)
			{
				ShowHelp();
				return 2;
			}

			FileInfo xmlFileInfo = new FileInfo(args[0]);
			if (!xmlFileInfo.Exists)
			{
				Console.WriteLine("*** Specified XML file not found.");
				ShowHelp();
				return 3;
			}
			XPathDocument xml = null;
			try
			{
				xml = new XPathDocument(xmlFileInfo.FullName);
			}
			catch (XmlException ex)
			{
				Console.WriteLine("*** Error loading XML: {0}", ex.Message);
				return 1;
			}

			FileInfo xslFileInfo = null;
			if (args.Length > 1)
			{
				xslFileInfo = new FileInfo(args[1]);
			}
			else
			{
				xslFileInfo = GetXsltFromProcessingInstruction(xml, xmlFileInfo.DirectoryName);
			}
			if (xslFileInfo == null || !xslFileInfo.Exists)
			{
				Console.WriteLine("*** XSL file not found.");
				ShowHelp();
				return 3;
			}

			XslCompiledTransform xsl = new XslCompiledTransform();
			try
			{
				xsl.Load(xslFileInfo.FullName);
			}
			catch (XsltException ex)
			{
				Console.WriteLine("*** Error loading XSLT: {0}", ex.Message);
				return 1;
			}

			XmlTextWriter writer = null;
			try
			{
				if (args.Length == 3)
				{
					writer = new XmlTextWriter(args[2], null);
				}
				else
				{
					writer = new XmlTextWriter(Console.Out);
				}
				xsl.Transform(xml, writer);
				return 0;
			}
			finally
			{
				if (writer != null)
				{
					((IDisposable)writer).Dispose();
				}
			}
		}

		public static void ShowHelp()
		{
			Console.WriteLine("XSLT Transformation Utility");
			Console.WriteLine("Usage: " + Path.GetFileName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase) + " sourcefile [xsltfile] [outdoc]");
			Console.WriteLine("Note:  If you want to specify an outfile, you must specify an xsltfile.");
		}

		public static FileInfo GetXsltFromProcessingInstruction(IXPathNavigable document, string basePath)
		{
			if (document == null)
			{
				throw new ArgumentNullException("document");
			}
			if (basePath == null)
			{
				throw new ArgumentNullException("basePath");
			}
			if (basePath.Length == 0)
			{
				throw new ArgumentException("Base path for inferred XSLT may not be empty.", "basePath");
			}
			XPathNavigator navigator = document.CreateNavigator();
			XPathNodeIterator iterator = navigator.SelectChildren(XPathNodeType.ProcessingInstruction);
			Regex hrefRegex = new Regex("href\\s*=\\s*([\'\"])([^\\1]+?)\\1", RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase);
			while (iterator.MoveNext())
			{
				if (iterator.Current.Name != "xml-stylesheet")
				{
					continue;
				}
				Match hrefMatch = hrefRegex.Match(iterator.Current.Value);
				if (hrefMatch.Success && hrefMatch.Groups.Count > 2)
				{
					return new FileInfo(Path.Combine(basePath, hrefMatch.Groups[2].Captures[0].Value));
				}
			}
			return null;
		}
	}
}
