using System;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;

namespace UnitTests.Resources
{
	public static class FileExtractor
	{
		public const string SchemaPath = "UnitTests.Resources.Schema.xsd";
		public const string ValidXmlPath = "UnitTests.Resources.Valid.xml";
		public const string InvalidXmlPath = "UnitTests.Resources.Invalid.xml";
		public const string MalformedXmlPath = "UnitTests.Resources.Malformed.xml";
		public const string TransformXmlNoPIPath = "UnitTests.Resources.Transform1.xml";
		public const string TransformXmlWithPIPath = "UnitTests.Resources.Transform2.xml";
		public const string TransformXmlMissingPIPath = "UnitTests.Resources.Transform3.xml";
		public const string TransformXsltPath = "UnitTests.Resources.Transform.xslt";
		public const string InvalidXsltPath = "UnitTests.Resources.Invalid.xslt";

		public static void ExtractResource(string resourcePath, string targetPath)
		{
			using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath))
			using (Stream outputStream = new FileStream(targetPath, FileMode.CreateNew))
			{
				byte[] buffer = new byte[1024];
				int read;
				while ((read = resourceStream.Read(buffer, 0, buffer.Length)) > 0)
				{
					outputStream.Write(buffer, 0, read);
				}
				outputStream.Close();
				resourceStream.Close();
			}
		}

		public static FileInfo ExtractTempFile(this TempFileCollection tfc, string resourcePath, string filename)
		{
			string targetPath = Path.Combine(tfc.BasePath, filename);
			ExtractResource(resourcePath, targetPath);
			tfc.AddFile(targetPath, false);
			return new FileInfo(targetPath);
		}
	}
}
