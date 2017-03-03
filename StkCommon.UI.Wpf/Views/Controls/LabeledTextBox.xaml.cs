using System;
using System.Windows;
using System.Windows.Controls;

namespace StkCommon.UI.Wpf.Views.Controls
{
    /// <summary>
    /// Interaction logic for LabeledTextBox.xaml
    /// </summary>
    public partial class LabeledTextBox
    {
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(LabeledTextBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty LabelTextProperty =
            DependencyProperty.Register("LabelText", typeof(string), typeof(LabeledTextBox),
                                        new PropertyMetadata(default(string)));

        public static readonly DependencyProperty HasTextProperty =
            DependencyProperty.Register("HasText", typeof(bool), typeof(LabeledTextBox), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty MaxLengthProperty =
            DependencyProperty.Register("MaxLength", typeof(int), typeof(LabeledTextBox), new PropertyMetadata(Int32.MaxValue));

        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(LabeledTextBox), new PropertyMetadata(default(bool)));

        public static readonly DependencyProperty TextBoxStyleProperty = DependencyProperty.Register(
            "TextBoxStyle", typeof(Style), typeof(LabeledTextBox), new PropertyMetadata(default(Style)));


        public LabeledTextBox()
        {
            InitializeComponent();
        }


        public string LabelText
        {
            get { return (string)GetValue(LabelTextProperty); }
            set { SetValue(LabelTextProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public bool HasText
        {
            get { return (bool)GetValue(HasTextProperty); }
            set { SetValue(HasTextProperty, value); }
        }

        public int MaxLength
        {
            get { return (int)GetValue(MaxLengthProperty); }
            set { SetValue(MaxLengthProperty, value); }
        }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public Style TextBoxStyle
        {
            get { return (Style)GetValue(TextBoxStyleProperty); }
            set { SetValue(TextBoxStyleProperty, value); }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HasText = !string.IsNullOrEmpty(TextBox1.Text);
        }
    }
}
