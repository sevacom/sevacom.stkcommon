using System.Collections.Generic;
using System.Linq;

namespace StkCommon.TestUtils.MockDataBase
{
    public class MockReaderData
    {
        public MockReaderData()
        {
            Results = new List<MockReaderResultData>();
        }

        public List<MockReaderResultData> Results { get; private set; }

        public MockReaderResultData CurrentResult
        {
            get { return Results.LastOrDefault(); }
            set { Results.Add(value); }
        }
    }
}