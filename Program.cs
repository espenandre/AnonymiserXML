using System;
using System.IO;
using System.Xml;

namespace AnonymiserXML
{
	class Program
	{
		static void Main(string[] args)
		{
			var basePath = Directory.GetCurrentDirectory();

			var folders = Directory.GetParent($"{basePath}").EnumerateDirectories("*.d");

			if (basePath.StartsWith("R:"))
			{
				Console.WriteLine("Du må kopiere filene bort fra R-stasjonen før du kan anonymisere");
				Console.WriteLine("Trykk en tast for å avslutte");
				Console.ReadKey();
				return;
			}

			Console.WriteLine($"Er du sikker på at du permanent vil anonymisere dataene i mappen {basePath}? (J/N)");
			var answer = Console.ReadKey(true);
			if (answer.KeyChar != 'j' && answer.KeyChar != 'J')
			{
				Console.WriteLine("Trykk en tast for å avslutte");
				Console.ReadKey();
				return;
			}

			var counter = 0;
			foreach (var folder in folders)
			{
				var acqData = $"AcqData";
				var filename = $@"{folder}\{acqData}\sample_info.xml";

				var attributes = File.GetAttributes(filename);
				if ((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				{
					// Make the file RW
					attributes = RemoveAttribute(attributes, FileAttributes.ReadOnly);
					File.SetAttributes(filename, attributes);
				}

				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.PreserveWhitespace = true;
				xmlDocument.Load(filename);

				XmlNodeList sampleNameNode;
				XmlNode root = xmlDocument.DocumentElement;

				sampleNameNode = root.SelectNodes("//SampleInfo/Field[Name='Sample Name']");

				foreach (XmlNode node in sampleNameNode)
				{
					var valueNode = node.SelectSingleNode("Value");
					valueNode.InnerText = "abc-123";
				}
				xmlDocument.Save(filename);
				counter++;
			}

			Console.WriteLine($"Endret {counter} filer");
			Console.WriteLine("Trykk en tast for å avslutte");
			Console.ReadKey();
		}

		private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
		{
			return attributes & ~attributesToRemove;
		}
	}
}
