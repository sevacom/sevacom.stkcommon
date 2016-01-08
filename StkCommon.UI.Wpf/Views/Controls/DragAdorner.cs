using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace StkCommon.UI.Wpf.Views.Controls
{
	public class DragAdorner : Adorner
	{
		private readonly DislodgmentListItem _dislodgmentListItem;
		private readonly AdornerLayer _adornerLayer;
		private double _leftOffset;
		private double _topOffset;

		public DragAdorner(DislodgmentListItem data, DataTemplate dataTemplate, UIElement adornedElement, AdornerLayer adornerLayer)
			: base(adornedElement)
		{
			if (data == null) throw new ArgumentNullException("data");
			if (adornerLayer == null) throw new ArgumentNullException("adornerLayer");

			_adornerLayer = adornerLayer;
			_dislodgmentListItem = new DislodgmentListItem { Content = data.Content, Opacity = 0.5, IsSelected = data.IsSelected};
			if (dataTemplate != null) 
				_dislodgmentListItem.ContentTemplate = dataTemplate;
			
			_adornerLayer.Add(this);
		}

		protected override Size MeasureOverride(Size constraint)
		{
			_dislodgmentListItem.Measure(constraint);
			return _dislodgmentListItem.DesiredSize;
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			_dislodgmentListItem.Arrange(new Rect(finalSize));
			return finalSize;
		}

		protected override Visual GetVisualChild(int index)
		{
			return _dislodgmentListItem;
		}

		protected override int VisualChildrenCount
		{
			get { return 1; }
		}

		public void Hide()
		{
			if (_dislodgmentListItem == null) 
				return;
			UpdatePosition(-10000, -10000); // Визибилити и опасити элемента не сработали: он не скрывается.
		}

		public void UpdatePosition(double left, double top)
		{
			_leftOffset = left;
			_topOffset = top;
			if (_adornerLayer != null)
			{
				_adornerLayer.Update(AdornedElement);
			}
		}

		public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
		{
			var desiredTransform = base.GetDesiredTransform(transform);
			var result = new GeneralTransformGroup();
			if (desiredTransform == null)
				return base.GetDesiredTransform(transform);

			result.Children.Add(desiredTransform);
			result.Children.Add(new TranslateTransform(_leftOffset, _topOffset));
			return result;
		}

		public void Destroy()
		{
			_adornerLayer.Remove(this);
		}
	}
}
