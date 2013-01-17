using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace IDservice.View
{
    public class ResizeAdorner : Adorner
    {
        private readonly ResizeControl _control;
        private readonly VisualCollection _visualChildren;

        public ResizeAdorner(UIElement adornedElement) : base(adornedElement)
        {
            _visualChildren = new VisualCollection(this);
            _control = new ResizeControl {DataContext = adornedElement};
            _visualChildren.Add(_control);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            _control.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override int VisualChildrenCount { get { return _visualChildren.Count; } }
        protected override Visual GetVisualChild(int index) { return _visualChildren[index]; }
    }
}
