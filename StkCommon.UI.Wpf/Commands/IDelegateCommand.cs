using System.ComponentModel;
using System.Windows.Input;
using StkCommon.UI.Wpf.Model;

namespace StkCommon.UI.Wpf.Commands
{
	/// <summary>
	/// Интерфейс команды с поддержкой имени команды, для удобства записи в логи и отображения пользователю имени команды
	/// </summary>
	public interface IDelegateCommand : ICommand, INotifyPropertyChanged
	{
		/// <summary>
		/// Имя команды, может быть пустым
		/// </summary>
		string CommandName { get; }

		/// <summary>
		/// Горячая клавиша
		/// </summary>
		ShortCut HotKey { get; set; }

		/// <summary>
		///     Raises the CanExecuteChaged event
		/// </summary>
		void RaiseCanExecuteChanged();
	}
}