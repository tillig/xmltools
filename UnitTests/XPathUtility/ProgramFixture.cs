using System;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Resources;
using XPathUtility;

namespace UnitTests.XPathUtility
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
		public void Main_XpathFound()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			int retCode = Program.Main(new string[] { file.FullName, "/root" });
			Assert.AreEqual(0, retCode, "The return code for xpath found should be 0.");
		}

		[TestMethod]
		public void Main_XpathNotFound()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			int retCode = Program.Main(new string[] { file.FullName, "/notfound" });
			Assert.AreEqual(1, retCode, "The return code for xpath not found should be 1.");
		}

		[TestMethod]
		public void Main_BadXpathExpression()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			int retCode = Program.Main(new string[] { file.FullName, "<notxpath>" });
			Assert.AreEqual(2, retCode, "The return code for bad xpath should be 2.");
		}

		[TestMethod]
		public void Main_FileNotFound()
		{
			int retCode = Program.Main(new string[] { "NotFound.xml", "/root" });
			Assert.AreEqual(3, retCode, "The return code for file not found should be 3.");
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
			int retCode = Program.Main(new string[] { file.FullName, "/root", "additionalarg" });
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
