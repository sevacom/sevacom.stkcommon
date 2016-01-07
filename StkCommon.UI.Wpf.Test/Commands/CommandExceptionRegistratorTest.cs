using System;
using FluentAssertions;
using NUnit.Framework;
using StkCommon.UI.Wpf.Commands;

namespace StkCommon.UI.Wpf.Test.Commands
{
	[TestFixture]
	public class CommandExceptionRegistratorTest
	{
		[TestCase("CommandName", "В ходе выполнения операции 'CommandName' произошла ошибка.")]
		[TestCase("", "В ходе выполнения операции произошла ошибка.")]
		public void ShouldRegisterCommandException(string commandName, string expectedMessage)
		{
			//Given
			Exception actualException = null;
			string actualMessage = null;
			var target = new CommandExceptionRegistrator((e, s) =>
			{
				actualException = e;
				actualMessage = s;
			});

			var expectedExeption = new Exception("Test");
			var command = new DelegateCommand(o => { }, o => true, commandName);

			//When
			target.RegisterCommandException(expectedExeption, command);

			//Then
			actualException.Should().Be(expectedExeption);
			actualMessage.Should().Be(expectedMessage);
		}
	}
}