using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageSlider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Load(object sender, RoutedEventArgs e)
        {
            double value = 0.4;
            this.Left = 0;
            this.Top = 0;
            //this.Height = Height * value;
            //this.Width = Width * value;

            //MainCanvas.Height = MainCanvas.Height * value;
            //MainCanvas.Width = MainCanvas.Width * value;

            //gdMainWindow.Height = gdMainWindow.Height * value;
            //gdMainWindow.Width = gdMainWindow.Width * value;
        }
    }
}
