using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

//using System.Data.SQLite;
using Ferma_2018.Logic.DB.Models.Misc;
using SQLite;

namespace Ferma_2018.Logic.DB
{

    class SQLiteMain
    {
        public string filename;
        private SQLiteConnection connection;

        public SQLiteMain (string filename)
        {
            this.filename = filename;
            init();
        }

        public void init()
        {
            connection = new SQLiteConnection(filename, true);
        }

        public SQLiteConnection Connection()
        {
            return connection;
        }
    }
}
