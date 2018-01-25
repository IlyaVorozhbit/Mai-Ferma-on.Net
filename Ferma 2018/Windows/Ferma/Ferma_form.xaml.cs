using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

using Ferma_2018.Logic.Ferma;

namespace Ferma_2018.Windows.Ferma
{
    /// <summary>
    /// Interaction logic for Ferma_form.xaml
    /// </summary>
    public partial class Ferma_form : Window
    {
        public FermaFileLoader file_loader;

        public Ferma_form()
        {
            InitializeComponent();
        }

        private void buttonDrawNode_Click(object sender, RoutedEventArgs e)
        {

            file_loader = new FermaFileLoader();
            file_loader.Load("noname2.frm");

            float width = file_loader.active_file.x_dimension_of_project_area;
            float height = file_loader.active_file.y_dimension_of_project_area;

            scheme.Width = width;
            scheme.Height = height;

            foreach (FermaNode node in file_loader.active_file.nodes)
            {
                Ellipse point = new Ellipse();
                point.Height = 5;
                point.Width = 5;
                point.ToolTip = "x: " + node.x.ToString()+ ", y: " + node.y.ToString();

                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 255, 255, 0);
                point.Fill = mySolidColorBrush;
                point.StrokeThickness = 2;
                point.Stroke = Brushes.Black;

                Canvas.SetLeft(point, node.x);
                Canvas.SetBottom(point, node.y);

                scheme.Children.Add(point);
            }

            for (short i = 0; i <= height; i ++)
            {
                if(i % 50 == 0)
                {
                    TextBlock textBlock = new TextBlock();             
                    textBlock.Text = i.ToString() + ".00";
                    textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));

                    Canvas.SetLeft(textBlock, 5);
                    Canvas.SetBottom(textBlock, i+10);

                    borders.Children.Add(textBlock);
                }
            }
        }


    }
}
