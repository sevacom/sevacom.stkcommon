using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.TestUtils.DbTestFramework;

namespace StkCommon.TestUtils.Test.DbTestFramework
{
    [TestFixture]
    public class DataReaderComparerTest
    {
        private static readonly string[] Row1 = { "Row1 value1", "Row1 value2", "Row1 value3" };
        private static readonly string[] Row2 = { "Row2 value1", "Row2 value2", "Row2 value3" };
        private static readonly string[] Row3 = { "Row3 value1", "Row3 value2", "Row3 value3" };

        private static readonly string[][] Row12 = { Row1, Row2 };
        private static readonly string[][] Row122 = { Row1, Row2, Row2 };
        private static readonly string[][] Row21 = { Row2, Row1 };
        private static readonly string[][] Row13 = { Row1, Row3 };
        private static readonly string[][] Row1223 = { Row1, Row2, Row2, Row3 };
        private static readonly string[][] Row123 = { Row1, Row2, Row3 };
        private static readonly string[][] Row1233 = { Row1, Row2, Row3, Row3 };

        private static readonly string[] Fields = { "Field1", "Field2", "Field3" };

        private static object[] ShouldCompareCaseSource()
        {
            return new object[]
            {
                new object[]{Row12, Row12, true},
                new object[]{Row12, Row21, true},
                new object[]{Row12, Row13, false},
                new object[]{Row1223, Row123, false},
                new object[]{Row123, Row1223, false},
                new object[]{Row1223, Row1233, false},
                new object[]{Row122, Row122, true},

            };
        }

        [TestCaseSource("ShouldCompareCaseSource")]
        public void ShouldCompare(string[][] resultData, string[][] expectedData, bool isEqual)
        {
            //Given
            var dr = AsDataReader(resultData);

            //When
            var ok = dr.Compare(Fields, expectedData);

            //Then
            string.IsNullOrEmpty(ok).Should().Be(isEqual);

        }


        private static IDataReader AsDataReader(IEnumerable<string[]> data)
        {
            return data
                .Select(row => new {Field1 = row[0], Field2 = row[1], Field3 = row[2]})
                .ToDataReader();

        }
    }
}