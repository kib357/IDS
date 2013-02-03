using System.Printing;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Forms;
using IDservice.ViewModel;
using PrintDialog = System.Windows.Controls.PrintDialog;
using UserControl = System.Windows.Controls.UserControl;

namespace IDservice.View
{
    /// <summary>
    /// Interaction logic for PrintView.xaml
    /// </summary>
    public partial class PrintView : UserControl
    {
        private PrintDialog pr = new PrintDialog();

        public PrintView()
        {
            InitializeComponent();
        }

        private void SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();
            if (dlg.ShowDialog() == DialogResult.OK && DataContext is IdViewModel)
            {
                var vm = DataContext as IdViewModel;
                vm.CardUserPhotoPath = dlg.SelectedPath;
            }
        }

        private void Print_OnClick(object sender, RoutedEventArgs e)
        {
            var res = pr.ShowDialog();
            var vm = DataContext as IdViewModel;
            if (vm.SelectedLayout.PrintBackground == false)
                BackgroundImage.Visibility = Visibility.Hidden;

            var ticket = pr.PrintTicket;
            var x = vm.SelectedLayout.PrintMarginX;
            var y = vm.SelectedLayout.PrintMarginY;
            if (ticket.PageMediaSize.Width.HasValue && ticket.PageMediaSize.Height.HasValue)
            {
                vm.SelectedLayout.PrintMarginX = x + pr.PrintableAreaWidth - ticket.PageMediaSize.Width.Value;
                vm.SelectedLayout.PrintMarginY = y + pr.PrintableAreaHeight - ticket.PageMediaSize.Height.Value;
            }
            Area.Width = pr.PrintableAreaWidth;
            Area.Height = pr.PrintableAreaHeight;
            Area.Measure(new Size(Area.Width, Area.Height));
            Area.Arrange(new Rect(0, 0, Area.Width, Area.Height));

            if (res == true)
                pr.PrintVisual(Area, "ID card");

            vm.SelectedLayout.PrintMarginX = x;
            vm.SelectedLayout.PrintMarginY = y;
            BackgroundImage.Visibility = Visibility.Visible;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as IdViewModel;
            if (vm.SelectedLayout.PrintBackground == false)
                BackgroundImage.Visibility = Visibility.Hidden;
            var queue = vm.SelectedPrinter;
            queue.CurrentJobSettings.Description = "idservice";
            var writer = PrintQueue.CreateXpsDocumentWriter(queue);
            //writer.Write(Area);
            var left = queue.UserPrintTicket.PageMediaSize.Width / 2 - Area.ActualWidth / 2;
            //Area.Margin = new Thickness((double)left, 0, 0, 0);
            var collator = writer.CreateVisualsCollator();

            collator.BeginBatchWrite();
            collator.Write(Area);
            collator.Write(Area);
            collator.EndBatchWrite();
            //writer.WriteAsync(Area);            
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var vm = DataContext as IdViewModel;
            if (vm == null) return;
            Area.Width = pr.PrintableAreaWidth;
            Area.Height = pr.PrintableAreaHeight;
        }
    }
}
