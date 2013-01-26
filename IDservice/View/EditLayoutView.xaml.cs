using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using IDservice.ViewModel;
using Microsoft.Win32;

namespace IDservice.View
{
    /// <summary>
    /// Interaction logic for EditLayoutView.xaml
    /// </summary>
    public partial class EditLayoutView : UserControl
    {
        private ResizeAdorner _selectedItemAdorner;
        private bool _isDragging;
        private Point _lastDragPoint;

        public EditLayoutView()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
                {
                    Filter = "Images|*.jpg;*.jpeg;*.png;",
                    Title = "Выберите файл для фона макета"
                };
            if (dlg.ShowDialog() == true && DataContext is IdViewModel)
            {
                var vm = DataContext as IdViewModel;
                vm.LoadNewLayoutBackground(dlg.FileName);
            }
        }

        private void LoadOtherSideButton_CLick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Images|*.jpg;*.jpeg;*.png;",
                Title = "Выберите файл для фона макета"
            };
            if (dlg.ShowDialog() == true && DataContext is IdViewModel)
            {
                var vm = DataContext as IdViewModel;
                vm.LoadNewLayoutOtherside(dlg.FileName);
            }
        }

        private void EditLayoutView_OnLoaded(object sender, RoutedEventArgs e)
        {
            myCanvas.PreviewMouseLeftButtonDown += CanvasOnPreviewMouseLeftButtonDown;
            myCanvas.MouseMove += CanvasMouseMove;
            myCanvas.MouseUp += CanvasMouseUp;
            if (Application.Current != null)
            {
                Application.Current.MainWindow.KeyDown += CanvasKeyDown;
                Application.Current.MainWindow.KeyUp += CanvasKeyUp;
            }
        }


        private void CanvasOnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RemoveAdorner();

            var element = e.Source as FrameworkElement;
            if (element == null || element is Canvas || element is Image) return;

            _selectedItemAdorner = new ResizeAdorner(element);
            if (!(element is TextBlock))
                AdornerLayer.GetAdornerLayer(element).Add(_selectedItemAdorner);
            _isDragging = true;
            myCanvas.CaptureMouse();
        }

        private void RemoveAdorner()
        {
            if (_selectedItemAdorner != null)
            {
                var selectedItemLayer = AdornerLayer.GetAdornerLayer(_selectedItemAdorner.AdornedElement);
                selectedItemLayer.Remove(_selectedItemAdorner);
                _selectedItemAdorner = null;
            }
        }

        private void CanvasKeyUp(object sender, KeyEventArgs e)
        {
            if (_selectedItemAdorner != null)
            {
                if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    var element = _selectedItemAdorner.AdornedElement as FrameworkElement;
                    if (element != null) element.Tag = null;
                }
            }
        }

        private void CanvasKeyDown(object sender, KeyEventArgs e)
        {
            if (_selectedItemAdorner != null)
            {
                var element = _selectedItemAdorner.AdornedElement as FrameworkElement;
                if (element == null) return;

                switch (e.Key)
                {
                    case Key.LeftShift:
                    case Key.RightShift:
                        element.Tag = "c";
                        break;
                    case Key.Left:
                        if (Canvas.GetLeft(element) > 0)
                            Canvas.SetLeft(element, Canvas.GetLeft(element) - 1);
                        break;
                    case Key.Right:
                        if (Canvas.GetLeft(element) + element.ActualWidth < myCanvas.ActualWidth)
                            Canvas.SetLeft(element, Canvas.GetLeft(element) + 1);
                        break;
                    case Key.Up:
                        if (Canvas.GetTop(element) > 0)
                            Canvas.SetTop(element, Canvas.GetTop(element) - 1);
                        break;
                    case Key.Down:
                        if (Canvas.GetTop(element) + element.ActualHeight < myCanvas.ActualHeight)
                            Canvas.SetTop(element, Canvas.GetTop(element) + 1);
                        break;
                }
            }
        }

        private void CanvasMouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging && _selectedItemAdorner != null)
            {
                var mousePosition = e.GetPosition(myCanvas);
                var item = _selectedItemAdorner.AdornedElement;
                var newPositionX = mousePosition.X - _lastDragPoint.X + Canvas.GetLeft(item);
                var newPositionY = mousePosition.Y - _lastDragPoint.Y + Canvas.GetTop(item);
                if (newPositionX >= 0 && newPositionX <= myCanvas.ActualWidth - item.DesiredSize.Width)
                    Canvas.SetLeft(item, newPositionX);
                if (newPositionY >= 0 && newPositionY <= myCanvas.ActualHeight - item.DesiredSize.Height)
                    Canvas.SetTop(item, newPositionY);
            }

            _lastDragPoint = e.GetPosition(myCanvas);
        }

        private void CanvasMouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            myCanvas.ReleaseMouseCapture();
        }

        private void EditLayoutView_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            RemoveAdorner();
        }
    }
}
