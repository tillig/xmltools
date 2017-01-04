using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace ValidateUtility
{
	public class XmlValidator
	{
		public FileInfo FileToValidate { get; private set; }
		public IList<String> ValidationErrors { get; private set; }
		public string[] AdditionalSchemaPaths { get; private set; }

		public XmlValidator(FileInfo fileToValidate, string[] additionalSchemaPaths)
		{
			if (fileToValidate == null)
			{
				throw new ArgumentNullException("fileToValidate");
			}
			if (!fileToValidate.Exists)
			{
				throw new FileNotFoundException("File to validate not found.", fileToValidate.FullName);
			}
			this.FileToValidate = fileToValidate;
			this.ValidationErrors = new List<string>();
			this.AdditionalSchemaPaths = additionalSchemaPaths;
		}

		public bool Validate()
		{
			XmlReaderSettings settings = this.BuildValidatorSettings();
			try
			{
				using (XmlReader reader = XmlReader.Create(this.FileToValidate.FullName, settings))
				{
					while (reader.Read()) ;
					reader.Close();
				}
			}
			catch (XmlException ex)
			{
				this.ValidationErrors.Add(ex.Message);
			}
			return this.ValidationErrors.Count == 0;
		}

		public XmlReaderSettings BuildValidatorSettings()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.IgnoreWhitespace = false;
			settings.ValidationEventHandler += ValidationCallback;
			settings.ValidationType = ValidationType.Schema;
			settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessSchemaLocation;
			settings.ValidationFlags |= XmlSchemaValidationFlags.ProcessInlineSchema;
			settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
			settings.Schemas = this.InitializeSchemas();
			return settings;
		}

		private XmlSchemaSet InitializeSchemas()
		{
			var schemaSet = new XmlSchemaSet();
			if (this.AdditionalSchemaPaths == null || this.AdditionalSchemaPaths.Length == 0)
			{
				return schemaSet;
			}
			foreach (string searchPath in this.AdditionalSchemaPaths)
			{
				if (File.Exists(searchPath))
				{
					schemaSet.Add(null, searchPath);
					continue;
				}
				foreach (var found in Directory.GetFiles(Directory.GetCurrentDirectory(), searchPath))
				{
					schemaSet.Add(null, found);
					continue;
				}
			}
			return schemaSet;
		}

		public void ValidationCallback(Object obj, ValidationEventArgs args)
		{
			this.ValidationErrors.Add(args.Message);
		}
	}
}
