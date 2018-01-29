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

    public partial class Ferma_form : Window
    {
        public FermaFileLoader file_loader;
        public FermaConstructor constructor;
        public List<int> drawed_kernels;

        public float difference_after_changing_y = 0f;
        public float difference_after_changing_x = 0f;
        public bool is_height_larger_than_earlier = false;
        public bool is_width_larger_than_earlier = false;

        public float initial_y_scale = 0f;

        public bool constructor_shows = false;

        public float width;
        public float height;

        public Ferma_form()
        {
            InitializeComponent();
        }

        public FermaFile ActiveFile()
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

            initial_y_scale = height;

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
                    textBlock.Text = (height-i).ToString() + ".00";
                    textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));

                    textBlock.RenderTransform = new TranslateTransform
                    {
                        X = 53,
                        Y = i + 5
                    };

                    Line line_y = new Line();

                    line_y.Stroke = Brushes.Black;

                    line_y.X1 = 93;
                    line_y.X2 = 108;
                    line_y.Y1 = i + 12;
                    line_y.Y2 = i + 12;

                    line_y.StrokeThickness = 2;

                    borders.Children.Add(line_y);
                    borders.Children.Add(textBlock);
                }

            }

            for (short i = 0; i<= width; i++)
            {

                if (i % 75 == 0)
                {
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = i.ToString() + ".00";
                    textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));

                    textBlock.RenderTransform = new TranslateTransform
                    {
                        X = i + 100,
                        Y = height + 50
                    };
                    borders.Children.Add(textBlock);

                    Line line_x = new Line();

                    line_x.Stroke = Brushes.Black;

                    line_x.X1 = i + 99 + textBlock.Text.Length * 2.6;
                    line_x.X2 = i + 99 + textBlock.Text.Length * 2.6;
                    line_x.Y1 = height + 35;
                    line_x.Y2 = height + 50;

                    line_x.StrokeThickness = 2;

                    borders.Children.Add(line_x);
                }
            }
        }

        public void PaintKernels()
        {
            Line kernel_line;
            short i = 0;

            drawed_kernels.Clear();

            foreach (FermaKernel kernel in file_loader.active_file.kernels)
            {
                kernel_line = new Line();
                kernel_line.Stroke = Brushes.Black;

                kernel_line.X1 = file_loader.active_file.nodes[kernel.start_node -1].x + 4;
                kernel_line.X2 = file_loader.active_file.nodes[kernel.end_node -1].x + 4;

                kernel_line.Y1 = height - file_loader.active_file.nodes[kernel.start_node - 1].y - 3;
                kernel_line.Y2 = height - file_loader.active_file.nodes[kernel.end_node - 1].y - 3;


                kernel_line.ToolTip =
                    "id: " + (i+1) + "\n" +
                    "X1: " + kernel_line.X1 + ", X2: " + kernel_line.X2 + "\n" +
                    "Y1: " + kernel_line.Y1 + ", Y2: " + kernel_line.Y2 + "\n" +
                    "start_node: " + (kernel.start_node)  + "\n" +
                    "end_node: " + (kernel.end_node);

                kernel_line.StrokeThickness = 2;

                scheme.Children.Add(kernel_line);

                drawed_kernels.Add(scheme.Children.Count - 1);
                i++;
            }
        }

        public bool ChangeSchemeScale(float width, float height)
        {
            if (width <= 0 || height <= 0)
            {
                MessageBox.Show(
                     "Введены некорректные значения ширины или высоты области.\n\nЗначение ширины и высоты должны быть положительны.",
                     "Ошибка изменения области",
                     System.Windows.MessageBoxButton.OK,
                     System.Windows.MessageBoxImage.Error
                 );

                return false;
            }
            else
            {
                this.height = height;
                this.width = width;

                if (ActiveFile().y_dimension_of_project_area > height)
                {
                    is_height_larger_than_earlier = false;
                    difference_after_changing_y = ActiveFile().y_dimension_of_project_area - height;
                }
                else
                {
                    is_height_larger_than_earlier = true;
                    difference_after_changing_y = height - ActiveFile().y_dimension_of_project_area;
                }

                if (ActiveFile().x_dimension_of_project_area > width)
                {
                    is_width_larger_than_earlier = false;
                    difference_after_changing_x = ActiveFile().x_dimension_of_project_area - width;
                }
                else
                {
                    is_width_larger_than_earlier = true;
                    difference_after_changing_x = width - ActiveFile().x_dimension_of_project_area;
                }

                Application.Current.MainWindow = this;

                if (is_height_larger_than_earlier)
                {
                    Application.Current.MainWindow.Height += difference_after_changing_y;
                    ferma_stats_panel.Margin = new Thickness(
                        ferma_stats_panel.Margin.Left,
                        ferma_stats_panel.Margin.Top + difference_after_changing_y,
                        0,
                        0
                    );
                }
                else
                {
                    Application.Current.MainWindow.Height -= difference_after_changing_y;
                    ferma_stats_panel.Margin = new Thickness(
                        ferma_stats_panel.Margin.Left,
                        ferma_stats_panel.Margin.Top - difference_after_changing_y,
                        0,
                        0
                    );
                }

                if (is_width_larger_than_earlier)
                {
                    Application.Current.MainWindow.Width += difference_after_changing_x;
                }
                else
                {
                    Application.Current.MainWindow.Width -= difference_after_changing_x;
                }

                ActiveFile().x_dimension_of_project_area = width;
                ActiveFile().y_dimension_of_project_area = height;

                scheme.Width = width;
                scheme.Height = height;

                if (is_height_larger_than_earlier)
                    borders.Height += difference_after_changing_y;
                else
                    borders.Height -= difference_after_changing_y;

                if (is_width_larger_than_earlier)
                    borders.Width += difference_after_changing_y;
                else
                    borders.Width -= difference_after_changing_y;

                RepaintScheme();

                return true;
            }

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
