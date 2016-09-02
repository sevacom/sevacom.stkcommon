using System;
using System.Diagnostics;

namespace StkCommon.Data.System
{
	/// <summary>
	/// Интрефейс для управления процессом
	/// </summary>
	public interface IShellProcess : IDisposable
	{
		/// <summary>
		/// Идентификатор процесса
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Подождать пока процесс запустится
		/// </summary>
		void WaitStartup();
	}

	internal class ShellProcess : IShellProcess
	{
		private Process _process;

		public ShellProcess(Process process)
		{
			if (process == null) throw new ArgumentNullException("process");
			_process = process;
		}

		public int Id
		{
			get
			{
				try
				{
					return _process.Id;
				}
				catch (InvalidOperationException)
				{
					return -1;
				}
			}
		}

		public void WaitStartup()
		{
			_process.WaitForInputIdle();
		}

		public void Dispose()
		{
			if (_process != null)
			{
				_process.Dispose();
				_process = null;
			}
		}
	}
}