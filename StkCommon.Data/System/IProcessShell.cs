using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace StkCommon.Data.System
{
	/// <summary>
	/// Создаёт и запускает процесс
	/// </summary>
	public interface IProcessShell
	{
		/// <summary>
		/// Выполнить файл с аргументами через Explorer
		/// </summary>
		void ShellExecute(string filePath, string arguments = "");

		/// <summary>
		/// Убить запущенный ранее процесс
		/// </summary>
		void Kill(int processId);

		/// <summary>
		/// Запустить процесс с аргументом, возвращает иденитфикатор процесса или -1
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="arguments"></param>
		IShellProcess ShellExecuteWithProcess(string filePath, string arguments = "");
	}

	public class WindowsProcessShell : IProcessShell
	{
		/// <summary>
		/// Выполнить файл с аргументами через Explorer
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="arguments"></param>
		public void ShellExecute(string filePath, string arguments = "")
		{
			using (var process = CreateProcess(filePath, arguments))
			{
				StartProcess(process);
			}
		}

		/// <summary>
		/// Запустить процесс с аргументом, возвращает иденитфикатор процесса или -1
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="arguments"></param>
		public IShellProcess ShellExecuteWithProcess(string filePath, string arguments = "")
		{
			var process = CreateProcess(filePath, arguments);
			try
			{
				StartProcess(process);
			}
			catch (Exception)
			{
				process.Dispose();
				throw;
			}

			return new ShellProcess(process);
		}

		public virtual void Kill(int processId)
		{
			try
			{
				using (var process = GetProcessById(processId))
					if (process != null && !process.HasExited)
						process.Kill();
			}
			catch
			{
				// ignored
			}
		}

		public void Kill(IShellProcess process)
		{
			Kill(process.Id);
		}

		protected virtual Process GetProcessById(int id)
		{
			return Process.GetProcessById(id);
		}

		protected virtual Process CreateProcess(string filePath, string arguments)
		{
			return new Process
			{
				StartInfo =
				{
					FileName = filePath,
					Arguments = arguments,
					UseShellExecute = true
				}
			};
		}

		protected virtual void StartProcess(Process process)
		{
			var fileName = process.StartInfo.FileName;
			var arguments = process.StartInfo.Arguments;
			try
			{
				process.Start();
			}
			catch (Exception e)
			{
				var message = string.Format("Не удалось открыть файл \"{0}\"", fileName);
				if (!string.IsNullOrEmpty(arguments))
					message += string.Format(" аргументы \"{0}\"", arguments);

				throw new ShellExecuteException(message + ".", e);
			}
		}
	}

	[Serializable]
	public class ShellExecuteException : ApplicationException
	{
		public ShellExecuteException()
		{
		}

		public ShellExecuteException(string message)
			: base(message)
		{
		}

		public ShellExecuteException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected ShellExecuteException(
			SerializationInfo info,
			StreamingContext context)
			: base(info, context)
		{
		}
	}
}