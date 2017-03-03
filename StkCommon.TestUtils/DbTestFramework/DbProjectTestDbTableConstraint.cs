using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace StkCommon.TestUtils.DbTestFramework
{
	public class DbProjectTestDbTableConstraint : TestDbTableConstraint
	{
		private readonly Regex _referenceColumns = new Regex(@"REFERENCES (.+) \((.+)\)", RegexOptions.Compiled);
		private readonly Dictionary<string, string> _data = new Dictionary<string, string>();

		public DbProjectTestDbTableConstraint(string xmlString)
		{
			if (xmlString == null) throw new ArgumentNullException("xmlString");

			Parse(xmlString);
		}
		private void Parse(string xmlString)
		{
			var xml = XDocument.Parse(xmlString);
			if (xml == null || xml.Document == null || xml.Document.Root == null)
				throw new Exception("Невозможно разобрать тело органичения!" + Environment.NewLine + xmlString);
			ExtractNameIfExists(xml);
			foreach (var data in ExtractDatas(xml))
			{
				_data.Add(data.Key, data.Value);
			}

			ExtactIndexColumns();

			switch (xml.Document.Root.Name.ToString())
			{
				case "SrlClassTypes":
				case "TableIt":
				case "ConstraintDefault":
				case "ColumnDesc":
				case "ExtProps":
				case "GrantDesc":
				case "ConstraintCheckTableLevel":
					ConstraintType = TestDbConstraintType.Unknown;
					break;

				case "PKIndexConstraint":
					ConstraintType = TestDbConstraintType.PkIndexConstraint;
					ParseIndexData();
					break;

				case "UniqueIndexConstraint":
					ConstraintType = TestDbConstraintType.UniqueIndexConstraint;
					ParseIndexData();
					break;

				case "IndexDesc":
					ConstraintType = TestDbConstraintType.IndexDesc;
					ParseIndexData();
					break;

				case "ForeignKey":
					ConstraintType = TestDbConstraintType.ForeignKey;
					ParseForeignKeyData();
					break;

				default:
					throw new Exception("Неизвестный тип ограничения! " + xml.Document.Root.Name);
			}
		}

		private void ExtactIndexColumns()
		{
			string indexColumns = null;
			if (_data.ContainsKey("keys"))
			{
				indexColumns = _data["keys"];
			}
			if (_data.ContainsKey("index_keys"))
			{
				indexColumns = _data["index_keys"];
			}
			if (string.IsNullOrEmpty(indexColumns))
				return;
			var rest = indexColumns.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());
			IndexColumns.Clear();
			IndexColumns.AddRange(rest);
		}

		private static Dictionary<string, string> ExtractDatas(XDocument xml)
		{
			var result = new Dictionary<string, string>();
			var elements = xml.MayBe(x => x.Document)
							  .MayBe(x => x.Root)
							  .MayBe(x => x.Elements());
			if (elements == null)
				return result;
			foreach (var element in elements)
			{
				result.Add(element.Name.ToString(), element.Value);
			}
			return result;
		}

		private void ExtractNameIfExists(XDocument xml)
		{
			var name = xml.MayBe(x => x.Document)
						  .MayBe(x => x.Root)
						  .MayBe(x => x.Attribute("f_name"))
						  .MayBe(x => x.Value);
			Name = name;
		}

		private void ParseForeignKeyData()
		{
			var references = _data["keys2"];
			var match = _referenceColumns.Match(references);
			if (match.Success)
			{
				ReferencedTable = match.Groups[1].Value;
				var referencedColumns = match.Groups[2].Value.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
				ReferencedColumns.Clear();
				ReferencedColumns.AddRange(referencedColumns);
			}
			WithNoCheck = _data["with_nocheck"] == "true";

			CascadeOptions = _data["cascade"];
		}

		private void ParseIndexData()
		{
			Clustered = _data["index_descr"].Contains("clustered") && !_data["index_descr"].Contains("nonclustered");
			var includedColum = _data["include_columns"]
				.MayBe(s =>
					s.Replace("<string>", "#,#")
					 .Replace("</string>", string.Empty)
					 .Split(new[] { "#,#" }, StringSplitOptions.RemoveEmptyEntries)
					 );
			IncludedColumns.Clear();
			IncludedColumns.AddRange(includedColum ?? new string[0]);
		}
	}
}