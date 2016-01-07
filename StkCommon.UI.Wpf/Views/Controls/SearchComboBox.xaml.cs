using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace StkCommon.UI.Wpf.Views.Controls
{
	/// <summary>
	/// Interaction logic for SearchComboBox.xaml
	/// </summary>
	public partial class SearchComboBox
	{
		#region Fields

		private readonly DispatcherTimer _searchEventDelayTimer;

		#endregion

		#region Constructor

		public SearchComboBox()
		{
			InitializeComponent();

			_searchEventDelayTimer = new DispatcherTimer
			{
				Interval = SearchEventTimeDelay.TimeSpan
			};
			_searchEventDelayTimer.Tick += OnSeachEventDelayTimerTick;
		}

		#endregion

		#region Dependency properties

		public static readonly DependencyProperty SearchTextProperty = DependencyProperty.Register(
			"SearchText", typeof(string), typeof(SearchComboBox));

		public static readonly DependencyProperty ShowComboBoxProperty = DependencyProperty.Register(
			"ShowComboBox", typeof(bool), typeof(SearchComboBox), new FrameworkPropertyMetadata { DefaultValue = true });

		public static readonly DependencyProperty ShowLabelTextOnFocusProperty = DependencyProperty.Register(
			"ShowLabelTextOnFocus", typeof(bool), typeof(SearchComboBox), new FrameworkPropertyMetadata { DefaultValue = false });

		public static readonly DependencyProperty HasTextProperty = DependencyProperty.Register(
			"HasText", typeof(bool), typeof(SearchComboBox));

		public static readonly DependencyProperty LabelTextProperty = DependencyProperty.Register(
			"LabelText", typeof(string), typeof(SearchComboBox), new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

		public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
			"ItemsSource", typeof(object), typeof(SearchComboBox));

		public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
			"SelectedItem", typeof(object), typeof(SearchComboBox), new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

		public static readonly DependencyProperty SearchEventTimeDelayProperty =
			DependencyProperty.Register("SearchEventTimeDelay",typeof(Duration),typeof(SearchComboBox),new FrameworkPropertyMetadata(new Duration(new TimeSpan(0, 0, 0, 0, 500)),
					OnSearchEventTimeDelayChanged));

		public static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register(
			"SearchCommand", typeof(ICommand), typeof(SearchComboBox));

		public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
			"CommandParameter", typeof(object), typeof(SearchComboBox));

		#endregion

		#region Properties

		/// <summary>
		/// Текст введенный в окно поиска
		/// </summary>
		public string SearchText
		{
			get { return (string)GetValue(SearchTextProperty); }
			set { SetValue(SearchTextProperty, value); }
		}

		/// <summary>
		/// Показывать ли комбобокс
		/// </summary>
		public bool ShowComboBox
		{
			get { return (bool)GetValue(ShowComboBoxProperty); }
			set { SetValue(ShowComboBoxProperty, value); }
		}

		/// <summary>
		/// Показывать ли LabelText когда установлен фокус
		/// </summary>
		public bool ShowLabelTextOnFocus
		{
			get { return (bool)GetValue(ShowLabelTextOnFocusProperty); }
			set { SetValue(ShowLabelTextOnFocusProperty, value); }
		}

		/// <summary>
		/// Есть ли в окне ввода текст
		/// </summary>
		public bool HasText
		{
			get { return (bool)GetValue(HasTextProperty); }
			private set { SetValue(HasTextProperty, value); }
		}

		/// <summary>
		/// Текст отображающийся в окне поиска когда в нем нет текста и отсутствует клавиатурный фокус ввода
		/// </summary>
		public string LabelText
		{
			get { return (string)GetValue(LabelTextProperty); }
			set { SetValue(LabelTextProperty, value); }
		}

		/// <summary>
		/// Коллекция элементов выпадающего списка
		/// </summary>
		public object ItemsSource
		{
			get { return GetValue(ItemsSourceProperty); }
			set { SetValue(ItemsSourceProperty, value); }
		}

		/// <summary>
		/// Шаблон данных для элементов выпадающего списка
		/// </summary>
		public DataTemplate ItemTemplate
		{
			get { return ItemsComboBox.ItemTemplate; }
			set { ItemsComboBox.ItemTemplate = value; }
		}

		/// <summary>
		/// Стиль для элементов выпадающего списка 
		/// </summary>
		public Style ItemContainterStyle
		{
			get { return ItemsComboBox.ItemContainerStyle; }
			set { ItemsComboBox.ItemContainerStyle = value; }
		}

		/// <summary>
		/// Выбранный в выпадающем списке элемент
		/// </summary>
		public object SelectedItem
		{
			get { return GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		/// <summary>
		/// Временная задержка перед началом поиска
		/// </summary>
		public Duration SearchEventTimeDelay
		{
			get { return (Duration)GetValue(SearchEventTimeDelayProperty); }
			set { SetValue(SearchEventTimeDelayProperty, value); }
		}

		/// <summary>
		/// Команда поиска
		/// </summary>
		public ICommand SearchCommand
		{
			get { return (ICommand)GetValue(SearchCommandProperty); }
			set { SetValue(SearchCommandProperty, value); }
		}

		/// <summary>
		/// Параметр команды поиска
		/// </summary>
		public object CommandParameter
		{
			get { return GetValue(CommandParameterProperty); }
			set { SetValue(CommandParameterProperty, value); }
		}

		#endregion

		#region Event handlers

		private void OnSeachEventDelayTimerTick(object o, EventArgs e)
		{
			_searchEventDelayTimer.Stop();
			ExecuteSearchCommand();
		}

		private void SearchTextBoxTextChanged(object sender, TextChangedEventArgs e)
		{
			var textBox = (TextBox)sender;
			HasText = !string.IsNullOrEmpty(textBox.Text);

			_searchEventDelayTimer.Stop();
			_searchEventDelayTimer.Start();
		}

		private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				SearchTextBox.Clear();
			}
			else if (e.Key == Key.Down)
			{
				ItemsComboBox.Focus();
				ItemsComboBox.IsDropDownOpen = true;
			}
		}

		private static void OnSearchEventTimeDelayChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			var stb = o as SearchComboBox;
			if (stb != null)
			{
				stb._searchEventDelayTimer.Interval = ((Duration)e.NewValue).TimeSpan;
				stb._searchEventDelayTimer.Stop();
			}
		}

		private void ButtonClick(object sender, RoutedEventArgs e)
		{
			SearchTextBox.Clear();
		}

		private void PopupOpened(object sender, EventArgs e)
		{
			var popup = (Popup)sender;
			popup.VerticalOffset = ActualHeight - 3;
			popup.HorizontalOffset = (LoupeButton.ActualWidth + 2) * -1;
			popup.Width = ActualWidth - 2;
		}

		private void ItemsComboBox_DropDownClosed(object sender, EventArgs e)
		{
			SearchTextBox.Focus();
			_searchEventDelayTimer.Stop();
			_searchEventDelayTimer.Start();
		}

        private void LoupeButton_Click(object sender, RoutedEventArgs e)
        {
            ItemsComboBox.IsDropDownOpen = true;
        }

		#endregion

		#region Public methods

		public void FocusTextBox()
		{
            if (!SearchTextBox.IsFocused)
            {
                SearchTextBox.Focus();
            }
		}

		#endregion

		#region Private methods

		private void ExecuteSearchCommand()
		{
			if (SearchCommand != null)
				SearchCommand.Execute(CommandParameter);
		}

		#endregion
	}
}
