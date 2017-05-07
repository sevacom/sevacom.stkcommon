using System;

namespace StkCommon.Data.Extensions
{
	/// <summary>
	/// Расширения для работы DateTime, позволяющие переопределять поведение в тестах.
	/// Например: DateTimeEx.Now вместо DateTime.Now
	/// </summary>
	public static class DateTimeEx
	{
		[ThreadStatic]
		private static IDateTimeProvider _provider;
		
		[ThreadStatic]
		private static DateTime? _nowDateTime;

		/// <summary>
		/// Установить значение, которое будет возвращать свойство Now
		/// Сделан для тестов.
		/// </summary>
		public static void MockNow(DateTime now)
		{
			_nowDateTime = now;
		}

		/// <summary>
		/// Установить провайдер, через который будут направляться вызовы
		/// Сделан для тестов, чтобы проверить обращения.
		/// </summary>
		/// <param name="provider"></param>
		public static void Mock(IDateTimeProvider provider)
		{
			if (provider == null) throw new ArgumentNullException("provider");
			_provider = provider;
		}

		/// <summary>
		/// Сбросить переопределённое поведение (вернуть к изначальному)
		/// </summary>
		public static void Reset()
		{
			_nowDateTime = null;
			_provider = null;
		}

		/// <summary>
		/// Возвращает DateTime.Now, если поведение не переопределили.
		/// </summary>
		public static DateTime Now
		{
			get
			{
				return _provider != null ? 
					_provider.Now : 
					_nowDateTime.GetValueOrDefault(DateTime.Now);
			}
		}
	}

	public interface IDateTimeProvider
	{
		DateTime Now { get; }
	}
}