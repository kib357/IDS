using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;

namespace IDservice.View
{
    public class ResizeThumb : Thumb
    {
        private const double MinSideSize = 37.7953;

        private RotateTransform _rotateTransform;
        private double _angle;
        private Adorner _adorner;
        private Point _transformOrigin;
        private FrameworkElement _designerItem;
        private Canvas _canvas;
        private double _aspectRatio = 1;
        private double _originalWidth;
        private double _originalHeight;

        public ResizeThumb()
        {
            DataContextChanged += OnDataContextChanged;
            DragStarted += ResizeThumb_DragStarted;
            DragDelta += ResizeThumb_DragDelta;
            DragCompleted += ResizeThumb_DragCompleted;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _designerItem = DataContext as FrameworkElement;
            if (_designerItem != null)
            {
                _originalWidth = _designerItem.ActualWidth;
                _originalHeight = _designerItem.ActualHeight;
            }
            _aspectRatio = _originalWidth/_originalHeight;
        }

        private void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            _designerItem = DataContext as FrameworkElement;

            if (_designerItem != null)
            {
                _canvas = VisualTreeHelper.GetParent(_designerItem) as Canvas;

                if (_canvas != null)
                {
                    _transformOrigin = _designerItem.RenderTransformOrigin;

                    _rotateTransform = _designerItem.RenderTransform as RotateTransform;
                    if (_rotateTransform != null)
                    {
                        _angle = _rotateTransform.Angle * Math.PI / 180.0;
                    }
                    else
                    {
                        _angle = 0.0d;
                    }
                }
            }
        }

        private bool _constrainProportions;
        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            _constrainProportions = _designerItem.Tag != null && 
                                    HorizontalAlignment != HorizontalAlignment.Stretch &&
                                    VerticalAlignment != VerticalAlignment.Stretch;

            double currentWidthOffset = _designerItem.Width - _originalWidth;
            double currentHeightOffset = _designerItem.Height - _originalHeight;
            double widthAdjustment = e.HorizontalChange * (HorizontalAlignment == HorizontalAlignment.Left ? -1 : 1);
            double heightAdjustment = e.VerticalChange * (VerticalAlignment == VerticalAlignment.Top ? -1 : 1);
            double changedWidth = _originalWidth + currentWidthOffset + widthAdjustment;
            double changedHeight = _originalHeight + currentHeightOffset + heightAdjustment;
            if (_constrainProportions)
            {
                double minAdjustment = Math.Abs(widthAdjustment) < Math.Abs(heightAdjustment) ? widthAdjustment : heightAdjustment;
                changedWidth = _originalWidth + currentWidthOffset + minAdjustment;
                changedHeight = (_originalWidth + currentWidthOffset + minAdjustment) / _aspectRatio;
            }

            double newX = double.MaxValue, newY = double.MaxValue;
            if (HorizontalAlignment == HorizontalAlignment.Left)
                newX = Canvas.GetLeft(_designerItem) + (_designerItem.Width - changedWidth);
            if (VerticalAlignment == VerticalAlignment.Top)
                newY = Canvas.GetTop(_designerItem) + (_designerItem.Height - changedHeight);

            CheckConstraints(ref newX, ref newY, ref changedWidth, ref changedHeight);
            //Debug.WriteLine(newX + ";" + newY + ";width: " + changedWidth + ";height: " + changedHeight);

            if (HorizontalAlignment != HorizontalAlignment.Stretch)
            {
                _designerItem.Width = changedWidth;
                if (newX != double.MaxValue)
                    Canvas.SetLeft(_designerItem, newX);
            }
            if (VerticalAlignment != VerticalAlignment.Stretch)
            {
                _designerItem.Height = changedHeight;
                if (newY != double.MaxValue)
                    Canvas.SetTop(_designerItem, newY);
            }
            e.Handled = true;            
        }        

        private void CheckConstraints(ref double x, ref double y, ref double width, ref double height)
        {
            if (x < 0)
            {
                width += x;               
                x = 0;
                if (_constrainProportions)
                {
                    y += height - width / _aspectRatio;
                    height = width/_aspectRatio;
                }
            }
            if (y < 0)
            {
                height += y;                
                y = 0;
                if (_constrainProportions)
                {
                    x += width - height*_aspectRatio;
                    width = height*_aspectRatio;
                }
            }
            if (width + Canvas.GetLeft(_designerItem) > _canvas.ActualWidth)
            {
                width = Math.Min(width, _canvas.ActualWidth - Canvas.GetLeft(_designerItem));
                if (_constrainProportions)
                {
                    y += height - width / _aspectRatio;
                    height = width / _aspectRatio;
                }
            }
            if (height + Canvas.GetTop(_designerItem) > _canvas.ActualHeight)
            {
                height = Math.Min(height, _canvas.ActualHeight - Canvas.GetTop(_designerItem));
                if (_constrainProportions)
                {
                    x += width - height * _aspectRatio;
                    width = height * _aspectRatio;
                }
            }
            if (_constrainProportions)
            {
                if (_aspectRatio >= 1)
                {
                    if (width < MinSideSize*_aspectRatio)
                    {
                        x = x + (width - MinSideSize*_aspectRatio);
                        width = MinSideSize*_aspectRatio;
                    }
                    if (height < MinSideSize)
                    {
                        y = y + (height - MinSideSize);
                        height = MinSideSize;
                    }
                }
                else
                {
                    if (width < MinSideSize)
                    {
                        x = x + (width - MinSideSize);
                        width = MinSideSize;
                    }
                    if (height < MinSideSize/_aspectRatio)
                    {
                        y = y + (height - MinSideSize/_aspectRatio);
                        height = MinSideSize/_aspectRatio;
                    }
                }
            }
            else
            {
                if (width < MinSideSize)
                {
                    x = x + (width - MinSideSize);
                    width = MinSideSize;
                }
                if (height < MinSideSize)
                {
                    y = y + (height - MinSideSize);
                    height = MinSideSize;
                }
            }
        }

        private void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            //_originalWidth = _designerItem.ActualWidth;
            //_originalHeight = _designerItem.ActualHeight;
            //_aspectRatio = _originalWidth / _originalHeight;
            if (_adorner != null)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_canvas);
                if (adornerLayer != null)
                {
                    adornerLayer.Remove(_adorner);
                }

                _adorner = null;
            }
        }
    }
}
