using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Resources;
using XsltUtility;

namespace UnitTests.XsltUtility
{
	[TestClass]
	public class ProgramFixture
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
		[ExpectedException(typeof(ArgumentException))]
		public void GetXsltFromProcessingInstruction_EmptyBasePath()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlWithPIPath, "Transform.xml");
			XPathDocument doc = new XPathDocument(xml.FullName);
			FileInfo xsl = Program.GetXsltFromProcessingInstruction(doc, "");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetXsltFromProcessingInstruction_NullDoc()
		{
			Program.GetXsltFromProcessingInstruction(null, Environment.CurrentDirectory);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetXsltFromProcessingInstruction_NullBasePath()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlWithPIPath, "Transform.xml");
			XPathDocument doc = new XPathDocument(xml.FullName);
			FileInfo xsl = Program.GetXsltFromProcessingInstruction(doc, null);
		}

		[TestMethod]
		public void GetXsltFromProcessingInstruction_NoProcessingInstruction()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlNoPIPath, "Transform.xml");
			XPathDocument doc = new XPathDocument(xml.FullName);
			FileInfo xsl = Program.GetXsltFromProcessingInstruction(doc, Environment.CurrentDirectory);
			Assert.IsNull(xsl, "If there is no processing instruction, null should be returned.");
		}

		[TestMethod]
		public void GetXsltFromProcessingInstruction_MissingPathInProcessingInstruction()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlMissingPIPath, "Transform.xml");
			XPathDocument doc = new XPathDocument(xml.FullName);
			FileInfo xsl = Program.GetXsltFromProcessingInstruction(doc, Environment.CurrentDirectory);
			Assert.IsNull(xsl, "If the processing instruction is missing the path, null should be returned.");
		}

		[TestMethod]
		public void GetXsltFromProcessingInstruction_ValidProcessingInstruction()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlWithPIPath, "Transform.xml");
			XPathDocument doc = new XPathDocument(xml.FullName);
			FileInfo xsl = Program.GetXsltFromProcessingInstruction(doc, Environment.CurrentDirectory);
			Assert.AreEqual("Transform.xslt", xsl.Name, "If there is a processing instruction, that value should be returned.");
		}

		[TestMethod]
		public void Main_XmlNotFound()
		{
			FileInfo xml = new FileInfo("no_such_file.xml");
			FileInfo xsl = this._tempFiles.ExtractTempFile(FileExtractor.TransformXsltPath, "Transform.xslt");
			FileInfo outfile = new FileInfo(Path.Combine(this._tempFileDir, "Output.txt"));
			int retCode = Program.Main(new string[] { xml.FullName, xsl.FullName, outfile.FullName });
			Assert.AreEqual(3, retCode, "The return code for XML not found should be 3.");
		}

		[TestMethod]
		public void Main_XmlNotValid()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.MalformedXmlPath, "Transform.xml");
			FileInfo xsl = this._tempFiles.ExtractTempFile(FileExtractor.TransformXsltPath, "Transform.xslt");
			FileInfo outfile = new FileInfo(Path.Combine(this._tempFileDir, "Output.txt"));
			int retCode = Program.Main(new string[] { xml.FullName, xsl.FullName, outfile.FullName });
			Assert.AreEqual(1, retCode, "The return code for bad XML should be 1.");
		}

		[TestMethod]
		public void Main_XslSpecifiedButNotFound()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlNoPIPath, "Transform.xml");
			FileInfo xsl = new FileInfo("no_such_file.xsl");
			FileInfo outfile = new FileInfo(Path.Combine(this._tempFileDir, "Output.txt"));
			int retCode = Program.Main(new string[] { xml.FullName, xsl.FullName, outfile.FullName });
			Assert.AreEqual(3, retCode, "The return code for XSL not found should be 3.");
		}

		[TestMethod]
		public void Main_XslInferredButNotFound()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlWithPIPath, "Transform.xml");
			int retCode = Program.Main(new string[] { xml.FullName });
			Assert.AreEqual(3, retCode, "The return code for XSL not found should be 3.");
		}

		[TestMethod]
		public void Main_XslInferredButNoProcessingInstruction()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlNoPIPath, "Transform.xml");
			int retCode = Program.Main(new string[] { xml.FullName });
			Assert.AreEqual(3, retCode, "The return code for XSL not found should be 3.");
		}

		[TestMethod]
		public void Main_ValidTransform_Console_XslInferredFromProcessingInstruction()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlWithPIPath, "Transform.xml");
			FileInfo xsl = this._tempFiles.ExtractTempFile(FileExtractor.TransformXsltPath, "Transform.xslt");
			int retCode = Program.Main(new string[] { xml.FullName });
			Assert.AreEqual(0, retCode, "The return code for a successful transform should be 0.");
		}

		[TestMethod]
		public void Main_ValidTransform_Console_XslSpecified()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlWithPIPath, "Transform.xml");
			FileInfo xsl = this._tempFiles.ExtractTempFile(FileExtractor.TransformXsltPath, "Transform.xslt");
			int retCode = Program.Main(new string[] { xml.FullName, xsl.FullName });
			Assert.AreEqual(0, retCode, "The return code for a successful transform should be 0.");
		}

		[TestMethod]
		public void Main_ValidTransform_OutputFile_XslSpecified()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlWithPIPath, "Transform.xml");
			FileInfo xsl = this._tempFiles.ExtractTempFile(FileExtractor.TransformXsltPath, "Transform.xslt");
			FileInfo outfile = new FileInfo(Path.Combine(this._tempFileDir, "Output.txt"));
			int retCode = Program.Main(new string[] { xml.FullName, xsl.FullName, outfile.FullName});
			Assert.AreEqual(0, retCode, "The return code for a successful transform should be 0.");
			Assert.IsTrue(outfile.Exists, "The output file should have been created.");
		}

		[TestMethod]
		public void Main_XslNotValid()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlNoPIPath, "Transform.xml");
			FileInfo xsl = this._tempFiles.ExtractTempFile(FileExtractor.InvalidXsltPath, "Transform.xslt");
			FileInfo outfile = new FileInfo(Path.Combine(this._tempFileDir, "Output.txt"));
			int retCode = Program.Main(new string[] { xml.FullName, xsl.FullName, outfile.FullName });
			Assert.AreEqual(1, retCode, "The return code for bad XSL should be 1.");
		}

		[TestMethod]
		public void Main_NotEnoughArguments()
		{
			int retCode = Program.Main(new string[] { });
			Assert.AreEqual(2, retCode, "The return code for not enough arguments should be 2.");
		}

		[TestMethod]
		public void Main_TooManyArguments()
		{
			FileInfo xml = this._tempFiles.ExtractTempFile(FileExtractor.TransformXmlNoPIPath, "Transform.xml");
			FileInfo xsl = this._tempFiles.ExtractTempFile(FileExtractor.TransformXsltPath, "Transform.xslt");
			FileInfo outfile = new FileInfo(Path.Combine(this._tempFileDir, "Output.txt"));
			int retCode = Program.Main(new string[] { xml.FullName, xsl.FullName, outfile.FullName, "additionalarg" });
			Assert.AreEqual(2, retCode, "The return code for too many arguments should be 2.");
		}

		[TestMethod]
		public void ShowHelp_NoExceptionThrown()
		{
			try
			{
				Program.ShowHelp();
			}
			catch (Exception ex)
			{
				Assert.Fail("Unexpected exception while showing help: {0}", ex.Message);
			}
		}
	}
}
