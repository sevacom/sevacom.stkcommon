using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using StkCommon.Data.Text;
using StkCommon.UI.Wpf.Commands;

namespace StkCommon.UI.Wpf.ViewModels
{
	/// <summary>
	/// Вьюмодель диалога сообщения об ошибке
	/// </summary>
	public class ErrorMessageViewModel : ViewModelBase
	{
		private bool? _finishResult;
        private bool _showDetails;

		#region Constructors

		public ErrorMessageViewModel(string caption, string message, string details)           
        {
			if (caption == null) throw new ArgumentNullException("caption");
			if (message == null) throw new ArgumentNullException("message");
			
			Caption = caption;
			Message = message;
            Details = details;
            ShowDetails = false;

			CloseCommand = new DelegateCommand(CloseCommandHandler);
            ShowDetailsCommand = new DelegateCommand(ShowDetailsCommandHandler);

			var icon = SystemIcons.Error;
			ErrorIcon = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(icon.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
			ErrorIcon.Freeze();
        }

		#endregion

		#region Public properties

		/// <summary>
		/// Признак закрытия диалога
		/// </summary>
		public bool? FinishResult
		{
			get { return _finishResult; }
			set
			{
				if (value == _finishResult)
				{
					return;
				}
				_finishResult = value;
				OnPropertyChanged(() => FinishResult);
			}
		}

		/// <summary>
		/// Заголовок диалога
		/// </summary>
		public string Caption { get; protected set; }

		/// <summary>
		/// Сообщение об ошибке
		/// </summary>
		public string Message { get; protected set; }

		/// <summary>
		/// Подробная информация об ошибке
		/// </summary>
		public string Details { get; protected set; }

		/// <summary>
		/// Признак отображения подробной информации об ошибке
		/// </summary>
		public bool ShowDetails
		{
			get { return string.IsNullOrWhiteSpace(Details) || _showDetails; }
			protected set
			{
				if (value == _showDetails)
				{
					return;
				}
				_showDetails = value;
				OnPropertyChanged(() => ShowDetails);
			}
		}

		/// <summary>
		/// Иконка информации/предупреждения/ошибки
		/// </summary>
		public ImageSource ErrorIcon { get; private set; }

		#endregion

		#region Commands

		/// <summary>
		/// Команда закрытия окна
		/// </summary>
		public ICommand CloseCommand { get; private set; }
		
		/// <summary>
		/// Команда отображения подробной информации
		/// </summary>
		public ICommand ShowDetailsCommand { get; private set; }

		#endregion

		#region Handlers

		private void CloseCommandHandler(object obj)
		{
			FinishResult = true;
		}

		private void ShowDetailsCommandHandler(object obj)
		{
			ShowDetails = !ShowDetails;
		}

		#endregion

		#region Methods

		/// <summary>
		/// Получить всю информацию из вьюмодели в виде текста
		/// </summary>
		public string GetFullInfoText()
		{
			return TextExtensions.JoinNotEmpty(Environment.NewLine + Environment.NewLine, 
				Caption, Message, Details);
		}

		#endregion
	}
}
