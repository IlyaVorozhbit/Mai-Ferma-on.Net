using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace Ferma_2018.Logic.DB.Models.Misc
{
    [SQLite.Table("latest_files")]
    public class LatestFile
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string filepath { get; set; }

        public override string ToString()
        {
            return string.Format(
                "{0}: {1}",
                Id,
                filepath
            );
        }

        public List<LatestFile> getLatestFiles(SQLiteConnection db)
        {          
            var q = db.Query<LatestFile>
            (
                "select * from latest_files order by id desc"
            );
            return q.ToList();
        }
    }
}
