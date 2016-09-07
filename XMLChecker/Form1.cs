using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Data.SqlClient;
using System.Data.Odbc;

namespace XMLChecker
{
    public partial class Form1 : Form
    {
        public Form1()
            
        {
            InitializeComponent();
            
        }

        private void myMethod(int method_selector)
        {
            
            // For SQL server connection to work :
            // 1.download https://www.microsoft.com/en-us/download/details.aspx?id=30438
            // 2.start an instance of an SQL server by typing SQLEXPRESS to its name as locally accesible ( 127.0.0.1 )
            SqlConnection conn = new SqlConnection("Server=.\\SQLEXPRESS;Database=;Trusted_Connection=True");
 
            String str = "CREATE DATABASE fruits";
            bool exists;
            SqlCommand myCommand = new SqlCommand(str, conn);
            try
            {
                conn.Open();
                myCommand.ExecuteNonQuery();
                exists = false;
                //MessageBox.Show("DataBase is Created Successfully", "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (System.Exception ex)
            {
                exists = true;
                //MessageBox.Show(ex.ToString(), "MyProgram", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            if (!exists)
            {
                
                XMLRecreate(method_selector);

            } // if(!exists)
            else
            {
                // Update yapilacak
                SQLClear();
                XMLRecreate(method_selector);
            }

            SQLRead();

        }

        private void SQLRead()
        {
            //XmlDocument xdocument = new XmlDocument();
            //xdocument.Load("http://www.serkanbarak.com/XMLFile1.xml");

            //dataGridView1.ColumnCount = 4;
            //dataGridView1.RowCount = 11;

            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            SqlConnection conn = new SqlConnection("Server=.\\SQLEXPRESS;Database=fruits;Trusted_Connection=True");

            String str = "SELECT COUNT(*) FROM catalog;";

            SqlCommand scmd = new SqlCommand(str, conn);
            conn.Open();
            SqlDataReader sdata = scmd.ExecuteReader();
            sdata.Read();
            int row_count=(int)sdata.GetValue(0);
            sdata.Close();


            str = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS;";
            scmd = new SqlCommand(str, conn);
            sdata = scmd.ExecuteReader();
            sdata.Read();
            int column_count = (int)sdata.GetValue(0);
            sdata.Close();
            ;

            str = "SELECT * FROM catalog;";
            scmd = new SqlCommand(str, conn);
            sdata = scmd.ExecuteReader();
            dataGridView1.ColumnCount = column_count;
            dataGridView1.RowCount = row_count + 1;
            if (column_count != 0) {

                // column_header'larını dizmek :

                for (int j = 0; j < row_count; j++)
                {
                    sdata.Read();
                    int i = 0;
                    for(; i< column_count ; i++)
                    {
                        dataGridView1.Columns[i].HeaderText = sdata.GetName(i);
                        dataGridView1[i, j].Value = sdata.GetValue(i);
                    }

                }

                
            }

;
        }

        private DateTime time_check;

        private void XMLRecreate(int method_selector)
        {
            XmlDocument xdocument = new XmlDocument();
            if (method_selector == 1)
                xdocument.Load("http://www.serkanbarak.com/XMLFile1.xml");
            else
                xdocument.Load("http://www.serkanbarak.com/XMLFile2.xml");

            SqlConnection conn = new SqlConnection("Server=.\\SQLEXPRESS;Database=fruits;Trusted_Connection=True");

            string str = "CREATE TABLE catalog( name TEXT, code INTEGER, price INTEGER, interest REAL );";

            SqlCommand myCommand = new SqlCommand(str, conn);

            conn.Open();
            myCommand.ExecuteNonQuery();

            
            
            foreach (XmlNode node in xdocument.DocumentElement.ChildNodes)
            {
                if (node.Name == "updatetime")
                    if (node.InnerText == "Now") time_check = DateTime.Now;
                    else time_check = DateTime.Parse(node.InnerText);
                

            } // foreach

            if (DateTime.Now >= time_check)
            {

                textBox1.Text = ""; if(t!=null)if(t.Enabled) t.Stop();
                foreach (XmlNode node in xdocument.DocumentElement.ChildNodes)
                {
                    if (node.Name != "xml" && node.Name != "updatetime")
                    {
                        string node2str = "";
                        foreach (XmlNode node2 in node.ChildNodes)
                        {
                            if (node2.Attributes["type"]?.InnerText == "string")
                                node2str += "'" + node2.InnerText + "',";
                            else
                                node2str += node2.InnerText + ",";
                        }
                        node2str = node2str.TrimEnd(',');
                        str = "INSERT INTO catalog (name, code, price, interest) VALUES (" + node2str + ");";
                        myCommand = new SqlCommand(str, conn);
                        myCommand.ExecuteNonQuery();
                    }
                }
            }
            else
            {
                // TIMER BASLATILACAK
                StartTimer();
            }

            conn.Close();
        }

        private void SQLClear()
        {
            // CAPRAZ KONTROL DAHA UZUN SURER. SQL DATABASE'I SIFIRLAYIP TEKRARDAN TABLO OLUSTURULACAK.
            SqlConnection conn = new SqlConnection("Server=.\\SQLEXPRESS;Database=fruits;Trusted_Connection=True");

            string str = "DROP TABLE catalog;";

            SqlCommand myCommand = new SqlCommand(str, conn);

            conn.Open();
            myCommand.ExecuteNonQuery();
            conn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myMethod(1);
        }

        
        System.Windows.Forms.Timer t = null;
        
        private void StartTimer()
        {
            t = new System.Windows.Forms.Timer();
            t.Interval = 1000;
           
            t.Tick += new EventHandler(t_Tick);
            t.Enabled = true;
            double time_span = (time_check - DateTime.Now).TotalSeconds;
            textBox1.Text = ((int)(time_span)).ToString();
            ;
        }

        void t_Tick(object sender, EventArgs e)
        {

            if (textBox1.Text == "0" || textBox1.Text=="")
            {
                myMethod(2);
                t.Stop();
            }
            else
                textBox1.Text = (int.Parse(textBox1?.Text) - 1).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            myMethod(2);
        }
    }


}

