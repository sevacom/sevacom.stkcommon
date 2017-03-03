using FluentAssertions;
using NUnit.Framework;
using StkCommon.Data.Settings;
using StkCommon.TestUtils;

namespace StkCommon.Data.Test.Settings
{
	[TestFixture]
	public class AppSettingsProviderTest : AutoMockerTestsBase<AppSettingsProvider>
	{
		private const string UndefinedParamName = "UndefinedParam";
		private const string ParamStringName = "ParamString";
		private const string ParamBoolName = "ParamBool";
		private const string ParamIntName = "ParamInt";
		private const string ParamEnumTest2Name = "ParamEnumTest2";
		private const string ParamEnumTest2StrName = "ParamEnumTest2Str";

		private const string ExpectedParamString = "ParamString1";
		private const bool ExpectedParamBool = true;
		private const int ExpectedParamInt = 42;
		private const TestEnum ExpectedParamTest2 = TestEnum.Test2;

		[Test]
		public void ShouldGetStringValue()
		{
			const string expectedDefault = "Default";
			Target.GetValue(ParamStringName).Should().Be(ExpectedParamString);
			Target.GetValue(ParamStringName, expectedDefault).Should().Be(ExpectedParamString);
			Target.GetValue(UndefinedParamName, expectedDefault).Should().Be(expectedDefault);
			Target.GetValue(UndefinedParamName).Should().BeNull();
		}

		[Test]
		public void ShouldGetIntValue()
		{
			const int expectedDefault = 1000;
			Target.GetValue(ParamIntName, expectedDefault).Should().Be(ExpectedParamInt);
			Target.GetValue(UndefinedParamName, expectedDefault).Should().Be(expectedDefault);
		}

		[Test]
		public void ShouldGetBoolValue()
		{
			const bool expectedDefault = true;
			Target.GetValue(ParamBoolName, expectedDefault).Should().Be(ExpectedParamBool);
			Target.GetValue(UndefinedParamName, expectedDefault).Should().Be(expectedDefault);
		}

		[Test]
		public void ShouldGetEnumValue()
		{
			const TestEnum expectedDefault = TestEnum.Test3;
			Target.GetValue(ParamEnumTest2Name, expectedDefault).Should().Be(ExpectedParamTest2);
			Target.GetValue(ParamEnumTest2StrName, expectedDefault).Should().Be(ExpectedParamTest2);
			Target.GetValue(UndefinedParamName, expectedDefault).Should().Be(expectedDefault);
		}

		public enum TestEnum
		{
			Test1 = 1,
			Test2 = 2,
			Test3 = 3
		}
	}
}
