using DbTestFramework.DemoApp.Model;
using NUnit.Framework;
using StkCommon.TestUtils.DbTestFramework;

namespace DbTestFramework.DemoApp.Tests
{
    [TestFixture]
    public class TestConfiguredFromCode
    {
        private IStkTestDbAccessor _target;
        private TestDbFramework _db;

        [SetUp]
        public void SetUp()
        {
            // строка подключения к пустой базе (база должна существовать!)
            const string connectionString = @"Data Source=192.168.18.81;Initial Catalog=WriteControlS2S1Test;User Id=sa;Password=m1m2m3-=;";
            // путь до папки с проектом из DbProjector (относительно папки bin\Debug\)
            _db = new TestDbFramework(connectionString, null);
			_target = new DataAccessorFactory(connectionString).GetAccessor();
        }

        [TearDown]
        public void TearDown()
        {
			if (_target != null)
				_target.Dispose();
			if (_db != null)
				_db.Dispose();
        }

        ///<summary>
        /// тест проверяет работы процедуры выборки данных
        ///</summary>
        [Test]
		[Ignore("Тест для примера")]
        public void ShouldLoadData()
        {
            //Given
            var tableRows = new[]
            {
                new[] {"1", "'Some Name'","44"},
                new[] {"4", "'Other Name'","77"},
            };
            const string selectProcedure = @"CREATE PROCEDURE dbo.usp_SelectProducts
 @id INT = NULL
AS 
 SELECT
  p.Id,
  p.Name,
  p.Weight
  FROM dbo.dt_Products p
  WHERE @id IS NULL OR p.id = @id";

            _db.Setup()
               .Table("dt_Products", "Id:Int", "Name:NVARCHAR(1000)", "Weight:Int")
               .Row(tableRows[0])
               .Row(tableRows[1])
               .Sp("usp_SelectProducts", selectProcedure)
               .Apply()
               ;
            _db.ResultQuery("Id","Name", "Weight")
               .AddRow(tableRows[1]);
            //When
            var result = _target.SelectProducts(4);

            //Then
            _db.VerifyResultQuery(result);
            
        }
    }
}