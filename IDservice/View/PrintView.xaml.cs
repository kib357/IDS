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
            var pr = new PrintDialog();
            if (pr.ShowDialog() == true)
            {
                pr.PrintVisual(Area, "grid");
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as IdViewModel;
            if (vm.PrintBackground == false)
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
            //var pr = new PrintDialog();
            //if (pr.ShowDialog() == true)
            //{
            //    pr.PrintVisual(Area, "grid");
            //}

            BackgroundImage.Visibility = Visibility.Visible;
        }
    }
}
