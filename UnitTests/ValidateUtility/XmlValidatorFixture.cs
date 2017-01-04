using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Resources;
using ValidateUtility;

namespace UnitTests.ValidateUtility
{
	[TestClass]
	public class XmlValidatorFixture
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
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			XmlValidator validator = new XmlValidator(file, null);
			XmlReaderSettings settings = validator.BuildValidatorSettings();
			Assert.AreEqual(ValidationType.Schema, settings.ValidationType, "Validation type was not set.");
			Assert.IsFalse(settings.IgnoreWhitespace, "Whitespace should not be ignored.");
			Assert.IsTrue((settings.ValidationFlags & XmlSchemaValidationFlags.ProcessSchemaLocation) == XmlSchemaValidationFlags.ProcessSchemaLocation, "XML schema locations should be processed.");
			Assert.IsTrue((settings.ValidationFlags & XmlSchemaValidationFlags.ProcessInlineSchema) == XmlSchemaValidationFlags.ProcessInlineSchema, "XML inline schema should be processed.");
			Assert.IsTrue((settings.ValidationFlags & XmlSchemaValidationFlags.ReportValidationWarnings) == XmlSchemaValidationFlags.ReportValidationWarnings, "XML validation warnings should be reported.");
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void Ctor_FileNotFound()
		{
			FileInfo file = new FileInfo("No_Such_File.xml");
			XmlValidator validator = new XmlValidator(file, null);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Ctor_NullFileInfo()
		{
			XmlValidator validator = new XmlValidator(null, new string[0]);
		}

		[TestMethod]
		public void Validate_ValidatesDocumentBySchema()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "Valid.xml");
			FileInfo schema = this._tempFiles.ExtractTempFile(FileExtractor.SchemaPath, "Schema.xsd");
			XmlValidator validator = new XmlValidator(file, null);
			Assert.IsTrue(validator.Validate(), "The document should be valid.");
			Assert.AreEqual(0, validator.ValidationErrors.Count, "There should be no validation errors.");
		}

		[TestMethod]
		public void Validate_ValidatesDocumentBySchema_NoSchemaSpecified()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.ValidXmlPath, "ValidNoSchemaSpecified.xml");
			FileInfo schema = this._tempFiles.ExtractTempFile(FileExtractor.SchemaPath, "Schema.xsd");
			XmlValidator validator = new XmlValidator(file, new string[] { FileExtractor.SchemaPath });
			Assert.IsTrue(validator.Validate(), "The document should be valid.");
			Assert.AreEqual(0, validator.ValidationErrors.Count, "There should be no validation errors.");
		}

		[TestMethod]
		public void Validate_FindsInvalidDocumentsBySchema()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.InvalidXmlPath, "Invalid.xml");
			FileInfo schema = this._tempFiles.ExtractTempFile(FileExtractor.SchemaPath, "Schema.xsd");
			XmlValidator validator = new XmlValidator(file, null);
			Assert.IsFalse(validator.Validate(), "The document should not be valid.");
			Assert.AreNotEqual(0, validator.ValidationErrors.Count, "There should be validation errors.");
		}

		[TestMethod]
		public void Validate_FindsInvalidDocumentsBySchema_NoSchemaSpecified()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.InvalidXmlPath, "InvalidNoSchemaSpecified.xml");
			FileInfo schema = this._tempFiles.ExtractTempFile(FileExtractor.SchemaPath, "Schema.xsd");
			XmlValidator validator = new XmlValidator(file, new string[] { FileExtractor.SchemaPath });
			Assert.IsFalse(validator.Validate(), "The document should not be valid.");
			Assert.AreNotEqual(0, validator.ValidationErrors.Count, "There should be validation errors.");
		}

		[TestMethod]
		public void Validate_FindsMalformedDocuments()
		{
			FileInfo file = this._tempFiles.ExtractTempFile(FileExtractor.MalformedXmlPath, "Malformed.xml");
			FileInfo schema = this._tempFiles.ExtractTempFile(FileExtractor.SchemaPath, "Schema.xsd");
			XmlValidator validator = new XmlValidator(file, null);
			Assert.IsFalse(validator.Validate(), "The document should not be valid.");
			Assert.AreNotEqual(0, validator.ValidationErrors.Count, "There should be validation errors.");
		}
	}
}
