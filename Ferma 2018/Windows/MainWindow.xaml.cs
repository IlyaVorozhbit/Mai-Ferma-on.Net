using Microsoft.Win32;
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

namespace Ferma_2018
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_exit_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btn_open_click(object sender, RoutedEventArgs e)
        {
            Ferma_2018.Windows.Ferma.Ferma_form ferma_form = new Windows.Ferma.Ferma_form();

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                ferma_form.openFile(openFileDialog.FileName);
                ferma_form.Show();
            }
            
        }

        private void new_ferma_btn_Click(object sender, RoutedEventArgs e)
        {
            Ferma_2018.Windows.Ferma.Ferma_form ferma_form = new Windows.Ferma.Ferma_form();
            ferma_form.Show();
        }
    }
}
