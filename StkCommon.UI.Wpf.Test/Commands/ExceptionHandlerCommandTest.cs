using System;
using System.Windows.Input;
using Moq;
using NUnit.Framework;
using StkCommon.UI.Wpf.Commands;

namespace StkCommon.UI.Wpf.Test.Commands
{
	[TestFixture]
	public class ExceptionHandlerCommandTest
	{
		private DelegateCommand _expectedCommand;
		private Exception _expectedException;
		private Mock<Action<Exception, ICommand>> _registerCommandException;
		private ExceptionHandlerCommand _target;

		[SetUp]
		public void Setup()
		{
			_expectedException = new Exception("Test");
			_expectedCommand = new DelegateCommand(p => { throw _expectedException; });
			_registerCommandException = new Mock<Action<Exception, ICommand>>();
			_target = new ExceptionHandlerCommand(_expectedCommand, _registerCommandException.Object);
		}
		/// <summary>
		/// должен регистрировать ошибку, когда при выполнении команды она произошла
		/// </summary>
		[Test]
		public void ShouldRegisterExceptionWhenCommandExecuteRiseException()
		{
			//Given //When
			_target.Execute(null);

			//Then
			_registerCommandException.Verify(p => p(_expectedException, _expectedCommand), 
				Times.Once(), "Регистрация ошибки не произошла");
		}

		/// <summary>
		/// должен регистрировать ошибку, если она произошла при проверке CanExecute
		/// </summary>
		[Test]
		public void ShouldRegisterExceptionWhenCommandCanExecuteRiseException()
		{
			//Given
			_expectedCommand = new DelegateCommand(p => { }, o => { throw _expectedException; });
			_target = new ExceptionHandlerCommand(_expectedCommand, _registerCommandException.Object);

			//When
			_target.CanExecute(null);

			//Then
			_registerCommandException.Verify(p => p(_expectedException, _expectedCommand),
				Times.Once(), "Регистрация ошибки не произошла");
		}

	}
}
