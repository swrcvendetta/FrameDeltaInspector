using FrameDeltaInspector.Windows;
using System.Configuration;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;

namespace FrameDeltaInspector
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }

}
