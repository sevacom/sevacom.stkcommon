using System;
using System.Windows.Input;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.TestUtils;
using StkCommon.UI.Wpf.Hotkeys;
using StkCommon.UI.Wpf.Model;

namespace StkCommon.UI.Wpf.Test.Hotkeys
{
	[TestFixture]
    public class HotkeyCommandInfoTest : AutoMockerTestsBase<HotkeyCommandInfo>
	{
		/// <summary>
		/// Должен вызывать ArgumentException если создают команду с пустым уникальным именем или DisplayName
		/// </summary>
		[Test]
		public void ShouldThrowArgumentExceptionWhenCommandInfoInvalid()
		{
			//Given //When //Then
			Assert.Throws<ArgumentException>(() => new HotkeyCommandInfo("", "test"));
			Assert.Throws<ArgumentException>(() => new HotkeyCommandInfo("uniqueId", ""));
			Assert.DoesNotThrow(() => new HotkeyCommandInfo("uniqueName", "test"));
		}

		/// <summary>
		/// Equals сравнивает по id
		/// </summary>
		[TestCase("uniqueName1", "uniqueName1", true)]
		[TestCase("uniqueName1", "uniqueName2", false)]
		public void ShouldEquals(string uniqueName1, string uniqueName2, bool expectedEquals)
		{
			//Given
			var expectedCommandInfo = new HotkeyCommandInfo(uniqueName1, "Name", "Category");
			var expectedCommandInfo2 = new HotkeyCommandInfo(uniqueName2, "Name2", "Category2");

			//When //Then
			expectedCommandInfo.Equals(expectedCommandInfo2).Should().Be(expectedEquals);
		}

		/// <summary>
		/// Проверяет работу EqualsByDefaultKey
		/// </summary>
		[TestCase(Key.T, ModifierKeys.None, Key.T, ModifierKeys.None, true)]
		[TestCase(Key.T, ModifierKeys.None, Key.P, ModifierKeys.None, false)]
		[TestCase(Key.T, ModifierKeys.None, Key.T, ModifierKeys.Alt, false)]
		[TestCase(Key.None, ModifierKeys.None, Key.None, ModifierKeys.None, false)]
		public void ShouldEqualsByDefaultKey(Key key1, ModifierKeys modifer1, Key key2, ModifierKeys modifer2, bool expectedEquals)
		{
			//Given
			var expectedCommandInfo = new HotkeyCommandInfo("uniqueName1", "Name", "Category", "11", new ShortCut(key1, modifer1));
			var expectedCommandInfo2 = new HotkeyCommandInfo("uniqueName2", "Name2", "Category2", "11", new ShortCut(key2, modifer2));

			//When //Then
			expectedCommandInfo.EqualsByDefaultKey(expectedCommandInfo2).Should().Be(expectedEquals);	
		}
	}
}
