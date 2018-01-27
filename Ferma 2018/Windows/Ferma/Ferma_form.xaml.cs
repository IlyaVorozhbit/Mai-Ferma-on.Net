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
using Microsoft.Win32;

using Ferma_2018.Logic.Ferma;
using System.Globalization;

namespace Ferma_2018.Windows.Ferma
{
    /// <summary>
    /// Interaction logic for Ferma_form.xaml
    /// </summary>
    public partial class Ferma_form : Window
    {
        public FermaFileLoader file_loader;
        public FermaConstructor constructor;
        public List<int> drawed_kernels;

        public bool constructor_shows = false;


        public float width;
        public float height;

        public Ferma_form()
        {
            InitializeComponent();
        }

        private FermaFile ActiveFile()
        {
            return file_loader.active_file;
        }

        private void buttonDrawNode_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Проект Ferma (*.frm)|*.frm";

            if (openFileDialog.ShowDialog() == true)
            {
                openFile(openFileDialog.FileName);

                if (constructor == null)
                    constructor = new FermaConstructor(ActiveFile(), this);
                else
                    RepaintConstructor();
            }     
            
        }

        public void openFile(string filename)
        {
            file_loader = new FermaFileLoader();
            file_loader.Load(filename);

            filename_label.Content = filename;

            float width = file_loader.active_file.x_dimension_of_project_area;
            float height = file_loader.active_file.y_dimension_of_project_area;

            this.width = width;
            this.height = height;

            scheme.Width = width;
            scheme.Height = height;

            stress_case.Items.Clear();
            stress_case.SelectedIndex = -1;

            if (file_loader.active_file.stresses_count > 0)
            {
                short i = 1;
                foreach (FermaNodeStressCase ferma_stress_case in file_loader.active_file.stress_cases)
                {
                    stress_case.Items.Add("Случай нагружения " + i);
                    i++;
                }

                if(stress_case.Items.Count > 0)
                {
                    stress_case.SelectedIndex = 0;
                    stress_case.IsEnabled = true;
                }
            }

            RepaintScheme();
        }

        public void RepaintConstructor()
        {
            constructor.Repaint(ActiveFile());
        }

        public void RepaintScheme()
        {
            scheme.Children.Clear();
            borders.Children.Clear();
            PaintKernels();

            PaintNodes();
            PaintBorders();
            LoadInfoPanelValues();
        }

        public void SelectKernel(short id)
        {
            Line kernel = scheme.Children[drawed_kernels[id-1]] as Line;
            kernel.Stroke = Brushes.Yellow;
        }

        public void UnselectKernel(short id)
        {
            Line kernel = scheme.Children[drawed_kernels[id-1]] as Line;
            kernel.Stroke = Brushes.Black;
        }

        public void LoadInfoPanelValues()
        {
            ro_label.Content = ActiveFile().stresses_modulus.ToString("e3", CultureInfo.InvariantCulture);
        }

        // Рисуем узлы
        public void PaintNodes()
        {
            short i = 1;


            int index = stress_case.SelectedIndex;

            // если количество случаев нагрузки больше 0, 
            // то тогда после загрузки нового файла у нас получается SelectedIndex == -1
            // соответственно - выбираем 0 индекс или 1 случай нагрузки

            if (file_loader.active_file.stress_cases.Length > 0)
                if (stress_case.SelectedIndex == -1)
                    index = 0;

            foreach (FermaNode node in file_loader.active_file.stress_cases[index].nodes)
            {
                Ellipse point = new Ellipse();
                point.Height = 10;
                point.Width = 10;
                point.ToolTip =
                    i+ "\n" +
                    "x: " + node.x.ToString() + ", y: " + node.y.ToString()
                    + ", \nstress x: " + node.x_stress_force + ", stress y: " + node.y_stress_force + "\n";
                    
                SolidColorBrush SolidColorBrush = new SolidColorBrush();

                //if (node.x_fixed || node.y_fixed)
                //    SolidColorBrush.Color = Color.FromRgb(255, 0, 0);
                //else
                    SolidColorBrush.Color = Color.FromRgb(255, 255, 255);

                point.Fill = SolidColorBrush;
                point.StrokeThickness = 10;

                if (node.x_fixed || node.y_fixed || node.x_stress_force != 0 || node.y_stress_force != 0)
                    point.Stroke = Brushes.Red;
                else
                    point.Stroke = Brushes.Blue;

                Canvas.SetLeft(point, node.x);
                Canvas.SetBottom(point, node.y);

                scheme.Children.Add(point);
                i++;
            }
        }

        // Рисуем цифры для осей
        public void PaintBorders()
        {
            for (short i = 0; i <= height; i++)
            {
                if (i % 50 == 0)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = i.ToString() + ".00";
                    textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));

                    Canvas.SetLeft(textBlock, 5);
                    Canvas.SetBottom(textBlock, i + 54);

                    Line myLine = new Line();
                    myLine.Stroke = Brushes.Black;

                    myLine.X1 = 45;
                    myLine.X2 = 60;
                    myLine.Y1 = i + 12;
                    myLine.Y2 = i + 12;

                    myLine.StrokeThickness = 2;

                    borders.Children.Add(myLine);
                    borders.Children.Add(textBlock);
                }

                if (i % 75 == 0 && i != 0)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = i.ToString() + ".00";
                    textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));

                    Canvas.SetLeft(textBlock, i + 100);
                    Canvas.SetBottom(textBlock, 0);

                    borders.Children.Add(textBlock);
                }
            }
        }

        public void PaintKernels()
        {
            Line myLine;
            short i = 0;

            drawed_kernels.Clear();

            foreach (FermaKernel kernel in file_loader.active_file.kernels)
            {
                myLine = new Line();
                myLine.Stroke = Brushes.Black;

                myLine.X1 = file_loader.active_file.nodes[kernel.start_node -1].x + 4;
                myLine.X2 = file_loader.active_file.nodes[kernel.end_node -1].x + 4;
                myLine.Y1 = height - file_loader.active_file.nodes[kernel.start_node -1].y - 3;
                myLine.Y2 = height - file_loader.active_file.nodes[kernel.end_node -1].y - 3;

                myLine.ToolTip =
                    "id: " + (i+1) + "\n" +
                    "X1: " + myLine.X1 + ", X2: " + myLine.X2 + "\n" +
                    "Y1: " + myLine.Y1 + ", Y2: " + myLine.Y2 + "\n" +
                    "start_node: " + (kernel.start_node)  + "\n" +
                    "end_node: " + (kernel.end_node);

                myLine.StrokeThickness = 2;

                scheme.Children.Add(myLine);

                drawed_kernels.Add(scheme.Children.Count - 1);
                i++;
            }
        }

        public void ChangeSchemeScale(float width, float height)
        {
            scheme.Width = width;
            scheme.Height = height;
        }

        private void stressCaseChange(object sender, SelectionChangedEventArgs e)
        {
            RepaintScheme();
        }

        private void buttonConstructorClick(object sender, RoutedEventArgs e)
        {
            constructor_shows = !constructor_shows;

            if (constructor == null)
            {
                constructor = new FermaConstructor(ActiveFile(), this);

                constructor.Left = Width+Left - 15;
                constructor.Top = Top;
            }

            if (constructor_shows)
                constructor.Show();
            else
                constructor.Hide();
        }

        private void onBeforeClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(constructor != null)
                constructor.Close();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            drawed_kernels = new List<int>();
        }
    }

 
}
