using System;

namespace StkCommon.Data.Diagnostics
{
	/// <summary>
	/// Набор типичных сценариев для обработки исключений.
	/// </summary>
	public interface IExceptionHandler
	{
		/// <summary>
		/// Тихий обработчик исключений, логирующий и давящий исключения.
		/// </summary>
		/// <param name="method">Делегат который нужно вызвать в обработчике.</param>
		/// <param name="errorMessage">Дополнительное сообщение для логирования в случае падения.</param>
		/// <param name="exceptionNotifier">Дополнительный информатор для оповещения об исключении.</param>
		void HandleActionSilent(Action method, string errorMessage, IExceptionNotifier exceptionNotifier = null);

		/// <summary>
		/// Тихий обработчик исключений, логирующий и давящий исключения.
		/// </summary>
		/// <param name="method">Делегат который нужно вызвать в обработчике.</param>
		/// <param name="errorMessage">Дополнительное сообщение для логирования в случае падения.</param>
		/// <param name="exceptionNotifier">Дополнительный информатор для оповещения об исключении.</param>
		T HandleActionSilent<T>(Func<T> method, string errorMessage, IExceptionNotifier exceptionNotifier = null);

		/// <summary>
		/// Обернуть возникшее исключение
		/// <param name="method">Делегат который нужно вызвать в обработчике.</param>
		/// <param name="errorMessage">Дополнительное сообщение для логирования в случае падения.</param>
		/// </summary>
		void WrapAndThrow<TException>(Action method, string errorMessage) where TException : Exception;

		/// <summary>
		/// Обернуть возникшее исключение
		/// <param name="method">Делегат который нужно вызвать в обработчике.</param>
		/// <param name="errorMessage">Дополнительное сообщение для логирования в случае падения.</param>
		/// </summary>
		TResult WrapAndThrow<TException, TResult>(Func<TResult> method, string errorMessage) where TException : Exception;

		/// <summary>
		/// Стандартный сценарий регистрации исключения в логе и оповещении.
		/// </summary>
		/// <param name="ex">Исключение для регистрации.</param>
		/// <param name="message">Дополнительное сообщение для логирования в случае падения.</param>
		/// <param name="exceptionNotifier">Дополнительный информатор для оповещения об исключении.</param>
		void RegisterException(Exception ex, string message = null, IExceptionNotifier exceptionNotifier = null);

		/// <summary>
		/// Стандартный сценарий регистрации сообщения об ошибке.
		/// </summary>
		/// <param name="message">Сообщение об ошибке.</param>
		/// <param name="exceptionNotifier">Дополнительный информатор для оповещения об исключении.</param>
		void RegisterException(string message, IExceptionNotifier exceptionNotifier = null);

		/// <summary>
		/// Безопасное освобождение ресурсов
		/// </summary>
		/// <typeparam name="TDisposable"></typeparam>
		/// <param name="disposable"></param>
		/// <param name="errorMessage"></param>
		/// <param name="exceptionNotifier"></param>
		void SafelyDispose<TDisposable>(ref TDisposable disposable, string errorMessage = null,
			IExceptionNotifier exceptionNotifier = null) where TDisposable : IDisposable;
	}
}
