using System.Linq;
using System.Windows.Input;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.TestUtils;
using StkCommon.UI.Wpf.Commands;
using StkCommon.UI.Wpf.Hotkeys;
using StkCommon.UI.Wpf.Model;

namespace StkCommon.UI.Wpf.Test.Hotkeys
{
	[TestFixture]
    public class HotkeyCommandManagerTest : AutoMockerTestsBase<HotkeyCommandManager>
	{
		private static readonly ShortCut CommandKeyShortcut = new ShortCut(Key.F5);
		private HotkeyCommandInfo _expectedCommandInfo;

	    public override void SetUp()
	    {
	        base.SetUp();
            _expectedCommandInfo = CreateCorrectCommandInfo();
        }

		private static HotkeyCommandInfo CreateCorrectCommandInfo()
		{
			return new HotkeyCommandInfo
			(
				"uniqueId",
				"test",
				"testCategory",
				"",
                CommandKeyShortcut
            );
		}

		private static bool IsCommandInfoEquals(HotkeyCommandInfo cmd1, HotkeyCommandInfo cmd2)
		{
			return cmd1.Equals(cmd2) &&
				   cmd1.DisplayName == cmd2.DisplayName &&
				   cmd1.Category == cmd2.Category &&
				   cmd1.DefaultHotkey.Equals(cmd2.DefaultHotkey);
		}


		/// <summary>
		/// Проверяет что правильная команда корректно регистрируется
		/// </summary>
		[Test]
		public void ShouldRegisterCommandWhenCommandCorrect()
		{
			//Given
			var expectedCommand = new DelegateCommand(o => { }, o => true);

			//When
			Target.RegisterCommand(_expectedCommandInfo, expectedCommand);
			var actualCommand = Target.HotKeyCommands.FirstOrDefault();

			//Then
		   
			Assert.IsNotNull(actualCommand);
			Target.HotKeyCommands.Should().OnlyContain(p => IsCommandInfoEquals(p.CommandInfo, _expectedCommandInfo));
			actualCommand.HotKey.Key.Should().Be(_expectedCommandInfo.DefaultHotkey.Key);
			actualCommand.HotKey.Modifier.Should().Be(_expectedCommandInfo.DefaultHotkey.Modifier);
		}

		/// <summary>
		/// Проверяет что кидается SameHotkeyCommandException если регистрируют команду с существующим именем и категорией
		/// </summary>
		[Test]
		public void ShouldThrowExceptionWhenRegisterSameCommand()
		{
			//Given
			Target.RegisterCommand(_expectedCommandInfo, Get<ICommand>());
			
			var sameDefaultHotKeyCommandInfo = new HotkeyCommandInfo(_expectedCommandInfo.Id + "2", _expectedCommandInfo.DisplayName + "-", _expectedCommandInfo.Category + "-", null, _expectedCommandInfo.DefaultHotkey);

			//When //Then			
			Assert.Throws<SameHotkeyCommandException>(() =>
				Target.RegisterCommand(sameDefaultHotKeyCommandInfo, Get<ICommand>()), "не сработал SameHotkeyCommandException");
		}

		/// <summary>
		/// Должен зарегистрировать такую же команду если она Shared и та которая уже есть Shared
		/// </summary>
		[Test]
		public void ShouldRegisterSameCommandWhenIsShared()
		{
			//Given		
			_expectedCommandInfo.IsShared = true;
			Target.RegisterCommand(_expectedCommandInfo, Get<ICommand>());

			var newSameSharedCommand = CreateCorrectCommandInfo();
			newSameSharedCommand.IsShared = true;

			//When //Then			
			Assert.DoesNotThrow(() =>
				Target.RegisterCommand(newSameSharedCommand, Get<ICommand>()));
			Target.HotKeyCommands.Should().ContainSingle(p => p.CommandInfo.Id == _expectedCommandInfo.Id);
		}

		/// <summary>
		/// Проверяет что вызывается нужная команда если подать в ProcessHotKey сочетание клавиш
		/// </summary>
		[TestCase(true, 1)]
		[TestCase(false, 0)]
		public void ShouldExecuteCommandWhenProcessHotKey(bool isEnabledProcessHotKeys, int callCount)
		{
			// Given
			var actualCommandExecuteCalls = 0;
			var actualCommandCanExecuteCalls = 0;
			Target.IsEnabledProcessHotKeys = isEnabledProcessHotKeys;

			Target.RegisterCommand(_expectedCommandInfo,
				new DelegateCommand(
					o => { actualCommandExecuteCalls++; },
					o => { actualCommandCanExecuteCalls++; return true; }));

			//When
			Target.ProcessHotKey(CommandKeyShortcut);

            //Then
		    actualCommandExecuteCalls.Should().Be(callCount);
		    actualCommandCanExecuteCalls.Should().Be(callCount);
		}

		/// <summary>
		/// По умолчанию должны обрабатываются горячие клавиши
		/// </summary>
		[Test]
		public void ShouldIsTrueIsEnabledProcessHotKeys()
		{
			// Given			
			//When			
			//Then
			Target.IsEnabledProcessHotKeys.Should().BeTrue();
		}
	}
}
