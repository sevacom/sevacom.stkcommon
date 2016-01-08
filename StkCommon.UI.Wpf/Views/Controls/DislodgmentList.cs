using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;

namespace StkCommon.UI.Wpf.Views.Controls
{
	[StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(DislodgmentListItem))]
	public class DislodgmentList : Selector
	{
		#region Private fields
		
		private bool _isRearrangement;
		private Point _rearrangementStartingPoint;
		private DislodgmentListItem _draggedItem;
		private DragAdorner _dragAdorner;
		private readonly ContextMenu _contextMenu;
		private ToggleButton _toggleButton;
		private ItemsPresenter _itemsPresenter;
		private DislodgmentListItem _hiddenSelection;

		#endregion

		#region Constructor

		public DislodgmentList()
		{
			_contextMenu = new ContextMenu();
			AllowDrop = true;
			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			CalculateDislodgment();
		}

		static DislodgmentList()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(DislodgmentList), new FrameworkPropertyMetadata(typeof(DislodgmentList)));
		}

		#endregion

		#region Public Properties

		public static readonly DependencyProperty IsOverloadedProperty = DependencyProperty.Register("IsOverloaded", typeof(bool), typeof(DislodgmentList));
		public static readonly DependencyProperty IsSelectedHiddenProperty = DependencyProperty.Register("IsSelectedHidden", typeof(bool), typeof(DislodgmentList));
		public static readonly DependencyProperty SelectCommandProperty = DependencyProperty.Register("SelectCommand", typeof(ICommand), typeof(DislodgmentList));
		public static readonly DependencyProperty DeselectCommandProperty = DependencyProperty.Register("DeselectCommand", typeof(ICommand), typeof(DislodgmentList));
		public static readonly DependencyProperty RearrangeCommandProperty = DependencyProperty.Register("RearrangeCommand", typeof(ICommand), typeof(DislodgmentList));
        public static readonly DependencyProperty OpenItemCommandProperty = DependencyProperty.Register("OpenItemCommand", typeof(ICommand), typeof(DislodgmentList));

		public bool IsSelectedHidden
		{
			get { return (bool)GetValue(IsSelectedHiddenProperty); }
			set { SetValue(IsSelectedHiddenProperty, value); }
		}
		public bool IsOverloaded
		{
			get { return (bool)GetValue(IsOverloadedProperty); }
			set { SetValue(IsOverloadedProperty, value); }
		}

		#endregion

		#region Commands

		public ICommand SelectCommand
		{
			get { return (ICommand)GetValue(SelectCommandProperty); }
			set { SetValue(SelectCommandProperty, value); }
		}
		
		public ICommand DeselectCommand
		{
			get { return (ICommand)GetValue(DeselectCommandProperty); }
			set { SetValue(DeselectCommandProperty, value); }
		}
		
		public ICommand RearrangeCommand
		{
			get { return (ICommand)GetValue(RearrangeCommandProperty); }
			set { SetValue(RearrangeCommandProperty, value); }
		}
       
		public ICommand OpenItemCommand
        {
            get { return (ICommand)GetValue(OpenItemCommandProperty); }
            set { SetValue(OpenItemCommandProperty, value); }
        }

		#endregion

		#region Selector Override

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (_itemsPresenter != null) _itemsPresenter.SizeChanged -= ItemsPresenterSizeChanged;
			_itemsPresenter = GetTemplateChild("PART_ItemsList") as ItemsPresenter;
			if (_itemsPresenter != null) _itemsPresenter.SizeChanged += ItemsPresenterSizeChanged;

			if (_hiddenSelection != null) _hiddenSelection.MouseLeftButtonUp -= HiddenSelectionClick;
			_hiddenSelection = GetTemplateChild("PART_HiddenSelection") as DislodgmentListItem;
			if (_hiddenSelection != null)
			{
				_hiddenSelection.MouseLeftButtonUp += HiddenSelectionClick;
				_hiddenSelection.SetParentDislodgmentList(this);
			    if (ItemTemplate != null) _hiddenSelection.ContentTemplate = ItemTemplate;
			}

			if (_toggleButton != null) _toggleButton.Click -= ShowMenu;
			_toggleButton = GetTemplateChild("PART_ShowMenu") as ToggleButton;
			if (_toggleButton != null)
			{
				_toggleButton.ContextMenu = _contextMenu;
				_toggleButton.Click += ShowMenu;
			}
		}

		protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			base.OnItemsChanged(e);
			if (_contextMenu == null) return;
			foreach (MenuItem menuItem in _contextMenu.Items)
			{
				menuItem.Click -= MiClick;
			}
			_contextMenu.Items.Clear();
		    if (ItemTemplate != null) _contextMenu.ItemTemplate = ItemTemplate;
			foreach (var item in Items)
			{
                var menuItem = new MenuItem { Header = item, Tag = item };
                if (ItemTemplate != null) menuItem.HeaderTemplate = ItemTemplate;
                menuItem.Click += MiClick;
                _contextMenu.Items.Add(menuItem);
			}

			// BeginInvoke нужен, чтобы CalculateDislodgment отработал, когда все другие обработчики
			// отработают, элемент отобразится в окне и у него проставится ActualWidth
			if (Dispatcher != null)
				Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
				{
					try
					{
						CalculateDislodgment();
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex.ToString());
					}
				}));
		}

		protected override bool IsItemItsOwnContainerOverride(object item)
		{
			return (item is DislodgmentListItem);
		}
		
		protected override DependencyObject GetContainerForItemOverride()
		{
			return new DislodgmentListItem();
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			if (_dragAdorner != null)
			{
				var currentPosition = e.GetPosition(this);
				_dragAdorner.UpdatePosition(currentPosition.X, currentPosition.Y);
			}

			if (_isRearrangement)
			{
				DislodgmentListItem targetDislodgmentListItem = null;
				var nextItemIsDropPreviewed = false;
				foreach (var item in Items)
				{
					var dislogmentListItem = GetDislodgmentListItemFromItem(item);
					var point = e.GetPosition(dislogmentListItem);
					dislogmentListItem.IsDropPreview = false;
					if (nextItemIsDropPreviewed && !dislogmentListItem.IsDraged)
					{
						dislogmentListItem.IsDropPreview = true;
						nextItemIsDropPreviewed = false;
					}
					if (targetDislodgmentListItem != null || !IsInsideElement(dislogmentListItem, point)) continue;
					if (IsInsideRightPartOfElement(dislogmentListItem, point)) nextItemIsDropPreviewed = true;
					else targetDislodgmentListItem = dislogmentListItem;
				}
				if (targetDislodgmentListItem != null) 
					targetDislodgmentListItem.IsDropPreview = true;
			}
			base.OnDragOver(e);
		}
		
		protected override void OnDragLeave(DragEventArgs e)
		{
			if (!IsInsideElement(this, e.GetPosition(this)))
			{
				foreach (var item in Items)
				{
					var dislogmentListItem = GetDislodgmentListItemFromItem(item);
					if (dislogmentListItem.IsDropPreview) dislogmentListItem.IsDropPreview = false;
				}
				if (_dragAdorner != null) 
					_dragAdorner.Hide();
			}
			base.OnDragLeave(e);
		}

		protected override void OnDrop(DragEventArgs e)
		{
			var sourceDislodgmentListItem = _draggedItem ?? e.Data.GetData(typeof(DislodgmentListItem)) as DislodgmentListItem;
		    if (sourceDislodgmentListItem != null)
		    {
		        var sourceItem = ItemContainerGenerator.ItemFromContainer(sourceDislodgmentListItem);
		        if (sourceItem != null)
		        {
		            var targetIndex = -1;
		            foreach (var item in Items)
		            {
		                var dislogmentListItem = GetDislodgmentListItemFromItem(item);
		                var point = e.GetPosition(dislogmentListItem);
		                if (!IsInsideElement(dislogmentListItem, point)) continue;
		                if (IsInsideRightPartOfElement(dislogmentListItem, point)) targetIndex = Items.IndexOf(item) + 1;
		                else targetIndex = Items.IndexOf(item);
		                break;
		            }
		            if (targetIndex == -1 && IsInsideElement(this, e.GetPosition(this))) targetIndex = Items.Count;
		            if (targetIndex > Items.IndexOf(sourceItem)) targetIndex--;
		            var rearrangeParameter = new RearrangeCommandParameter
		            {
		                Item = sourceItem,
		                Index = targetIndex
		            };
		            if (RearrangeCommand != null && RearrangeCommand.CanExecute(rearrangeParameter))
		                RearrangeCommand.Execute(rearrangeParameter);
		        }
		    }
		    AfterDragDrop();
		}

		#endregion

		#region Private Methods

		private void ShowMenu(object sender, RoutedEventArgs e)
		{
			_toggleButton.ContextMenu.IsOpen = true;
		}

		private void ItemsPresenterSizeChanged(object sender, SizeChangedEventArgs sizeInfo)
		{
			if (sizeInfo.WidthChanged)
			{
				CalculateDislodgment();
			}
		}

		private void HiddenSelectionClick(object sender, MouseButtonEventArgs e)
		{
			SelectItem(_hiddenSelection.Tag);
		}

		
		private void MiClick(object sender, RoutedEventArgs e)
		{
			var menuItem = sender as MenuItem;
			if (menuItem == null) return;
			SelectItem(menuItem.Tag);
		}

		internal void NotifyListItemClicked(DislodgmentListItem item, MouseButtonEventArgs e)
		{
            if (e.ChangedButton == MouseButton.Left)
				SelectItem(item.Name == "PART_HiddenSelection"
					? ItemContainerGenerator.ContainerFromItem(item.Tag)
					: item);
		}

	    internal void NotifyListItemDoubleClicked(DislodgmentListItem item, MouseButtonEventArgs e)
	    {
            if (OpenItemCommand != null) OpenItemCommand.Execute(ItemContainerGenerator.ItemFromContainer(item));
	    }

        internal void NotifyListItemMouseDragged(DislodgmentListItem item, MouseEventArgs e)
		{
			if (RearrangeCommand == null) return;
			if (_isRearrangement || !IsReachedMinimumDragDistance(_rearrangementStartingPoint, e.GetPosition(this))) return;
			BeforeDragDrop();
		}
		
		internal void NotifyListItemDragged(DislodgmentListItem item, MouseButtonEventArgs e)
		{
			if (e.ChangedButton != MouseButton.Left) return;
			_rearrangementStartingPoint = e.GetPosition(this);
			if (item.Name == "PART_HiddenSelection")
				_draggedItem = ItemContainerGenerator.ContainerFromItem(_hiddenSelection.Tag) as DislodgmentListItem;
			else
				_draggedItem = item;
		}

		private void BeforeDragDrop()
		{
			if (_draggedItem == null)
				return;
			_draggedItem.IsDraged = true;
			_isRearrangement = true;
			IsSelectedHidden = false;

			if (_dragAdorner == null)
			{
				var adornerLayer = AdornerLayer.GetAdornerLayer(this);
				_dragAdorner = new DragAdorner(_draggedItem, ItemTemplate, this, adornerLayer);
				_dragAdorner.UpdatePosition(_rearrangementStartingPoint.X, _rearrangementStartingPoint.Y);
			}

			DragDrop.AddQueryContinueDragHandler(this, QueryContinueDragHandler);
			DragDrop.DoDragDrop(this, _draggedItem, DragDropEffects.Move);
		}
		
		private void AfterDragDrop()
		{

			if (_dragAdorner != null)
			{
				_dragAdorner.Destroy();
				_dragAdorner = null;
			}

			DragDrop.RemoveQueryContinueDragHandler(this, QueryContinueDragHandler);
			_isRearrangement = false;
			foreach (var item in Items)
			{
				var dislogmentListItem = GetDislodgmentListItemFromItem(item);
				dislogmentListItem.IsDropPreview = false;
				dislogmentListItem.IsDraged = false;
			}
			_draggedItem = null;
			CalculateDislodgment();
		}

		private void QueryContinueDragHandler(object o, QueryContinueDragEventArgs e)
		{
			if (!e.KeyStates.HasFlag(DragDropKeyStates.LeftMouseButton))
				AfterDragDrop();
		}

		private DislodgmentListItem GetDislodgmentListItemFromItemOrNull(object item)
		{
			var dislodgmentListItem = item as DislodgmentListItem ??
					   ItemContainerGenerator.ContainerFromItem(item) as DislodgmentListItem;
			return dislodgmentListItem;
		}
	
		private DislodgmentListItem GetDislodgmentListItemFromItem(object item)
		{
			var dislodgmentListItem = GetDislodgmentListItemFromItemOrNull(item);
			if (dislodgmentListItem == null) 
				throw new Exception("Cannot get a container for Item in DislogementList");
			return dislodgmentListItem;
		}

		private void SelectItem(object item)
		{
			var dislogmentListItem = GetDislodgmentListItemFromItemOrNull(item);
			if (dislogmentListItem == null)
				return;

			if (dislogmentListItem.IsSelected)
			{
				dislogmentListItem.IsSelected = false;
				if (DeselectCommand != null) DeselectCommand.Execute(null);
			}
			else
			{
				if (SelectedItem != null) 
					GetDislodgmentListItemFromItem(SelectedItem).IsSelected = false;
				dislogmentListItem.IsSelected = true;
				if (SelectCommand != null) 
					SelectCommand.Execute(ItemContainerGenerator.ItemFromContainer(dislogmentListItem));
			}
			CalculateDislodgment();
		}

		private void CalculateDislodgment()
		{
			if (_itemsPresenter == null) return;
			var isNowOverloaded = false;
			var isNowSelectedHidden = false;
			var actualWidth = _itemsPresenter.ActualWidth;
			var currentWidth = 0d;
			var hiddenItemsCount = 0;
			const double gap = 18d;

			foreach (var item in Items)
			{
				var dislodgmentListItem = GetDislodgmentListItemFromItemOrNull(item);
				if(dislodgmentListItem == null)
					continue;
				if (dislodgmentListItem.IsSelected && currentWidth < actualWidth && currentWidth + dislodgmentListItem.ActualWidth >= actualWidth &&
					!IsSelectedHidden)
				{
					isNowOverloaded = true;
				}
				if (isNowOverloaded)
				{
					dislodgmentListItem.IsHidden = true;
					if (dislodgmentListItem.IsSelected)
					{
						isNowSelectedHidden = true;
						_hiddenSelection.Content = dislodgmentListItem.Content;
						_hiddenSelection.IsSelected = dislodgmentListItem.IsSelected;
						_hiddenSelection.Tag = item;
					}
					else
					{
						hiddenItemsCount++;
					}
				}
				else
				{
					dislodgmentListItem.IsHidden = false;
					currentWidth += dislodgmentListItem.ActualWidth;
					if (currentWidth + gap > actualWidth) isNowOverloaded = true;
				}
				foreach (var menuItem in _contextMenu.Items)
				{
				    var menuItemContainer = menuItem as MenuItem ?? _contextMenu.ItemContainerGenerator.ContainerFromItem(menuItem) as MenuItem;
				    if (menuItemContainer == null || menuItemContainer.Tag != item) continue;
                    menuItemContainer.Visibility = (dislodgmentListItem.IsHidden && !dislodgmentListItem.IsSelected ? Visibility.Visible : Visibility.Collapsed);
    			}
			}
			if (hiddenItemsCount == 0) isNowOverloaded = false; // По факту ведь ничего не спрятано, пипку не показываем
			IsOverloaded = isNowOverloaded;
			IsSelectedHidden = (!_isRearrangement && isNowSelectedHidden); // Всегда скрываем залипший пункт, если сейчас драгдроп
		}

		private static bool IsInsideElement(FrameworkElement element, Point point)
		{
			return point.X >= 0 && point.X <= element.ActualWidth &&
				   point.Y >= 0 && point.Y <= element.ActualHeight;
		}

		private static bool IsInsideRightPartOfElement(FrameworkElement element, Point point)
		{
			return point.X >= element.ActualWidth / 2 && point.X <= element.ActualWidth &&
				   point.Y >= 0 && point.Y <= element.ActualHeight;
		}

		private static bool IsReachedMinimumDragDistance(Point start, Point end)
		{
			return (Math.Abs(start.X - end.X) > SystemParameters.MinimumHorizontalDragDistance ||
					Math.Abs(start.Y - end.Y) > SystemParameters.MinimumVerticalDragDistance);
		}

		#endregion
		
		// Заменить контекстное меню с невлезшими фильтрами на дропдаун. Научить драгенддропить на и из дропдауна.
    
    }

	/// <summary>
	/// Параметр для команды RearrangeCommand
	/// </summary>
	public struct RearrangeCommandParameter
	{
		public object Item { get; set; }

		public int Index { get; set; }
	}
}
