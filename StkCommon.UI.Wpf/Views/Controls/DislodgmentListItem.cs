using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;

namespace StkCommon.UI.Wpf.Views.Controls
{
	public class DislodgmentListItem : ContentControl
	{
		/// <summary>
		/// Время между кликами, для определения двойного клика
		/// </summary>
		private const int DoubleClickInterval = 250;

		#region Private fields

		private DislodgmentList _settedDislodgmentList;
	    private readonly DispatcherTimer _doubleClickTimer;
	    private int _clickCount;
	    private MouseButtonEventArgs _mouseButtonEventArgs;

		#endregion

		#region Constructor

		public DislodgmentListItem()
		{
			IsHidden = false;
			IsDraged = false;
			IsDropPreview = false;
            _doubleClickTimer = new DispatcherTimer
            {
				Interval = TimeSpan.FromMilliseconds(DoubleClickInterval), 
                IsEnabled = false
            };
            _doubleClickTimer.Tick += DoubleClickTimerTick;
		}

		static DislodgmentListItem()
		{
			SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(DislodgmentListItem));
			UnselectedEvent = Selector.UnselectedEvent.AddOwner(typeof(DislodgmentListItem));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DislodgmentListItem), new FrameworkPropertyMetadata(typeof(DislodgmentListItem)));
		}

		#endregion

		#region Properties

		public static readonly DependencyProperty IsSelectedProperty = Selector.IsSelectedProperty.AddOwner(typeof(DislodgmentListItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault | FrameworkPropertyMetadataOptions.Journal, OnIsSelectedChanged));
		public static readonly DependencyProperty IsHiddenProperty = DependencyProperty.Register("IsHidden", typeof(bool), typeof(DislodgmentListItem), new PropertyMetadata(false));
		public static readonly DependencyProperty IsDragedProperty = DependencyProperty.Register("IsDraged", typeof(bool), typeof(DislodgmentListItem), new PropertyMetadata(false));
		public static readonly DependencyProperty IsDropPreviewProperty = DependencyProperty.Register("IsDropPreview", typeof(bool), typeof(DislodgmentListItem), new PropertyMetadata(false));

		public bool IsSelected
		{
			get
			{
				return (bool)GetValue(IsSelectedProperty);
			}
			set
			{
				SetValue(IsSelectedProperty, value);
			}
		}
		
		public bool IsHidden
		{
			get
			{
				return (bool)GetValue(IsHiddenProperty);
			}
			set
			{
				SetValue(IsHiddenProperty, value);
			}
		}
		
		public bool IsDraged
		{
			get
			{
				return (bool)GetValue(IsDragedProperty);
			}
			set
			{
				SetValue(IsDragedProperty, value);
			}
		}
		
		public bool IsDropPreview
		{
			get
			{
				return (bool)GetValue(IsDropPreviewProperty);
			}
			set
			{
				SetValue(IsDropPreviewProperty, value);
			}
		}

		private DislodgmentList ParentDislodgmentList
		{
			get
			{
				return ParentSelector as DislodgmentList ?? _settedDislodgmentList;
			}
		}

		#endregion

		#region Events

		public static readonly RoutedEvent SelectedEvent;

		public static readonly RoutedEvent UnselectedEvent;

		public event RoutedEventHandler Selected
		{
			add { AddHandler(SelectedEvent, value); }
			remove { RemoveHandler(SelectedEvent, value); }
		}

		public event RoutedEventHandler Unselected
		{
			add { AddHandler(UnselectedEvent, value); }
			remove { RemoveHandler(UnselectedEvent, value); }
		}

		#endregion

		private Selector ParentSelector
		{
			get
			{
				return ItemsControl.ItemsControlFromItemContainer(this) as Selector;
			}
		}

		protected virtual void OnSelected(RoutedEventArgs e)
		{
			HandleIsSelectedChanged(true, e);
		}

		protected virtual void OnUnselected(RoutedEventArgs e)
		{
			HandleIsSelectedChanged(false, e);
		}

		private void HandleIsSelectedChanged(bool newValue, RoutedEventArgs e)
		{
			RaiseEvent(e);
		}

        private void DoubleClickTimerTick(object sender, EventArgs e)
	    {
	        _doubleClickTimer.Stop();
            _clickCount = 0;
            if (ParentDislodgmentList != null) ParentDislodgmentList.NotifyListItemClicked(this, _mouseButtonEventArgs);
	    }

		protected override void OnMouseMove(MouseEventArgs e)
		{
			var parentDislodgmentList = ParentDislodgmentList;
			if (parentDislodgmentList != null && Mouse.LeftButton == MouseButtonState.Pressed)
			{
				parentDislodgmentList.NotifyListItemMouseDragged(this, e);
			}
			base.OnMouseMove(e);
		}

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = true;
                if (_clickCount > 0)
                {
                    if (ParentDislodgmentList != null) ParentDislodgmentList.NotifyListItemDoubleClicked(this, _mouseButtonEventArgs);
                    _doubleClickTimer.Stop();
                    _clickCount = 0;
                }
                else if (ParentDislodgmentList.OpenItemCommand != null)
                {
                    _doubleClickTimer.Start();
                    _mouseButtonEventArgs = e;
                    _clickCount++;
                }
                else
                    ParentDislodgmentList.NotifyListItemClicked(this, e);
            }
            base.OnMouseUp(e);
        }

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
            if (!e.Handled)
			{
				if (ParentDislodgmentList != null)
					ParentDislodgmentList.NotifyListItemDragged(this, e);
			}
			base.OnMouseLeftButtonDown(e);
		}

	   
		private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var dislodgmentListItem = d as DislodgmentListItem;
			if (dislodgmentListItem == null)
				return;

			if ((bool)e.NewValue)
			{
				dislodgmentListItem.OnSelected(new RoutedEventArgs(Selector.SelectedEvent, dislodgmentListItem));
			}
			else
			{
				dislodgmentListItem.OnUnselected(new RoutedEventArgs(Selector.UnselectedEvent, dislodgmentListItem));
			}
		}

		internal void SetParentDislodgmentList(DislodgmentList parent)
		{
			_settedDislodgmentList = parent;
		}
	}
}
