using System.Windows;
using System.Windows.Controls;

namespace IDservice.View
{
    public class ResizeControl : Control
    {
        static ResizeControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeControl), new FrameworkPropertyMetadata(typeof(ResizeControl)));
        }
    }
}
