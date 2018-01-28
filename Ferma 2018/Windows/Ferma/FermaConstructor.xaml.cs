using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for FermaConstructor.xaml
    /// </summary>
    public partial class FermaConstructor : Window
    {
        FermaFile active_file;
        Ferma_form ferma_form;
        short selected_kernel;

        bool kernel_selected = false;
        bool is_it_init = true;

        public FermaConstructor(FermaFile active_file, Ferma_form ferma_form)
        {
            this.active_file = active_file;
            this.ferma_form = ferma_form;
            InitializeComponent();
        }

        public void Repaint(FermaFile active_file)
        {
            this.active_file = active_file;
            LoadActiveFileParams();
            LoadActiveFileKernelsToGrid();
        }

        private void onBeforeClosing(object sender, CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void LoadActiveFileParams()
        {
            x_dimension_of_project_area.Text = active_file.x_dimension_of_project_area.ToString();
            y_dimension_of_project_area.Text = active_file.y_dimension_of_project_area.ToString();

            kernels_count.Text = active_file.kernels_count.ToString();
            nodes_count.Text = active_file.nodes_count.ToString();

            modul_uprugosti.Text = active_file.elastic_modulus.ToString();
            density.Text = active_file.material_density.ToString();
            stress.Text = active_file.stresses_modulus.ToString();

            switch (active_file.linear_dimension)
            {
                case "мм":
                    linear_dimension.SelectedIndex = 0;
                    break;

                case "см":
                    linear_dimension.SelectedIndex = 1;
                    break;

                case "М":
                    linear_dimension.SelectedIndex = 2;
                    break;
            }

            switch (active_file.dimension_of_forces)
            {
                case "Н":
                    dimension_of_forces.SelectedIndex = 0;
                    break;

                case "кГ":
                    dimension_of_forces.SelectedIndex = 1;
                    break;
            }
        }

        private void Init(object sender, EventArgs e)
        {
            button.Visibility = Visibility.Hidden;
            LoadActiveFileParams();
        }

        private void LoadActiveFileKernelsToGrid()
        {
            List<FermaKernelGridTable> result = new List<FermaKernelGridTable>(active_file.kernels_count);

            short id = 1;

            foreach (FermaKernel kernel in active_file.kernels)
            {
                result.Add(new FermaKernelGridTable(id, kernel.start_node, kernel.end_node, 0f));
                id++;
            }
            kernels.ItemsSource = result;
        }

        private void kernels_Loaded(object sender, RoutedEventArgs e)
        {
            LoadActiveFileKernelsToGrid();
        }

        private void kernels_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FermaKernelGridTable path = kernels.SelectedItem as FermaKernelGridTable;

            if (kernel_selected)
            {
                UnselectKernel(selected_kernel);
                SelectKernel(path.id);
            }
            else
                SelectKernel(path.id);
        }

        private void SelectKernel(short id)
        {
            ferma_form.SelectKernel(id);
            selected_kernel = id;
            kernel_selected = true;
        }

        private void UnselectKernel(short id)
        {
            ferma_form.UnselectKernel(id);
            kernel_selected = false;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            /*ferma_form.ChangeSchemeScale(
                float.Parse(x_dimension_of_project_area.Text), 
                float.Parse(y_dimension_of_project_area.Text)
            );*/
        }

        private void linear_dimension_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!is_it_init)
                active_file.linear_dimension = (linear_dimension.Items[linear_dimension.SelectedIndex] as ComboBoxItem).Content.ToString();
        }

        private void dimension_of_forces_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!is_it_init)
                active_file.dimension_of_forces = (dimension_of_forces.Items[dimension_of_forces.SelectedIndex] as ComboBoxItem).Content.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            is_it_init = false;
        }
    }

}
