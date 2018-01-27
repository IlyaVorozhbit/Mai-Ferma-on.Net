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
using Ferma_2018.Logic.DB;
using Ferma_2018.Logic.DB.Models.Misc;
using System.Data.SQLite;
using System.Data.Entity;

namespace Ferma_2018
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        private SQLiteMain sqlite_main;
        private List<LatestFile> latest_files;
        private Ferma_2018.Windows.Ferma.Ferma_form ferma_form;
        private List<Windows.Ferma.Ferma_form> ferma_forms;

        private int files_start_index = 0;

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

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                OpenFermaWithFile(openFileDialog.FileName);
            }
            
        }

        public void WorkWithLatestFiles(string filename)
        {
            var file = new LatestFile();
            file.filepath = filename;

            if (latest_files.Capacity > 0)
            {

                var check = sqlite_main.Connection().Query<LatestFile>(
                    "select * from latest_files where filepath = ?", filename
                );

                if (check.Count > 0)
                {

                    for (short i = 0; i < latest_files.Count; i++)
                    {
                        if (latest_files[i].filepath == filename)
                        {

                            latest_files.RemoveAt(i);
                            latest_files.Insert(0, file);
                            MainMenuRedraw(filename);

                            var check_2 = sqlite_main.Connection().Query<LatestFile>(
                                "delete from latest_files where filepath = ?", filename
                            );

                            var check_3 = sqlite_main.Connection().Insert(file);
                        }
                    }
                }
                else
                {
                    latest_files.Insert(0, file);
                    MainMenuRedraw(filename);
                    var check_3 = sqlite_main.Connection().Insert(file);
                }

            }
            else
            {
                project_menu.Items.Add(new Separator());
                files_start_index = project_menu.Items.Count;

                latest_files.Insert(0, file);
                MainMenuRedraw(filename);
                var check_3 = sqlite_main.Connection().Insert(file);
            }
        }

        public void OpenFermaWithFile(string filename)
        {    

            WorkWithLatestFiles(filename);

            ferma_form = new Windows.Ferma.Ferma_form();
            ferma_forms.Add(ferma_form);

            ferma_form.openFile(filename);
            ferma_form.Show();

        }

        public void OpenFermaWithFile(object sender, RoutedEventArgs e)
        {
            var t = e.Source as MenuItem;

            OpenFermaWithFile(t.Header.ToString());
        }

        private void new_ferma_btn_Click(object sender, RoutedEventArgs e)
        {
            Ferma_2018.Windows.Ferma.Ferma_form ferma_form = new Windows.Ferma.Ferma_form();
            ferma_forms.Add(ferma_form);

            ferma_form.Show();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            ferma_forms = new List<Windows.Ferma.Ferma_form>();

            ferma_form = new Windows.Ferma.Ferma_form();
            ferma_forms.Add(ferma_form);

            sqlite_main = new SQLiteMain("Data/main.db");
            LatestFile model = new LatestFile();

            latest_files = new List<LatestFile>();
            latest_files = model.getLatestFiles(sqlite_main.Connection());
            files_start_index = project_menu.Items.Count;

            if (latest_files.Capacity > 0)
            {
                project_menu.Items.Add(new Separator());
                files_start_index = project_menu.Items.Count;

                foreach (var file in latest_files)
                {
                    MenuItem menu_item = new MenuItem();
                    menu_item.Header = file.filepath;

                    menu_item.Click += new RoutedEventHandler(OpenFermaWithFile);
                    project_menu.Items.Add(menu_item);
                }
            }

        }

        private void MainMenuRedraw(string name)
        {
            //MessageBox.Show(files_start_index.ToString());
            MainMenuDeleteLastFile(name);
            MainMenuAddLastFile(name);
        }

        private void MainMenuDeleteLastFile(string name)
        {
            for(int i = files_start_index; i<project_menu.Items.Count; i++)
            {
                if(project_menu.Items[i].GetType() == typeof(MenuItem))
                {
                    var item = project_menu.Items[i] as MenuItem;

                    if (item.Header.ToString() == name)
                    {
                        project_menu.Items.RemoveAt(i);
                    }
                }              

            }
        }

        private void MainMenuAddLastFile(string name)
        {
            MenuItem menu_item = new MenuItem();
            menu_item.Header = name;

            menu_item.Click += new RoutedEventHandler(OpenFermaWithFile);

            project_menu.Items.Insert(files_start_index, menu_item);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sqlite_main.Connection().Close();

            foreach(Ferma_2018.Windows.Ferma.Ferma_form form in ferma_forms)          
                form.Close();       
        }
    }
}
