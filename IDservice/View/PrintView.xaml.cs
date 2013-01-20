using System.Printing;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Forms;
using IDservice.ViewModel;
using Microsoft.Win32;
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
                //vm.(dlg.SelectedPath);
            }
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as IdViewModel;
            var queue = vm.SelectedPrinter;
            queue.CurrentJobSettings.Description = "idservice";
            var writer = PrintQueue.CreateXpsDocumentWriter(queue);
            //writer.Write(Area);
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
        }
    }
}
