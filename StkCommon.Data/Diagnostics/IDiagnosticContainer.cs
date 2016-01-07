using System;

namespace StkCommon.Data.Diagnostics
{
	/// <summary>
	/// Регистрации и оповещение пользователя о каких-то исключительных ситуациях (ошибках, предупреждениях)
	/// </summary>
	public interface IDiagnosticContainer
	{
		IExceptionNotifier ExceptionNotifier { get; }

		IExceptionHandler ExceptionHandler { get; }

		ISimpleLogger Logger { get; }

		void SafelyDispose<TDisposable>(ref TDisposable disposable) where TDisposable : IDisposable;

		void SafelyDispose<TDisposable>(ref TDisposable disposable, string errorMessage) where TDisposable : IDisposable;

		void BeginSilentAndNotify(Action method, string errorMessage);

		T BeginSilentAndNotify<T>(Func<T> method, string errorMessage);

		void RegisterException(Exception ex, string message = null);

		void RegisterException(string message);

	}

	/// <summary>
	/// Регистрации и оповещение пользователя о каких-то исключительных ситуациях (ошибках, предупреждениях)
	/// </summary>
	public class DiagnosticContainer : IDiagnosticContainer
	{
		public DiagnosticContainer(ISimpleLogger logger, IExceptionHandler exceptionHandler, IExceptionNotifier exceptionNotifier)
		{
			if (logger == null) throw new ArgumentNullException("logger");
			if (exceptionHandler == null) throw new ArgumentNullException("exceptionHandler");
			if (exceptionNotifier == null) throw new ArgumentNullException("exceptionNotifier");
			ExceptionHandler = exceptionHandler;
			ExceptionNotifier = exceptionNotifier;
			Logger = logger;
		}

		public IExceptionNotifier ExceptionNotifier { get; private set; }

		public IExceptionHandler ExceptionHandler { get; private set; }

		public ISimpleLogger Logger { get; private set; }

		public void SafelyDispose<TDisposable>(ref TDisposable disposable) where TDisposable : IDisposable
		{
			SafelyDispose(ref disposable, null);
		}

		public void SafelyDispose<TDisposable>(ref TDisposable disposable, string errorMessage) where TDisposable : IDisposable
		{
			ExceptionHandler.SafelyDispose(ref disposable, errorMessage, ExceptionNotifier);
		}

		public void BeginSilentAndNotify(Action method, string errorMessage)
		{
			BeginSilentAndNotify<object>(() =>
			{
				method();
				return null;
			}, errorMessage);
		}

		public T BeginSilentAndNotify<T>(Func<T> method, string errorMessage)
		{
			if (method == null) throw new ArgumentNullException("method");
			return ExceptionHandler.HandleActionSilent(method, errorMessage, ExceptionNotifier);
		}

		public void RegisterException(Exception ex, string message = null)
		{
			ExceptionHandler.RegisterException(ex, message, ExceptionNotifier);
		}

		public void RegisterException(string message)
		{
			ExceptionHandler.RegisterException(message, ExceptionNotifier);
		}
	}
}
