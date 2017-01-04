using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace XPathUtility
{
	public class XPathEvaluator
	{
		public XPathDocument Document { get; private set; }

		public XPathEvaluator(FileInfo xmlDocument)
		{
			if (xmlDocument == null)
			{
				throw new ArgumentNullException("xmlDocument");
			}
			if (!xmlDocument.Exists)
			{
				throw new FileNotFoundException("Unable to find specified XML document.", xmlDocument.FullName);
			}

			try
			{
				this.Document = new XPathDocument(xmlDocument.FullName);
			}
			catch (XmlException ex)
			{
				throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Error reading XML from document at {0}: {1}", xmlDocument.FullName, ex.Message), "xmlDocument");
			}
		}

		public IList<string> Evaluate(string xpath)
		{
			if (xpath == null)
			{
				throw new ArgumentNullException("xpath");
			}
			if (xpath.Length == 0)
			{
				throw new ArgumentException("XPath expression may not be empty.", "xpath");
			}

			XPathExpression expression = null;
			try
			{
				expression = XPathExpression.Compile(xpath);
			}
			catch (XPathException ex)
			{
				throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "XPath expression is not valid: {0}", ex.Message), "xpath");
			}

			var results = new List<string>();
			var navigator = this.Document.CreateNavigator();
			var iterator = navigator.Select(expression);
			while (iterator.MoveNext())
			{
					switch (iterator.Current.NodeType)
					{
						case XPathNodeType.Text:
						case XPathNodeType.Whitespace:
						case XPathNodeType.SignificantWhitespace:
						case XPathNodeType.Attribute:
							results.Add(iterator.Current.Value);
							break;
						default:
							using(StringWriter swriter = new StringWriter())
							using (XmlTextWriter xwriter = new XmlTextWriter(swriter))
							{
								xwriter.Formatting = Formatting.Indented;
								iterator.Current.WriteSubtree(xwriter);
								xwriter.Close();
								swriter.Close();
								results.Add(swriter.ToString());
							}
							break;
					}
			}
			return results;
		}
	}
}
