using System.Windows;
using System.Windows.Controls;

namespace StkCommon.UI.Wpf.Views.Controls
{
    /// <summary>
	/// Interaction logic for LabeledComboBox.xaml
	/// </summary>
	public partial class LabeledComboBox
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(LabeledComboBox), new PropertyMetadata());

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(object), typeof(LabeledComboBox), new PropertyMetadata());

        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath", typeof(object), typeof(LabeledComboBox), new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(LabeledComboBox), new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LabeledComboBox), new FrameworkPropertyMetadata(OnTextChanged) { BindsTwoWayByDefault = true });

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(LabeledComboBox), new PropertyMetadata(default(int)));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(LabeledComboBox), new PropertyMetadata());

        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register("ItemContainerStyle", typeof(Style), typeof(LabeledComboBox), new PropertyMetadata());

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabeledComboBox), new FrameworkPropertyMetadata { BindsTwoWayByDefault = true });

        public static readonly DependencyProperty HasTextProperty =
            DependencyProperty.Register("HasText", typeof(bool), typeof(LabeledComboBox), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register("IsEditable", typeof(bool), typeof(LabeledComboBox), new PropertyMetadata(default(bool)));

        /// <summary>
        /// Label отображается, когда выбранный элемент равен этому значению
        /// </summary>
        public static readonly DependencyProperty ShowLabelWhenSelectedItemIsProperty =
            DependencyProperty.Register("ShowLabelWhenSelectedItemIs", typeof(object), typeof(LabeledComboBox), new PropertyMetadata(null));


        public LabeledComboBox()
        {
            InitializeComponent();
        }

        public object ItemsSource
        {
            get { return GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object DisplayMemberPath
        {
            get { return GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public object SelectedValuePath
        {
            get { return GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            private set { SetValue(HasTextProperty, value); }
        }

        public bool IsEditable
        {
            get { return (bool)GetValue(IsEditableProperty); }
            set { SetValue(IsEditableProperty, value); }
        }

        /// <summary>
        /// Label отображается, когда выбранный элемент равен этому значению
        /// </summary>
        public object ShowLabelWhenSelectedItemIs
        {
            get { return GetValue(ShowLabelWhenSelectedItemIsProperty); }
            set { SetValue(ShowLabelWhenSelectedItemIsProperty, value); }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HasText = SelectedItem == null
                ? SelectedItem != ShowLabelWhenSelectedItemIs
                : !SelectedItem.Equals(ShowLabelWhenSelectedItemIs);
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var labeledComboBox = (LabeledComboBox)d;
            labeledComboBox.HasText = !string.IsNullOrEmpty(labeledComboBox.Text);
        }
    }
}
