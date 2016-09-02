using System;
using System.Threading;
using System.Threading.Tasks;

namespace StkCommon.Data.Common
{
	public static class ThreadTaskExtension
	{
		public static bool IsSyncRun { get; set; }

		/// <summary>
		/// Выполнить action асинхронно или синхронно используется для возможности тестирования
		/// </summary>
		public static Task<TResult> Run<TResult>(Func<TResult> action, bool? isSyncRun = null)
		{
			return Run(action, CancellationToken.None, isSyncRun);
		}

		/// <summary>
		/// Выполнить action асинхронно или синхронно используется для возможности тестирования
		/// </summary>
		public static Task<TResult> Run<TResult>(Func<TResult> action, CancellationToken cancellationToken, bool? isSyncRun = null)
		{
			return IsSync(isSyncRun) ? Task.FromResult(action()) : Task.Run(action, cancellationToken);
		}

		/// <summary>
		/// Выполнить action асинхронно или синхронно используется для возможности тестирования
		/// </summary>
		public static Task Run(Action action, bool? isSyncRun = null)
		{
			return Run(action, CancellationToken.None, isSyncRun);
		}

		/// <summary>
		/// Выполнить action асинхронно или синхронно используется для возможности тестирования
		/// </summary>
		public static Task Run(Action action, CancellationToken cancellationToken, bool? isSyncRun = null)
		{
			if (IsSync(isSyncRun))
			{
				action();
				return Task.FromResult<object>(null);
			}

			return Task.Run(action, cancellationToken);
		}

		/// <summary>
		/// Проверяет с задержкой исполняется ли задание или нет
		/// </summary>
		/// <returns>true - исполняется, false - завершилось</returns>
		public static async Task<bool> IsLoading(this Task waitTask, bool? isSyncRun = null, TimeSpan? waitDelta = null)
		{
			if (IsSync(isSyncRun))
			{
				return false;
			}

			await Task.Delay(waitDelta.GetValueOrDefault(TimeSpan.FromMilliseconds(250)));
			return !waitTask.IsCompleted;
		}

		private static bool IsSync(bool? isSyncRun)
		{
			return (null == isSyncRun ? IsSyncRun : isSyncRun.Value);
		}
	}
}
