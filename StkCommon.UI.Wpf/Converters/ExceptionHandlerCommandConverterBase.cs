using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using StkCommon.UI.Wpf.Commands;
using StkCommon.UI.Wpf.ViewModels;

namespace StkCommon.UI.Wpf.Converters
{
	/// <summary>
	/// Используется для при биндинге команд, которые необходимо обернуть в обработчик ошибок с поднятием сообщений.
	/// </summary>
	[ValueConversion(typeof(ICommand), typeof(ICommand))]
	public abstract class ExceptionHandlerCommandConverterBase : ValueConverterBase
	{
		#region IValueConverter Members

		public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (ViewModelBase.IsInDesignModeStatic || value == null)
				return value;
			if (value is ExceptionHandlerCommand)
				return value;
			return new ExceptionHandlerCommand((ICommand)value, RegisterCommandException);
		}

		public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		#endregion

		/// <summary>
		/// Получить экзмпляр регистратора ошибок для команд
		/// Рекомендовано тут возвращать null при этом переопределить метод RegisterException и записывать уже готовое сообщение 
		/// </summary>
		/// <returns>при возврате null надо переопределить метод RegisterException</returns>
		protected virtual ICommandExceptionRegistrator GetCommandExceptionRegistrator()
		{
			return null;
		}

		private void RegisterCommandException(Exception exception, ICommand command)
		{
			var registrator = GetCommandExceptionRegistrator();
			if(registrator != null)
				registrator.RegisterCommandException(exception, command);
			else
				RegisterException(exception, ExceptionHandlerCommand.FormatCommandToErrorMessage(command));
		}

		/// <summary>
		/// Регистрация уже форматированной стандартным образом ошибки по команде
		/// Если переопределяете этот метод то GetCommandExceptionRegistrator() должен возвращать NULL
		/// </summary>
		/// <param name="exception"></param>
		/// <param name="message"></param>
		protected virtual void RegisterException(Exception exception, string message)
		{
			
		}
	}
}
