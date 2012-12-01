using System;
using System.Diagnostics;
using System.Printing;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.ViewModel;

namespace IDservice
{
    public class IdViewModel : NotificationObject
    {
        public IdViewModel()
        {
            var src = new BitmapImage();
            src.BeginInit();
            src.UriSource = new Uri("background.jpg", UriKind.Relative);
            src.CacheOption = BitmapCacheOption.OnLoad;
            src.EndInit();
            Background = src;

            var printServer = new LocalPrintServer();

            PrintQueueCollection printQueuesOnLocalServer = printServer.GetPrintQueues(new[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections });

            foreach (PrintQueue printer in printQueuesOnLocalServer)
            {
                Debug.WriteLine("\tThe shared printer : " + printer.Name);                
            } 
        }

        private BitmapImage _background;
        public BitmapImage Background
        {
            get { return _background; }
            set
            {
                _background = value;
                RaisePropertyChanged("Background");
            }
        }
    }
}
