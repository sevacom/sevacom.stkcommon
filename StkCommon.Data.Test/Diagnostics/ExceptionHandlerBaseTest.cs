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
		private readonly Exception _expectedException = new Exception("Test");
		private const string ExpectedErrorMessage = "TestErrorMessage";
		private ExceptionHandlerTest _target;
		private IExceptionNotifier _expectedNotifier;
		private readonly object _expectedActionResult = new object();

		[SetUp]
		public void SetUp()
		{
			_target = new ExceptionHandlerTest();
			_expectedNotifier = Mock.Of<IExceptionNotifier>();
		}

		/// <summary>
		/// Exception должен давиться и регистрироваться
		/// </summary>
		[Test]
		public void ShouldHandleActionSilentRegisterExceptionWhenThrowException()
		{
			//Given //When
			Assert.DoesNotThrow(() => _target.HandleActionSilent(ExceptionAction, ExpectedErrorMessage, _expectedNotifier));

			//Then
			_target.RegistredException.Should().Be(_expectedException, "Exception не зарегистрирована");
			_target.RegistredMessage.Should().Be(ExpectedErrorMessage, "ErrorMessage не зарегистрирована");
			_target.RegistredExceptionNotifier.Should().Be(_expectedNotifier);
		}

		/// <summary>
		/// Метод возвращающий результат должен отрабатывать
		/// </summary>
		[Test]
		public void ShouldHandleActionSilentWorkWhenNotThrowException()
		{
			//Given //When
			var actualResult = _target.HandleActionSilent(() => ResultAction(), ExpectedErrorMessage, _expectedNotifier);

			//Then
			actualResult.Should().Be(_expectedActionResult, "Возвращаемый результат метода не совпадает");
			_target.RegistredException.Should().Be(null);
			_target.RegistredMessage.Should().Be(null);
			_target.RegistredExceptionNotifier.Should().Be(null);
		}

		/// <summary>
		/// Должен оборачивать Exception и регистрировать 
		/// </summary>
		[Test]
		public void ShouldWrapAndThrowException()
		{
			//Given //When
			Assert.Throws<WrapException>(() => _target.WrapAndThrow<WrapException>(ExceptionAction, ExpectedErrorMessage));

			//Then
			_target.RegistredException.Should().Be(_expectedException, "Exception не зарегистрирована");
			_target.RegistredMessage.Should().Be(ExpectedErrorMessage, "ErrorMessage не зарегистрирована");
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
			_target.SafelyDispose(ref disposable, ExpectedErrorMessage);
			
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
			Assert.DoesNotThrow(() => _target.SafelyDispose(ref disposable, ExpectedErrorMessage));
		}

		/// <summary>
		/// Должен подавить исключение в Dispose и зарегистрировать его
		/// </summary>
		[Test]
		public void ShouldRegisterExceptionWhenDisposeThrowExcpetion()
		{
			//Given 
			var disposableMock = new Mock<IDisposable>();
			disposableMock.Setup(p => p.Dispose()).Throws(_expectedException);
			var disposable = disposableMock.Object;

			//When
			Assert.DoesNotThrow(() => _target.SafelyDispose(ref disposable, ExpectedErrorMessage));

			//Then
			_target.RegistredException.Should().Be(_expectedException, "Exception не зарегистрирована");
			_target.RegistredMessage.Should().Be(ExpectedErrorMessage, "ErrorMessage не зарегистрирована");
		}

		#region Support methods and classes

		private void ExceptionAction()
		{
			throw _expectedException;
		}

		private object ResultAction()
		{
			return _expectedActionResult;
		}

		private class ExceptionHandlerTest : ExceptionHandlerBase
		{
			public Exception RegistredException { get; private set; }

			public string RegistredMessage { get; private set; }

			public IExceptionNotifier RegistredExceptionNotifier { get; private set; }

			#region Overrides of ExceptionHandlerBase

			/// <summary>
			/// Стандартный сценарий регистрации исключения в логе и оповещении.
			/// </summary>
			/// <param name="ex">Исключение для регистрации.</param>
			/// <param name="message">Дополнительное сообщение для логирования в случае падения.</param>
			/// <param name="exceptionNotifier">Дополнительный информатор для оповещения об исключении.</param>
			public override void RegisterException(Exception ex, string message = null, IExceptionNotifier exceptionNotifier = null)
			{
				RegistredException = ex;
				RegistredMessage = message;
				RegistredExceptionNotifier = exceptionNotifier;
			}

			/// <summary>
			/// Стандартный сценарий регистрации сообщения об ошибке.
			/// </summary>
			/// <param name="message">Сообщение об ошибке.</param>
			/// <param name="exceptionNotifier">Дополнительный информатор для оповещения об исключении.</param>
			public override void RegisterException(string message, IExceptionNotifier exceptionNotifier = null)
			{
				RegistredMessage = message;
				RegistredExceptionNotifier = exceptionNotifier;
			}

			#endregion
		}

		private class WrapException : Exception
		{
			public WrapException(string message, Exception ex)
				: base(message, ex)
			{

			}
		}

		#endregion

	}

}
