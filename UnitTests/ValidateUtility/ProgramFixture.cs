using System;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ValidateUtility;
using UnitTests.Resources;

namespace UnitTests.ValidateUtility
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
		public void Main_DocumentValidAutoValidate()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			FileInfo schema = this._tempFiles.ExtractTempFile(FileExtractor.SchemaPath, "Schema.xsd");
			int retCode = Program.Main(new string[] { file.FullName });
			Assert.AreEqual(0, retCode, "The return code for a valid document should be 0.");
		}

		[TestMethod]
		public void Main_DocumentValidSchemaValidate()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			FileInfo schema = this._tempFiles.ExtractTempFile(FileExtractor.SchemaPath, "Schema.xsd");
			int retCode = Program.Main(new string[] { file.FullName });
			Assert.AreEqual(0, retCode, "The return code for a valid document should be 0.");
		}

		[TestMethod]
		public void Main_DocumentInvalid()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.InvalidXmlPath, "Invalid.xml");
			FileInfo schema = this._tempFiles.ExtractTempFile(FileExtractor.SchemaPath, "Schema.xsd");
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
		public void ParseCommandLine_AdditionalSchemaPaths()
		{
			var options = Program.ParseCommandLine(new string[] { "file", "1", "2" });
			Assert.AreEqual("file", options.File.Name, "The filename was not properly parsed.");
			Assert.AreEqual(2, options.AdditionalSchemaPaths.Length, "The wrong number of additional schemas were found.");
			Assert.AreEqual("1", options.AdditionalSchemaPaths[0], "The first additional schema was not parsed correctly.");
			Assert.AreEqual("2", options.AdditionalSchemaPaths[1], "The second additional schema was not parsed correctly.");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentException))]
		public void ParseCommandLine_NoArgs()
		{
			Program.ParseCommandLine(new string[] { });
		}

		[TestMethod]
		public void ParseCommandLine_OneArg()
		{
			var options = Program.ParseCommandLine(new string[] { "file" });
			Assert.AreEqual("file", options.File.Name, "The filename was not properly parsed.");
			Assert.IsNull(options.AdditionalSchemaPaths, "If no additional schema paths are provided, none should be parsed.");
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
