using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Oce
{
    public partial class Form1 : Form
    {
        Converter converter = new Converter();
        List<Comments> comList = new List<Comments>();
        public Form1()
        {

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var ctx = new Model1())
            {
                var sqlCommand = File.ReadAllText(@"C:\Users\popel\Downloads\Telegram Desktop\DB_DUMP.sql");
                ctx.Database.ExecuteSqlCommandAsync(sqlCommand);
            }

            MessageBox.Show("New database is filled");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            List<Comments> values = converter.FromJson("D:/Academy/Oce/Oce/bin/Debug/jsoncomments.txt");
            comList = values;
            converter.ListToDb(values);
            
        }

        public class Converter
        {
            public List<Comments> FromJson(string adress)
            {
                var json = File.ReadAllText(adress);
                var newList = JsonConvert.DeserializeObject<List<Comments>>(json);
                MessageBox.Show("Json is converted!");
                return newList;
            }

            public void ListToDb(List<Comments> values)
            {
                using (var db = new Model1())
                {
                    foreach (var com in values)
                    {
                        db.Comments.Add(com);
                    }
                    db.SaveChanges();
                }
                MessageBox.Show("List is converted to database!");
            }
            
            public void RemoveCodeFromDb(List<Comments> values)
            {
                using (var db = new Model1())
                {
                    var commentsList = db.Comments.ToList();
                    foreach (var com in values)
                    {
                        if (commentsList.All(c => c.Id != com.Id)) continue;
                        {
                            var el = db.Comments.FirstOrDefault(c => c.Id == com.Id);
                            if (el != null) db.Comments.Remove(el);
                        }
                    }
                    db.SaveChanges();
                }
                MessageBox.Show("Database is cleared!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            converter.RemoveCodeFromDb(comList);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (var ctx = new Model1())
            {
                string command = "RESTORE DATABASE AdventureWorks2012 FROM DISK = 'C:/Program Files/Microsoft SQL Server/MSSQL14.SQLEXPRESS/MSSQL/Backup/TestDb.bak'; ";
                ctx.Database.ExecuteSqlCommandAsync(command);
            }
            MessageBox.Show("Database is restored from backup!");
        }
    }
}
