using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DBSelect
{
    public partial class DBselect : Form
    {
        public DBselect()
        {
            InitializeComponent();
            mc.SetServer("localhost")
            .SetDataBase("mysql")
            .SetUserID("root")
            .SetPassword("Xkn99777")
            .SetDataBase("projectevedb")
            .SetPort("3306")
            .SetCharset("utf8");
        }

        public class MysqlConnector
        {
            string server = null;
            string userid = null;
            string password = null;
            string database = null;
            string port = "3306";
            string charset = "utf-8";

            public MysqlConnector() { }
            public MysqlConnector SetServer(string server)
            {
                this.server = server;
                return this;
            }
            public MysqlConnector SetUserID(string userid)
            {
                this.userid = userid;
                return this;
            }
            public MysqlConnector SetDataBase(string database)
            {
                this.database = database;
                return this;
            }
            public MysqlConnector SetPassword(string password)
            {
                this.password = password;
                return this;
            }
            public MysqlConnector SetPort(string port)
            {
                this.port = port;
                return this;
            }
            public MysqlConnector SetCharset(string charset)
            {
                this.charset = charset;
                return this;
            }
            
            #region  建立MySql数据库连接
            /// <summary>
            /// 建立数据库连接.
            /// </summary>
            /// <returns>返回MySqlConnection对象</returns>
            private MySqlConnection GetMysqlConnection()
            {
                string M_str_sqlcon = string.Format("server={0};user id={1};password={2};database={3};port={4};Charset={5}", server, userid, password, database, port, charset);
                MySqlConnection myCon = new MySqlConnection(M_str_sqlcon);
                return myCon;
            }
            #endregion

            #region  执行MySqlCommand命令
            /// <summary>
            /// 执行MySqlCommand
            /// </summary>
            /// <param name="M_str_sqlstr">SQL语句</param>
            public void ExeUpdate(string M_str_sqlstr)
            {
                MySqlConnection mysqlcon = this.GetMysqlConnection();
                mysqlcon.Open();
                MySqlCommand mysqlcom = new MySqlCommand(M_str_sqlstr, mysqlcon);
                mysqlcom.ExecuteNonQuery();
                mysqlcom.Dispose();
                mysqlcon.Close();
                mysqlcon.Dispose();
            }
            #endregion

            #region  创建MySqlDataReader对象
            /// <summary>
            /// 创建一个MySqlDataReader对象
            /// </summary>
            /// <param name="M_str_sqlstr">SQL语句</param>
            /// <returns>返回MySqlDataReader对象</returns>
            public MySqlDataReader ExeQuery(string M_str_sqlstr)
            {
                Console.WriteLine(M_str_sqlstr);
                MySqlConnection mysqlcon = this.GetMysqlConnection();
                MySqlCommand mysqlcom = new MySqlCommand(M_str_sqlstr, mysqlcon);
                mysqlcon.Open();
                MySqlDataReader mysqlread = mysqlcom.ExecuteReader(CommandBehavior.CloseConnection);
                return mysqlread;
            }
            #endregion
        }
        MysqlConnector mc = new MysqlConnector();
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string sql = string.Empty;
                if (Username.Text == string.Empty&&Password.Text == string.Empty )
                {
                    MessageBox.Show("User Name or Passworld Should't be Empty", "Error", MessageBoxButtons.OK);
                }
                else
                {
                    sql = "select PassWord,Level from UserLogin where UserName=\"" + Username.Text+"\"";
                    //string sql = textBox1.Text;
                    resout.Text += sql + "\r\n";
                    string result = "";
                    string lV = "";
                    //执行查询
                    MySqlDataReader reader = mc.ExeQuery(sql);
                    if (reader.FieldCount != 2)
                    {
                        MessageBox.Show("User Name or Passworld is Not Correct", "Error", MessageBoxButtons.OK);
                    }
                    while (reader.Read())
                    {
                        //for (int i = 0; i < reader.FieldCount; i++)
                        //{
                        //    result += reader.GetName(i) + "\t" + reader.GetValue(i) + "\r\n";
                        //}
                        result = reader.GetValue(0).ToString();
                        lV = reader.GetValue(1).ToString();
                    }
                    if (result == Password.Text)
                    {
                        resout.Text += "User Name or Passworld is Correct" + "\r\n";
                        resout.Text += "用户等级：" + lV + "\r\n";
                    }
                    else
                    {
                        MessageBox.Show("User Name or Passworld is Not Correct", "Error", MessageBoxButtons.OK);
                    }
                }
                //resout.Text = result;
                //执行增删改等操作
                //mc.ExeUpdate(sql);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
            string sql = string.Empty;
            if (Username.Text == string.Empty && Password.Text == string.Empty)
            {
                MessageBox.Show("User Name or Passworld Should't be Empty", "Error", MessageBoxButtons.OK);
            }
            else
            {
                sql = "INSERT INTO UserLogin (UserName,PassWord,Level) VALUES ('"+ Username.Text + "','"+ Password.Text +"',0)";
                resout.Text += sql + "\r\n";
                mc.ExeUpdate(sql);
            }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
            }
        }
    }
}
