using DbTestFramework.DemoApp.Model;
using NUnit.Framework;
using StkCommon.TestUtils.DbTestFramework;

namespace DbTestFramework.DemoApp.Tests
{
    /// <summary>
    /// Тесты показывающие примеры использования проекта DbProjector, в качестве источника настроек
    /// </summary>
    [TestFixture]
    public class TestConfiguredFromDbProjector
    {
        private IStkTestDbAccessor _target;
        private TestDbFramework _db;

        [SetUp]
        public void SetUp()
        {
            // строка подключения к пустой базе (база должна существовать!)
            const string connectionString = @"Data Source=192.168.18.81;Initial Catalog=DataBaseTest;User Id=sa;Password=mag;";
			 
            // путь до папки с проектом из DbProjector (относительно папки bin\Debug\)
            _db = new TestDbFramework(connectionString, new DbProjectorDbStruct(@"..\..\DataBase\SignatecTest"));
			_target = new DataAccessorFactory(connectionString).GetAccessor();
        }

        [TearDown]
        public void TearDown()
        {
			if(_target != null)
				_target.Dispose();
			if (_db != null)
				_db.Dispose();
        }

        ///<summary>
        /// Простой тест с использованием параметров и автоикнрементного поля
        ///</summary>
        [Test] 
		[Ignore("Тест для примера")]
        public void SimpleTestWithAutoInc()
        {
            //Given
            const string name = "SomeName2";
            const int weight = 10;
            //задаем параметры, которые будут использованы для подстановки
            _db.Parameters(name, weight)
                //указываем какой файл использовать для настройки теста
                .SetupFromFile(@"Tests\DbProjectorConfiguredTest1.csv");

            //When
            //вызываем тестируюмую процедур
            var reader = _target.InsertOrUpdateProduct(null, name, weight);

            //Then
            // проверка результатов выполнения - бросает исключение, с описанием проблемы
            _db.VerifyAll(reader);
        }

        ///<summary>
        /// Тест с использованием foreign key
        ///</summary>
        [Test]
		[Ignore("Тест для примера")]
        public void SimpleTestWithForeignKey()
        {
            //Given
            const int productId = 22;
            _db.Parameters(productId)
                .SetupFromFile(@"Tests\DbProjectorConfiguredTest2.csv");

            //When
            _target.DeleteProduct(productId);

            //Then
            _db.VerifyAll();
        }
    }
}