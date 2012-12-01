using System.Printing;
using System.Windows;
using System.Windows.Controls;

namespace IDservice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = new IdViewModel();
            this.DataContext = viewModel;

            
        }

        private void Print_OnClick(object sender, RoutedEventArgs e)
        {
            var queue = LocalPrintServer.GetDefaultPrintQueue();
            queue.CurrentJobSettings.Description = "idservice";
            var writer = PrintQueue.CreateXpsDocumentWriter(queue);
            writer.Write(Area);
            //var pr = new PrintDialog();
            //if (pr.ShowDialog() == true)
            //{
            //    pr.PrintVisual(Area, "grid");
            //}
        }
    }
}
