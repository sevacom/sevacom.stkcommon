using System;
using StkCommon.Data.Utils;

namespace StkCommon.Data.Diagnostics
{
	/// <summary>
	/// Базовая реализация IExceptionHandler с возможностью переопределения механизма регистрации ошибок
	/// Если logger не указан, то регистрация ошибок не работает
	/// </summary>
	public class ExceptionHandlerBase : IExceptionHandler
	{
		private readonly ISimpleLogger _logger;

		/// <summary>
		/// Инициализирует регистрацию ошибок через EmptySimpleLogger (т.е. без регистрации ошибок в логе)
		/// </summary>
		protected ExceptionHandlerBase()
			: this(EmptySimpleLogger.EmptyLogger)
		{
		}

		/// <summary>
		/// Инициализирует регистрацию ошибок через ISimpleLogger
		/// </summary>
		/// <param name="logger"></param>
		public ExceptionHandlerBase(ISimpleLogger logger)
		{
			if (logger == null) throw new ArgumentNullException("logger");
			_logger = logger;
		}

		/// <summary>
		/// Тихий обработчик исключений, логирующий и давящий исключения.
		/// </summary>
		/// <param name="method">Делегат который нужно вызвать в обработчике.</param>
		/// <param name="errorMessage">Дополнительное сообщение для логирования в случае падения.</param>
		/// <param name="exceptionNotifier">Дополнительный информатор для оповещения об исключении.</param>
		public void HandleActionSilent(Action method, string errorMessage, IExceptionNotifier exceptionNotifier = null)
		{
			if (method == null) throw new ArgumentNullException("method");

			HandleActionSilent<object>(() =>
			{
				method();
				return null;
			}, errorMessage, exceptionNotifier);
		}

		/// <summary>
		/// Тихий обработчик исключений, логирующий и давящий исключения.
		/// </summary>
		/// <param name="method">Делегат который нужно вызвать в обработчике.</param>
		/// <param name="errorMessage">Дополнительное сообщение для логирования в случае падения.</param>
		/// <param name="exceptionNotifier">Дополнительный информатор для оповещения об исключении.</param>
		public T HandleActionSilent<T>(Func<T> method, string errorMessage, IExceptionNotifier exceptionNotifier = null)
		{
			if (method == null) throw new ArgumentNullException("method");

			try
			{
				return method();
			}
			catch (Exception ex)
			{
				RegisterException(ex, errorMessage, exceptionNotifier);
				return default(T);
			}
		}

		/// <summary>
		/// Обернуть возникшее исключение
		/// <param name="method">Делегат который нужно вызвать в обработчике.</param>
		/// <param name="errorMessage">Дополнительное сообщение для логирования в случае падения.</param>
		/// </summary>
		public void WrapAndThrow<TException>(Action method, string errorMessage) where TException : Exception
		{
			WrapAndThrow<TException, object>(() =>
			{
				method();
				return null;
			}, errorMessage);
		}

		/// <summary>
		/// Обернуть возникшее исключение
		/// <param name="method">Делегат который нужно вызвать в обработчике.</param>
		/// <param name="errorMessage">Дополнительное сообщение для логирования в случае падения.</param>
		/// </summary>
		public TResult WrapAndThrow<TException, TResult>(Func<TResult> method, string errorMessage)
			where TException : Exception
		{
			if (method == null) throw new ArgumentNullException("method");

			try
			{
				return method();
			}
			catch (Exception ex)
			{
				RegisterException(ex, errorMessage);
				throw WrapException<TException>(errorMessage, ex);
			}
		}

		/// <summary>
		/// Стандартный сценарий регистрации исключения в логе и оповещении.
		/// </summary>
		/// <param name="ex">Исключение для регистрации.</param><param name="message">Дополнительное сообщение для логирования в случае падения.</param>
		/// <param name="exceptionNotifier">Дополнительный информатор для оповещения об исключении.</param>
		public virtual void RegisterException(Exception ex, string message = null, IExceptionNotifier exceptionNotifier = null)
		{
			var errorId = _logger.Error(ex.Message, ex);

			if (exceptionNotifier != null)
				exceptionNotifier.Notify(ex, message, errorId);
		}

		/// <summary>
		/// Стандартный сценарий регистрации сообщения об ошибке.
		/// </summary>
		/// <param name="message">Сообщение об ошибке.</param><param name="exceptionNotifier">Дополнительный информатор для оповещения об исключении.</param>
		public virtual void RegisterException(string message, IExceptionNotifier exceptionNotifier = null)
		{
			if (string.IsNullOrEmpty(message))
				throw new ArgumentNullException("message");

			var errorId = _logger.Error(message);

			if (exceptionNotifier != null)
				exceptionNotifier.Notify(message, errorId);
		}

		/// <summary>
		/// Безопасное освобождение ресурсов
		/// </summary>
		/// <typeparam name="TDisposable"></typeparam>
		/// <param name="disposable"></param>
		/// <param name="errorMessage"></param>
		/// <param name="exceptionNotifier"></param>
		public void SafelyDispose<TDisposable>(ref TDisposable disposable, string errorMessage = null,
			IExceptionNotifier exceptionNotifier = null) where TDisposable : IDisposable
		{
			try
			{
				DisposeHelper.SafeDispose(ref disposable);
			}
			catch (Exception ex)
			{
				if (errorMessage == null)
					errorMessage = "Ошибка освобождения ресурсов для " + disposable.GetType().Name;

				RegisterException(ex, errorMessage, exceptionNotifier);
			}
		}

		private static T WrapException<T>(string message, Exception ex) where T : Exception
		{
			var constructorInfo = typeof (T).GetConstructor(new[] {typeof (string), typeof (Exception)});
			if (constructorInfo != null)
				return (T) constructorInfo.Invoke(new object[] {message, ex});

			throw new InvalidOperationException(
				"Не удалось обернуть исключение, т.к. нет подходящего конструтора (String, Exception).", ex);
		}
	}
}