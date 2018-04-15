using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace OneMap.Controls
{
    public class MindMapLinkAdorner : Adorner
    {
        private const double ElbowRadius = 8.0;

        private readonly bool _isLeft;

        private readonly Pen _linkPen = new Pen(new SolidColorBrush(Colors.DarkRed), 2.0);

        private readonly TextBlock _mainLabel;

        private readonly DispatcherTimer _redrawTimer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(50)};

        public MindMapLinkAdorner(TreeView adornedElement, TextBlock mainLabel, bool isLeft) : base(adornedElement)
        {
            _mainLabel = mainLabel;
            _isLeft = isLeft;

            adornedElement.AddHandler(TreeViewItem.CollapsedEvent,
                new RoutedEventHandler(ExpandedChanged));

            adornedElement.AddHandler(TreeViewItem.ExpandedEvent,
                new RoutedEventHandler(ExpandedChanged));

            // If an item is added to the MindMap, and it doesn't have a size determined yet, 
            // try again after a short delay. Ugly hack, but it works. 
            _redrawTimer.Tick += (sender, args) =>
            {
                InvalidateVisual();
                _redrawTimer.Stop();
            };
        }

        private void ExpandedChanged(object sender, RoutedEventArgs args)
        {
            InvalidateVisual();
        }

        private void TreeViewItemIsLoadedChanged(object sender, RoutedEventArgs args)
        {
            var tvi = sender as TreeViewItem;
            tvi.Loaded -= TreeViewItemIsLoadedChanged;
            ((TreeView) AdornedElement).InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var tv = (TreeView) AdornedElement;

            foreach (var tvi in GetVisibleItems(tv)) DrawLink(drawingContext, tvi);
        }

        private void DrawLink(DrawingContext drawingContext, TreeViewItem tvi)
        {
            if (tvi.ActualHeight == 0.0)
            {
                _redrawTimer.Start();

                return;
            }

            var horizontalSegmentWidth = _isLeft ? -10.0 : 10.0;

            var parentElement = GetParent(tvi);

            var offsetItem = _isLeft ? 20.0 : 0.0;
            var offsetParent = !_isLeft ? 0.0 : 20.0;

            var point1 = new Point(_isLeft ? tvi.ActualWidth - offsetItem : 20.0,
                tvi.ActualHeight / 2);

            var startPoint = tvi.TranslatePoint(point1,
                AdornedElement);

            Point endPoint;

            if (_mainLabel == parentElement)
            {
                var point2 = new Point(_isLeft ? 0.0 : parentElement.RenderSize.Width - offsetParent,
                    parentElement.RenderSize.Height / 2);

                endPoint = parentElement.TranslatePoint(point2, AdornedElement);
            }
            else
            {
                var parentTvi = (TreeViewItem) parentElement;
                var hdr = (ContentPresenter) parentTvi.Template.FindName("PART_Header", parentTvi);

                var point2 = new Point(_isLeft ? 0.0 : hdr.RenderSize.Width - offsetParent, hdr.RenderSize.Height / 2);
                endPoint = hdr.TranslatePoint(point2, AdornedElement);
            }

            drawingContext.DrawLine(_linkPen, startPoint, endPoint);
        }

        private UIElement GetParent(TreeViewItem tvi)
        {
            var ancestor = VisualTreeHelper.GetParent(tvi);

            while (ancestor != null)
            {
                if (ancestor is TreeViewItem result) return result;

                if (ancestor is TreeView) return _mainLabel;

                ancestor = VisualTreeHelper.GetParent(ancestor);
            }

            return _mainLabel;
        }

        private IEnumerable<TreeViewItem> GetVisibleItems(FrameworkElement parent)
        {
            if (parent == null) yield break;

            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(parent); i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;

                if (child is TreeViewItem tvi)
                {
                    yield return tvi;

                    if (tvi.IsExpanded)
                        foreach (var childTvi in GetVisibleItems(tvi))
                            yield return childTvi;
                }
                else
                {
                    foreach (var childTvi in GetVisibleItems(child)) yield return childTvi;
                }
            }
        }

        protected void OnRender0(DrawingContext drawingContext)
        {
            var radius = 2.0;


            if (AdornedElement is TreeViewItem tvi)
            {
                var startX = tvi.ActualWidth - 8;
                var startY = tvi.ActualHeight / 2;

                var parentElement = (UIElement) tvi.Parent;

                if (parentElement is TreeView) parentElement = _mainLabel;

                var parentY = parentElement.TranslatePoint(new Point(0.0, parentElement.RenderSize.Height / 2), tvi);

                var startPoint = new Point(startX, startY);

                var itemHorizontalEnd = new Point(startX + 8, startY);

                var verticalStart = new Point(itemHorizontalEnd.X + radius, itemHorizontalEnd.Y + radius);

                var verticalEnd = new Point(verticalStart.X, parentY.Y);

                var parentHorizontalStart = new Point(verticalEnd.X + radius, verticalEnd.Y + radius);

                var parentHorizontalEnd = new Point(parentHorizontalStart.X, parentHorizontalStart.Y);

                var stroke = new Pen(new SolidColorBrush(Colors.Red), 1.5);

                drawingContext.DrawLine(stroke, startPoint, itemHorizontalEnd);

                drawingContext.DrawLine(stroke, verticalStart, verticalEnd);

                drawingContext.DrawLine(stroke, parentHorizontalStart, parentY);
            }
        }
    }
}