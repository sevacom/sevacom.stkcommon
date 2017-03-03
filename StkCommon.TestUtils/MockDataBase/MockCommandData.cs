using System.Collections.Generic;

namespace StkCommon.TestUtils.MockDataBase
{
	public class MockCommandData
	{
		public int NonQueryResult { get; set; }

		public MockReaderData ReaderResult { get; set; }

		public string CommandText { get; set; }

		public bool IsUsing { get; set; }

		public List<MockDbDataParameter> Parameters { get; set; }
	}
}