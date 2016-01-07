using System;
using System.Globalization;
using System.Windows.Input;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using StkCommon.UI.Wpf.Commands;
using StkCommon.UI.Wpf.Converters;

namespace StkCommon.UI.Wpf.Test.Converters
{
	[TestFixture]
	public class ExceptionHandlerCommandConverterBaseTest
	{
		private Mock<ExceptionHandlerCommandConverterBase> _mockTarget;
		private ExceptionHandlerCommandConverterBase _target;

		[SetUp]
		public void Setup()
		{
			_mockTarget = new Mock<ExceptionHandlerCommandConverterBase>
			{
				CallBase = true
			};
			_target = _mockTarget.Object;
		}

		/// <summary>
		/// Не должен вызывать метод GetCommandExceptionRegistrator при конвертации
		/// </summary>
		[Test]
		public void ShouldNotCallGetCommandExceptionRegistratorWhenConvert()
		{
			//Given //When
			_target.ConvertWithDefaultParams();

			//Then
			_mockTarget.Protected().Verify("GetCommandExceptionRegistrator", Times.Never());
		}

		/// <summary>
		/// Должен конвертировать команду в ExceptionHandlerCommand
		/// </summary>
		[Test]
		public void ShouldConvertReturnExceptionHandlerCommand()
		{
			//Given //When
			var actualCommand = _target.ConvertWithDefaultParams();

			//Then
			actualCommand.Should().BeOfType<ExceptionHandlerCommand>();
		}

		/// <summary>
		/// Должен не создавать новую ExceptionHandlerCommand, поверх переданной 
		/// </summary>
		[Test]
		public void ShouldConvertDoNothingWhenValueIsExceptionHandlerCommand()
		{
			//Given 
			var expectedCommand = new ExceptionHandlerCommand(new DelegateCommand(p => {}), 
				new Action<Exception, string>((exception, s) => {}));
			//When
			var actualCommand = _target.ConvertWithDefaultParams(expectedCommand);

			//Then
			actualCommand.Should().BeSameAs(expectedCommand);
		}

		/// <summary>
		/// Конвертированная команда должна регистрировать ошибки в ICommandExceptionRegistrator
		/// </summary>
		[Test]
		public void ShouldConvertedCommandRegisterExceptionInICommandExceptionRegistrator()
		{
			//Given
 			var mockRegistrator = new Mock<ICommandExceptionRegistrator>();
			_mockTarget.Protected()
				.Setup<ICommandExceptionRegistrator>("GetCommandExceptionRegistrator")
				.Returns(mockRegistrator.Object);

			var expectedException = new Exception("111");
			var expectedCommand = new DelegateCommand(p => { throw expectedException; });
			
			//When
			var actualCommand = (ICommand)_target.ConvertWithDefaultParams(expectedCommand);
			actualCommand.Execute(null);
			
			//Then
			mockRegistrator.Verify(p => p.RegisterCommandException(expectedException, expectedCommand), Times.Once);
		}
	}

	internal static class ExceptionHandlerCommandConverterBaseExtensions
	{
		public static object ConvertWithDefaultParams(this ExceptionHandlerCommandConverterBase target, object command = null)
		{
			if (command == null)
				command = new DelegateCommand(p => { });
			return target.Convert(command, null, null, CultureInfo.CurrentCulture);
		}
	}
}
