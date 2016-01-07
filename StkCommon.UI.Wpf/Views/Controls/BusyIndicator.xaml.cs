using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace StkCommon.UI.Wpf.Views.Controls
{
	/// <summary>
	/// Индикатор загрузки данных
	/// Interaction logic for BusyIndicator.xaml
	/// </summary>
	public partial class BusyIndicator : UserControl
	{
		#region DependencyProperties

		/// <summary>
		/// Включение видимости и анимации
		/// </summary>
		public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
			   "IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false, (o, args) => ((BusyIndicator)o).IsBusyPropertyChanged(args)));

		/// <summary>
		/// Цвет вращающегося элемента
		/// </summary>
		public static readonly DependencyProperty FillProperty = DependencyProperty.Register(
			"Fill", typeof(Brush), typeof(BusyIndicator), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(239, 239, 239))));

		#endregion

		private Storyboard _storyBoard;

		/// <summary>
		/// Включение видимости и анимации
		/// </summary>
		public bool IsBusy
		{
			get { return (bool)GetValue(IsBusyProperty); }
			set { SetValue(IsBusyProperty, value); }
		}

		/// <summary>
		/// Цвет вращающегося элемента
		/// </summary>
		public Brush Fill
		{
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}

		public BusyIndicator()
		{
			InitializeComponent();
		}

		#region Handlers

		private void BusyIndicatorOnLoaded(object sender, RoutedEventArgs e)
		{
			if (IsBusy && _storyBoard == null)
			{
				_storyBoard = (Storyboard)FindResource("RotationStoryboard");
				_storyBoard.Begin();
			}
		}

		private void IsBusyPropertyChanged(DependencyPropertyChangedEventArgs args)
		{
			if ((bool)args.NewValue && _storyBoard == null)
			{
				_storyBoard = (Storyboard)FindResource("RotationStoryboard");
				_storyBoard.Begin();
			}
			else if (_storyBoard != null)
			{
				_storyBoard.Stop();
				_storyBoard = null;
			}
		}

		#endregion
	}
}
