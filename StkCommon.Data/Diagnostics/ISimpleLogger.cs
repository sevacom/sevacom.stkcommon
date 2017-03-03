using System;

namespace StkCommon.Data.Diagnostics
{
	/// <summary>
	/// Логирование, базовый интерфейс
	/// </summary>
	public interface ISimpleLogger
	{
		bool IsDebugEnabled { get; }

		void Fatal(string message);
		void Fatal(string message, Exception ex);

		/// <summary>
		/// Ошибка
		/// </summary>
		/// <returns>идентификатор ошибки</returns>
		string Error(string message);

		/// <summary>
		/// Ошибка
		/// </summary>
		/// <returns>идентификатор ошибки</returns>
		string Error(string message, Exception ex);

		void Debug(string message);
		void Debug(string message, Exception exception);

		void Warn(string message);

		void Info(string message);
	}

	public class EmptySimpleLogger: ISimpleLogger
	{
		public static readonly ISimpleLogger EmptyLogger = new EmptySimpleLogger();

		public bool IsDebugEnabled { get; private set; }
		
		public void Fatal(string message)
		{
		}

		public void Fatal(string message, Exception ex)
		{
		}

		public string Error(string message)
		{
			return string.Empty;
		}

		public string Error(string message, Exception ex)
		{
			return string.Empty;
		}

		public void Debug(string message)
		{
		}

		public void Debug(string message, Exception exception)
		{
		}

		public void Warn(string message)
		{
		}

		public void Info(string message)
		{
		}
	}
}