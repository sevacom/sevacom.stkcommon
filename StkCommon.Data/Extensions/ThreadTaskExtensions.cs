using System;
using System.Threading;
using System.Threading.Tasks;

namespace StkCommon.Data.Extensions
{
	public static class ThreadTaskExtensions
	{
		/// <summary>
		/// True - все Task созданные при помощи этого расширения запускаются синхронно.
		/// По умолчанию False
		/// </summary>
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
			return IsSync(isSyncRun) ? FromResult(action()) : Task.Factory.StartNew(action, cancellationToken);
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
				return FromResult<object>(null);
			}

			return Task.Factory.StartNew(action, cancellationToken);
		}

		/// <summary>
		/// Выполнить задачу через Task и забыть.
		/// Если action кидает Exception, то он проглатывается, поэтому если хотите знать что за ошибка произошла, то оборачивайте внутри action
		/// </summary>
		public static void Execute(Action action, bool? isSyncRun = null)
		{
			if (IsSync(isSyncRun))
				action();
			else
				Run(action, isSyncRun).ContinueWith(p => p.Dispose());
		}

		private static bool IsSync(bool? isSyncRun)
		{
			return isSyncRun ?? IsSyncRun;
		}

		private static Task<T> FromResult<T>(T result)
		{
			var tcs = new TaskCompletionSource<T>();
			tcs.SetResult(result);
			return tcs.Task;
		}
	}
}
