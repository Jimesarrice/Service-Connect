using HslCommunication;
using HslCommunication.BasicFramework;
using HslCommunication.Enthernet;
using HslCommunication.LogNet;
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

namespace Server
{
    public partial class Server : Form
    {
        public Server()
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
        private NetSimplifyServer net_simplify_server = new NetSimplifyServer(); //实例化



        private void Server_Load(object sender, EventArgs e)
        {
            Net_Simplify_Server_Initialization();
            //richTextBox1.Text = Guid.NewGuid().ToString();
        }
        // 同步传送数据的初始化
        private void Net_Simplify_Server_Initialization()
        {
            try
            {
                net_simplify_server.KeyToken = new Guid("3ee0dbce-3489-4634-9706-fedf7bb61741");//设置身份令牌，本质就是一个GUID码，验证客户端使用
                string LogSavePath = null;
                net_simplify_server.LogNet = new LogNetSingle(LogSavePath + @"\simplify_log.txt");//日志路径，单文件存储模式，采用组件信息
                net_simplify_server.LogNet.SetMessageDegree(HslMessageDegree.DEBUG);//默认debug及以上级别日志均进行存储，根据需要自行选择，DEBUG存储的信息比较多
                net_simplify_server.ReceiveStringEvent += Net_simplify_server_ReceiveStringEvent;//接收到字符串触发
                net_simplify_server.ReceivedBytesEvent += Net_simplify_server_ReceivedBytesEvent;//接收到字节触发
                net_simplify_server.ServerStart(15500);//网络端口，此处使用了一个随便填写的端口
            }
            catch (Exception ex)
            {
                SoftBasic.ShowExceptionMessage(ex);
            }
        }

        /// <summary>
        /// 接收来自客户端的字节数据
        /// </summary>
        /// <param name="state">网络状态</param>
        /// <param name="customer">字节数据，根据实际情况选择是否使用</param>
        /// <param name="data">来自客户端的字节数据</param>
        private void Net_simplify_server_ReceivedBytesEvent(AsyncStateOne state, NetHandle customer, byte[] data)
        {
            if (customer == 1000)
            {
                // 收到指令为1000的请求时，返回1000长度的字节数组
                net_simplify_server.SendMessage(state, customer, new byte[1000]);
                this.richTextBox1.Text += data.ToString();
            }
            else
            {
                net_simplify_server.SendMessage(state, customer, data);
            }
        }
        
        /***********************************************************************************************
         *
         *    方法说明：    当接收到来自客户端的数据的时候触发的方法
         *    特别注意：    如果你的数据处理中引发了异常，应用程序将会奔溃，SendMessage异常系统将会自动处理
         *
         ************************************************************************************************/
         
        /// <summary>
        /// 接收到来自客户端的字符串数据，然后将结果发送回客户端，注意：必须回发结果
        /// </summary>
        /// <param name="state">客户端的地址</param>
        /// <param name="handle">用于自定义的指令头，可不用，转而使用data来区分</param>
        /// <param name="data">接收到的服务器的数据</param>
        private void Net_simplify_server_ReceiveStringEvent(AsyncStateOne state, NetHandle handle, string data)
        {

            /*******************************************************************************************
             *
             *     说明：同步消息处理总站，应该根据不同的消息设置分流到不同的处理方法
             *    
             *     注意：处理完成后必须调用 net_simplify_server.SendMessage(state, customer, "处理结果字符串，可以为空");
             *
             *******************************************************************************************/

            if (handle.CodeMajor == 1)
            {
                ProcessCodeMajorOne(state, handle, data);
            }
            else if (handle.CodeMajor == 2)
            {
                ProcessCodeMajorTwo(state, handle, data);
            }
            else if (handle.CodeMajor == 3)
            {
                ProcessCodeMajorThree(state, handle, data);
            }
            else
            {
                net_simplify_server.SendMessage(state, handle, data);
            }
        }

        private void  ProcessCodeMajorOne(AsyncStateOne state, NetHandle handle, string data)
        {
            if (handle.CodeIdentifier == 1)
            {
                // 下面可以再if..else
                //net_simplify_server.SendMessage(state, handle, "测试数据大类1，命令1，接收到的数据是：" + data);
                //this.richTextBox1.Text += "发送的数据"+data.ToString();
                try
                {
                    string sql = string.Empty;
                    sql = "select PassWord from UserLogin where UserName=\"" + data + "\"";
                    //string sql = textBox1.Text;
                    richTextBox1.Text += sql + "\r\n";
                    string result = "";
                    string lV = "";
                    //执行查询
                    MySqlDataReader reader = mc.ExeQuery(sql);
                    while (reader.Read())
                    {
                        result = reader.GetValue(0).ToString();
                        //lV = reader.GetValue(1).ToString();
                    }
                    net_simplify_server.SendMessage(state, handle, result);
                    richTextBox1.Text += state.ToString() + "\t" + handle.ToString() + "\t" + result + "\r\n";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
                }
            }
            else
            {
                net_simplify_server.SendMessage(state, handle, data);
                this.richTextBox1.Text += "发送的数据" + data.ToString();
            }
        }

        private void ProcessCodeMajorTwo(AsyncStateOne state, NetHandle handle, string data)
        {
            if (handle.CodeIdentifier == 1)
            {
                string sql = string.Empty;
                //下面可以再if..else
                //net_simplify_server.SendMessage(state, handle, "测试数据大类2，命令1，接收到的数据是：" + data);
                //this.richTextBox1.Text += "发送的数据" + data.ToString();
                sql = "INSERT INTO UserLogin (UserName,PassWord,Level) VALUES ('" + data + "',0)";
                richTextBox1.Text += sql + "\r\n";
                mc.ExeUpdate(sql);
                net_simplify_server.SendMessage(state, handle, "添加成功");
            }
            else
            {
                net_simplify_server.SendMessage(state, handle, data);
                this.richTextBox1.Text += "发送的数据" + data.ToString();
            }
        }

        private void ProcessCodeMajorThree(AsyncStateOne state, NetHandle handle, string data)
        {
            if (handle.CodeIdentifier == 1)
            {
                // 下面可以再if..else
                //net_simplify_server.SendMessage(state, handle, "测试数据大类3，命令1，接收到的数据是：" + data);
                //this.richTextBox1.Text += "发送的数据" + data.ToString();
                try
                {
                    string sql = string.Empty;
                    sql = "select UserID from UserLogin where UserName=\"" + data + "\"";
                    //string sql = textBox1.Text;
                    richTextBox1.Text += sql + "\r\n";
                    string result = string.Empty;
                    //执行查询
                    MySqlDataReader reader = mc.ExeQuery(sql);
                    while (reader.Read())
                    {
                        result = reader.GetValue(0).ToString();
                    }
                    if(result != string.Empty)
                    {
                        result = "0";
                        net_simplify_server.SendMessage(state, handle, result);
                        richTextBox1.Text += state.ToString() + "\t" + handle.ToString() + "\t" + result + "\r\n";
                    }
                    else
                    {
                        result = "1";
                        net_simplify_server.SendMessage(state, handle, result);
                        richTextBox1.Text += state.ToString() + "\t" + handle.ToString() + "\t" + result + "\r\n";

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK);
                }
            }
            else
            {
                net_simplify_server.SendMessage(state, handle, data);
                this.richTextBox1.Text += "发送的数据" + data.ToString();
            }
        }
    }
}
