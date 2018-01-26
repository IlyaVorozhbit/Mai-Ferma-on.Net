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

        public FermaConstructor(FermaFile active_file)
        {
            this.active_file = active_file;
            InitializeComponent();
        }

        private void onBeforeClosing(object sender, CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void Init(object sender, EventArgs e)
        {
            x_dimension_of_project_area.Text = active_file.x_dimension_of_project_area.ToString();
            y_dimension_of_project_area.Text = active_file.y_dimension_of_project_area.ToString();

            modul_uprugosti.Text = active_file.elastic_modulus.ToString();
            density.Text = active_file.material_density.ToString();
            stress.Text = active_file.stresses_modulus.ToString();
        }
    }

}
