using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Resources;
using XPathUtility;

namespace UnitTests.XPathUtility
{
	[TestClass]
	public class XPathEvaluatorFixture
	{
		private TempFileCollection _tempFiles = null;
		private string _tempFileDir = null;

		[TestInitialize]
		public void TestInitialize()
		{
			this._tempFiles = new TempFileCollection();
			this._tempFiles.KeepFiles = false;
			this._tempFileDir = this._tempFiles.BasePath;
			if (!Directory.Exists(this._tempFileDir))
			{
				Directory.CreateDirectory(this._tempFileDir);
			}
		}

		[TestCleanup]
		public void TestCleanup()
		{
			this._tempFiles.Delete();
			this._tempFiles = null;
			Directory.Delete(this._tempFileDir, true);
			this._tempFiles = null;
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void Ctor_FileNotFound()
		{
			FileInfo file = new FileInfo("no_such_file.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_NullFile()
		{
			XPathEvaluator evaluator = new XPathEvaluator(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Ctor_FileIsInvalidXml()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.MalformedXmlPath, "Malformed.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Evaluate_NullXpath()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
			evaluator.Evaluate(null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Evaluate_EmptyXpath()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
			evaluator.Evaluate("");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void Evaluate_InvalidXpath()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
			evaluator.Evaluate("<invalid>");
		}

		[TestMethod]
		public void Evaluate_XpathNotFound()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
			IList<string> results = evaluator.Evaluate("/root/notfound");
			Assert.AreEqual(0, results.Count, "If the expression isn't found, no results should be returned.");
		}

		[TestMethod]
		public void Evaluate_XpathFoundSingleInstanceElement()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
			IList<string> results = evaluator.Evaluate("/root/first");
			Assert.AreEqual(1, results.Count, "The wrong number of results was returned.");
			Assert.AreEqual("<first />", results[0], "The wrong result was returned.");
		}

		[TestMethod]
		public void Evaluate_XpathFoundSingleInstanceText()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
			IList<string> results = evaluator.Evaluate("/root/unlimited[@value='3']/text()");
			Assert.AreEqual(1, results.Count, "The wrong number of results was returned.");
			Assert.AreEqual("text", results[0], "The wrong result was returned.");
		}

		[TestMethod]
		public void Evaluate_XpathFoundSingleInstanceTree()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
			IList<string> results = evaluator.Evaluate("/root/unlimited[@value='2']");
			Assert.AreEqual(1, results.Count, "The wrong number of results was returned.");
			Assert.AreEqual(String.Format("<unlimited value=\"2\">{0}  <nested />{0}</unlimited>", Environment.NewLine), results[0], "The wrong result was returned.");
		}

		[TestMethod]
		public void Evaluate_XpathFoundMultipleInstanceTree()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
			IList<string> results = evaluator.Evaluate("/root/unlimited");
			Assert.AreEqual(4, results.Count, "The wrong number of results was returned.");
		}

		[TestMethod]
		public void Evaluate_XpathFoundSingleInstanceValue()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			XPathEvaluator evaluator = new XPathEvaluator(file);
			IList<string> results = evaluator.Evaluate("/root/unlimited/@value");
			Assert.AreEqual(3, results.Count, "The wrong number of results was returned.");
			Assert.AreEqual("1", results[0], "The wrong result was returned.");
			Assert.AreEqual("2", results[1], "The wrong result was returned.");
			Assert.AreEqual("3", results[2], "The wrong result was returned.");
		}
	}
}
