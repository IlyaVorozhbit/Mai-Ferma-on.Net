using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ferma_2018.Logic.Ferma
{

    public class FermaKernel
    {
        public short start_node;
        public short end_node;

        public float cross_sectional_area; // площадь сечения стержня
    }

    public class FermaNode
    {
        public float x;
        public float y;

        public bool x_fixed;
        public bool y_fixed;

        public float x_stress_force; // сила нагрузки по оси X
        public float y_stress_force; // сила нагрузки по оси Y
    }

    public class FermaFile
    {
        public int kernels_count;       // стержни
        public int nodes_count;         // узлы
        public int nodes_fixed_count;   // закреплённые узлы

        public float elastic_modulus;
        public int stresses_count;

        public float stresses_modulus;
        public double material_density;

        public string linear_dimension; // линейная размерность
        public string dimension_of_forces; // размерность сил

        public float x_dimension_of_project_area; // размерность области по x
        public float y_dimension_of_project_area; // размерность области по y

        public FermaKernel[] kernels;
        public FermaNode[] nodes;
    }

    public class FermaFileLoader
    {
        public FermaFile active_file;
        public int       current_line;

        public FermaFileLoader()
        {

        }

        public void Load(string filename)
        {
    
            //TODO: хз как читать кодировку, правильно, файла, поэтому размерности 
            //      считываются через одно место...

            if (File.Exists(filename))
            {
                string[] lines = System.IO.File.ReadAllLines(filename);

                active_file = new FermaFile();

                active_file.kernels_count = int.Parse(lines[0]);
                active_file.nodes_count   = int.Parse(lines[1]);
                active_file.nodes_fixed_count   = int.Parse(lines[2]);

                active_file.elastic_modulus = ParseFloat(lines[3]);
                active_file.stresses_count = int.Parse(lines[4]);

                active_file.stresses_modulus = ParseFloat(lines[5]);
                active_file.material_density = ParseFloat(lines[6]);

                current_line = 6; // фактически - линия 7, в массиве - 6

                if(active_file.kernels_count > 0)
                {
                    current_line++;

                    active_file.kernels = new FermaKernel[active_file.kernels_count];

                    for (short i = 0; i < active_file.kernels_count; i++)
                    {
                        FermaKernel kernel = new FermaKernel();

                        kernel.start_node = short.Parse(lines[current_line]);
                        current_line++;

                        kernel.end_node = short.Parse(lines[current_line]);
                        current_line++;

                        active_file.kernels[i] = kernel;
                    }

                }

                if(active_file.nodes_count > 0)
                {

                    active_file.nodes = new FermaNode[active_file.nodes_count];

                    for (short i = 0; i < active_file.nodes_count; i++)
                    {
                        FermaNode node = new FermaNode();

                        node.x =ParseFloat(lines[current_line]);
                        current_line++;

                        node.y = ParseFloat(lines[current_line]);
                        current_line++;

                        active_file.nodes[i] = node;
                    }
                }

                if(active_file.kernels_count > 0)
                {
                    for (short i = 0; i < active_file.kernels_count; i++)
                    {                    
                        active_file.kernels[i].cross_sectional_area = ParseFloat(lines[current_line]);
                        current_line++;                    
                    }
                }

                if (active_file.nodes_count > 0)
                {
               
                    for (short i = 0; i < active_file.nodes_count; i++)
                    {
                        short is_x_fixed = short.Parse(lines[current_line]);                       
                        active_file.nodes[i].x_fixed = is_x_fixed==1? false : true;
                        current_line++;

                        short is_y_fixed = short.Parse(lines[current_line]);
                        active_file.nodes[i].y_fixed = is_y_fixed == 1 ? false : true;
                        current_line++;
                    }

                    if(active_file.stresses_count > 0)
                    {
                        for(short j = 0; j < active_file.stresses_count; j ++)
                        {
                            for (short i = 0; i < active_file.nodes_count; i++)
                            {
                                active_file.nodes[i].x_stress_force = ParseFloat(lines[current_line]);
                                current_line++;

                                active_file.nodes[i].y_stress_force = ParseFloat(lines[current_line]);
                                current_line++;
                            }
                       }
  
                    }


                }

                active_file.linear_dimension = lines[current_line];
                current_line++;

                active_file.dimension_of_forces = lines[current_line];
                current_line++;

                active_file.x_dimension_of_project_area = ParseFloat(lines[current_line]);
                current_line++;

                active_file.y_dimension_of_project_area = ParseFloat(lines[current_line]);

                //MessageBox.Show(Encoding.GetEncoding(filename).ToString());
                //MessageBox.Show("Файл не найден", "Ошибка");

            }
            else
                MessageBox.Show("Файл не найден", "Ошибка");
        }

        public float ParseFloat(string value)
        {
            float the_value; // собственно, значение в итоге

            bool until_dot_part_parsed = false; // мы находимся до точки?
            string until_dot_part = ""; // часть числа до точки

            bool is_degree_positive = false; // 

            bool is_before_degree_sign = true; // мы находимся до знака степени?
            string before_degree_sign = ""; // часть числа до знака степени

            double before_degree_sign_double; // часть числа до знака степени, 
                                            // потребуется для корректировки 

            bool is_after_degree_sign = false; //
            string after_degree_sign = ""; //

            int degree_multiplier = 1;

            short i = 0;

            foreach (char c in value)
            {
                if(!until_dot_part_parsed)
                {
                    if (c == '.')
                        until_dot_part_parsed = true;
                    else                   
                       until_dot_part += c;                                   
                }
                else
                {
                    if(!is_after_degree_sign)
                    {
                        if(!is_before_degree_sign)
                        {
                            if (c == '+')
                            {
                                is_after_degree_sign = true;
                                is_degree_positive = true;
                            }
                            if (c == '-')
                                is_after_degree_sign = true;
                        }
                        else
                        {
                            if (value[i + 1] == '+' || value[i + 1] == '-')
                                is_before_degree_sign = false;

                            if(c != 'E')
                                before_degree_sign += c;
                        }

                    }
                    else
                    {
                        after_degree_sign += c;
                    }                    
                }
                i++;
            }

            before_degree_sign_double =
                double.Parse(before_degree_sign) / (10
                *
                (double)Math.Pow(10, before_degree_sign.Length-1));

            degree_multiplier = is_degree_positive ? 1 : -1;

           the_value = (float)
            (
                (float.Parse(until_dot_part) + before_degree_sign_double)
                * 
                Math.Pow(10, degree_multiplier*double.Parse(after_degree_sign))
            );

            return the_value;

        }

    }
}
