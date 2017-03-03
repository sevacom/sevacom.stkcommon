using System;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using StkCommon.Data.Diagnostics;

namespace StkCommon.Data.Test.Diagnostics
{
	[TestFixture]
	public class ExceptionHandlerBaseTest
	{
		private Exception _expectedException;
		private string _expectedErrorMessage;
		private ExceptionHandlerBase _target;
		private Mock<IExceptionNotifier> _notifier;
		private readonly object _expectedActionResult = new object();
		private Mock<ISimpleLogger> _logger;
		private string _expectedErrorId;

		[SetUp]
		public void SetUp()
		{
			_expectedException = new Exception("Test");
			_expectedErrorId = "ErrorId";
			_expectedErrorMessage = "TestErrorMessage";

			_logger = new Mock<ISimpleLogger>();
			_logger
				.Setup(p => p.Error(It.IsAny<string>(), It.IsAny<Exception>()))
				.Returns(_expectedErrorId);
			_logger
				.Setup(p => p.Error(It.IsAny<string>()))
				.Returns(_expectedErrorId);

			_notifier = new Mock<IExceptionNotifier>();
			_target = new ExceptionHandlerBase(_logger.Object);
		}

		/// <summary>
		/// Exception должен давиться и регистрироваться
		/// </summary>
		[Test]
		public void ShouldHandleActionSilentRegisterExceptionWhenThrowException()
		{
		    //Given //When
			Assert.DoesNotThrow(() => _target.HandleActionSilent(ExceptionAction, 
				_expectedErrorMessage, _notifier.Object));

            //Then
            VerifyWriteAndNotifyExpectedError();
		}

	    /// <summary>
		/// Метод возвращающий результат должен отрабатывать
		/// </summary>
		[Test]
		public void ShouldHandleActionSilentWorkWhenNotThrowException()
		{
			//Given //When
			var actualResult = _target.HandleActionSilent(() => ResultAction(), _expectedErrorMessage, _notifier.Object);

			//Then
			actualResult.Should().Be(_expectedActionResult, "Возвращаемый результат метода не совпадает");
			_logger.Verify(p => p.Error(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
			_notifier.Verify(p => p.Notify(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		}

		/// <summary>
		/// Должен оборачивать Exception и регистрировать 
		/// </summary>
		[Test]
		public void ShouldWrapAndThrowExceptionWithRegister()
		{
			//Given //When
			Assert.Throws<WrapException>(() => _target.WrapAndThrow<WrapException>(ExceptionAction, _expectedErrorMessage));

			//Then
			_logger.Verify(p => p.Error(_expectedErrorMessage, _expectedException), Times.Once);
			_notifier.Verify(p => p.Notify(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		}

		/// <summary>
		/// Должен вызывать Dispose
		/// </summary>
		[Test]
		public void ShouldDisposeCorrect()
		{
			//Given 
			var disposableMock = new Mock<IDisposable>();
			var disposable = disposableMock.Object;
			
			//When
			_target.SafelyDispose(ref disposable, _expectedErrorMessage);
			
			//Then
			disposableMock.Verify(p => p.Dispose(), Times.Once(), "Метод Dispose не вызван");
		}

		/// <summary>
		/// Должен без ошибок выполнять Dispose для null объекта
		/// </summary>
		[Test]
		public void ShouldDisposeNullObject()
		{
			//Given 
			IDisposable disposable = null;

			//When//Then
			Assert.DoesNotThrow(() => _target.SafelyDispose(ref disposable, _expectedErrorMessage));
		}

		/// <summary>
		/// Должен подавить исключение в Dispose и зарегистрировать его
		/// </summary>
		[Test]
		public void ShouldRegisterExceptionAndNotifyWhenDisposeThrowExcpetion()
		{
			//Given 
			var disposableMock = new Mock<IDisposable>();
			disposableMock.Setup(p => p.Dispose()).Throws(_expectedException);
			var disposable = disposableMock.Object;

			//When
			Assert.DoesNotThrow(() => _target.SafelyDispose(ref disposable, _expectedErrorMessage, _notifier.Object));

            //Then
            VerifyWriteAndNotifyExpectedError();
		}

		/// <summary>
		/// Должен регистрировать ошибку в логере и передавать в нотификатор.
		/// Если сообщение пустое то в логер записывается сообщение из Exception
		/// </summary>
		[Test]
		public void ShouldRegisterExceptionWriteToLoggerAndNotifyWhenEmptyMessage()
		{
			//Given 
			//When
			_target.RegisterException(_expectedException, null, _notifier.Object);
			
			//Then
			_logger.Verify(p => p.Error(_expectedException.Message, _expectedException), Times.Once);
			_notifier.Verify(p => p.Notify(_expectedException, null, _expectedErrorId), Times.Once);
		}

		[Test]
		public void ShouldRegisterExceptionWriteOnlyToLoggerWhenNotifierNull()
		{
			//Given 
			//When
			_target.RegisterException(_expectedErrorMessage);

			//Then
			_logger.Verify(p => p.Error(_expectedErrorMessage), Times.Once);
			_notifier.Verify(p => p.Notify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
		}

        private void VerifyWriteAndNotifyExpectedError()
        {
            _logger.Verify(p => p.Error(_expectedErrorMessage, _expectedException), Times.Once);
            _notifier.Verify(p => p.Notify(_expectedException, _expectedErrorMessage, _expectedErrorId), Times.Once);
        }

        private void ExceptionAction()
		{
			throw _expectedException;
		}

		private object ResultAction()
		{
			return _expectedActionResult;
		}


		private class WrapException : Exception
		{
			public WrapException(string message, Exception ex)
				: base(message, ex)
			{

			}
		}
	}

}
