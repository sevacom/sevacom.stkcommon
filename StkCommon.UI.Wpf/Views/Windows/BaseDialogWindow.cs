using System;
using System.Windows;
using StkCommon.UI.Wpf.Helpers;

namespace StkCommon.UI.Wpf.Views.Windows
{
	/// <summary>
	/// Базовое окно для диалогов, скрывает иконку в заголовке и кнопки min max
	/// </summary>
	public class BaseDialogWindow : Window
	{
		/// <summary>
		/// Скрывать или нет иконку в заголовке
		/// </summary>
		protected bool IsHideIcon { get; set; }

		/// <summary>
		/// Скрывать или нет кнопки минимизации и максимизации в заголовке
		/// </summary>
		protected bool IsHideMinimizeAndMaximizeButtons { get; set; }

		public BaseDialogWindow()
		{
			IsHideIcon = true;
			IsHideMinimizeAndMaximizeButtons = true;
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			if (IsHideIcon)
				WindowHelper.RemoveIcon(this);
			if (IsHideMinimizeAndMaximizeButtons)
				WindowHelper.HideMinimizeAndMaximizeButtons(this);
		}
	}
}
