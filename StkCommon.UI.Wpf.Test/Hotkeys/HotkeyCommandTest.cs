using System.Windows.Input;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using StkCommon.TestUtils;
using StkCommon.UI.Wpf.Hotkeys;
using StkCommon.UI.Wpf.Model;

namespace StkCommon.UI.Wpf.Test.Hotkeys
{
	[TestFixture]
    public class HotkeyCommandTest : AutoMockerTestsBase<HotkeyCommand>
	{

		public override void SetUp()
		{
            base.SetUp();
			GetMock<ICommand>().Setup(c => c.CanExecute(It.IsAny<object>())).Returns(true);
		}

	    protected override HotkeyCommand DirectCreateTarget()
	    {
            return new HotkeyCommand(new HotkeyCommandInfo("cmdId", "Тестовая команда"),
                                        new ShortCut(Key.NumLock),
                                        Get<ICommand>());
	    }


		/// <summary>
		/// Должен вызвать команду в обработчике исключений.
		/// </summary>
		[Test]
		public void ShouldExecuteCommandWhenRiseHotkeyCommand()
		{
			// Given
			var executedInsideSilentHandler = 0;

			GetMock<ICommand>().Setup(c => c.Execute(It.IsAny<object>()))
				.Callback((object o) => executedInsideSilentHandler++);

			//When
			Target.RiseHotkeyCommand();

			// Then
			executedInsideSilentHandler.Should().Be(1, "Комнада не была вызвана.");
		}
	}
}