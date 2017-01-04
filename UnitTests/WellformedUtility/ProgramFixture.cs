using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Resources;
using WellformedUtility;

namespace UnitTests.WellformedUtility
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
		public void BuildValidatorSettings_SetsAllValues()
		{
			XmlReaderSettings settings = Program.BuildValidatorSettings();
			Assert.AreEqual(ConformanceLevel.Auto, settings.ConformanceLevel, "Conformance level was not set.");
			Assert.IsFalse(settings.IgnoreComments, "Comments should not be ignored.");
			Assert.IsFalse(settings.IgnoreProcessingInstructions, "Processing instructions should not be ignored.");
			Assert.IsFalse(settings.IgnoreWhitespace, "Whitespace should not be ignored.");
			Assert.AreEqual(ValidationType.None, settings.ValidationType, "No validation should be performed.");
			Assert.AreEqual(XmlSchemaValidationFlags.None, settings.ValidationFlags, "No validation should be performed.");
		}

		[TestMethod]
		public void Main_DocumentWellformed()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			int retCode = Program.Main(new string[] { file.FullName });
			Assert.AreEqual(0, retCode, "The return code for a wellformed document should be 0.");
		}

		[TestMethod]
		public void Main_DocumentNotWellformed()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.MalformedXmlPath, "Malformed.xml");
			int retCode = Program.Main(new string[] { file.FullName });
			Assert.AreEqual(1, retCode, "The return code for an invalid document should be 1.");
		}

		[TestMethod]
		public void Main_FileNotFound()
		{
			int retCode = Program.Main(new string[] { "NoSuchFile.xml" });
			Assert.AreEqual(3, retCode, "The return code for not finding the file to validate should be 3.");
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
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			int retCode = Program.Main(new string[] { file.FullName, "additionalarg" });
			Assert.AreEqual(2, retCode, "The return code for too many arguments should be 2.");
		}

		[TestMethod]
		public void ParseCommandLine_ValidArgs()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			FileInfo actual = Program.ParseCommandLine(new string[] { file.FullName });
			Assert.AreEqual(file.FullName, actual.FullName, "The file info returned should point to the found file.");
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void ParseCommandLine_FileNotFound()
		{
			Program.ParseCommandLine(new string[] { "NoSuchFile.xml" });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseCommandLine_NotEnoughArgs()
		{
			Program.ParseCommandLine(new string[] { });
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseCommandLine_TooManyArgs()
		{
			Program.ParseCommandLine(new string[] { "a", "b" });
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
