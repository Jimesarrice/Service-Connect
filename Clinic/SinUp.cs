using HslCommunication;
using HslCommunication.Enthernet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clinic
{
    public partial class SinUp : Form
    {
        public bool User = false;

        public SinUp()
        {
            InitializeComponent();
        }
        public static NetSimplifyClient Net_simplify_client { get; set; } = new NetSimplifyClient(
        new IPEndPoint(IPAddress.Parse("139.199.227.148"), 15500))
        //new IPEndPoint(IPAddress.Parse("127.0.0.1"), 15500))  // 指定服务器的ip，和服务器设置的端口
        {
            KeyToken = new Guid("3ee0dbce-3489-4634-9706-fedf7bb61741"), // 这个guid码必须要服务器的一致，否则服务器会拒绝连接
            ConnectTimeout = 5000,// 连接的超时时间
        };

        private void button1_Click(object sender, EventArgs e)
        {
            if (UserName.Text == string.Empty&&PassWord .Text ==string .Empty )
            {
                MessageBox.Show("用户名或密码不能为空");
            }
            else
            {
                if (User)
                {
                    string uplodstring = string.Empty;
                    uplodstring = UserName.Text + "','" + PassWord.Text;
                    OperateResult<string> result = Net_simplify_client.ReadFromServer(new NetHandle(2, 1, 1), uplodstring); // 指示了大类2，子类1，编号1

                    if (result.IsSuccess)
                    {
                        // 按照上面服务器的代码，此处显示数据为："上传成功！返回的数据：测试数据大类1，命令1，接收到的数据是：发送的数据"
                        //MessageBox.Show(result.Content);
                        richTextBox1.Text += result.Content + "\r\n";
                        User = false;
                    }
                    else
                    {
                        MessageBox.Show("操作失败！原因：" + result.Message);// 失败的原因基本上是连接不上，如果GUID码填写错误，也会连接不上
                    }
                }
                else
                {
                    MessageBox.Show("用户名被占用或未检测");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(UserName.Text ==string .Empty )
            {
                MessageBox.Show("用户名不能为空");
            }
            else
            {
                OperateResult<string> result = Net_simplify_client.ReadFromServer(new NetHandle(3, 1, 1), UserName.Text); // 指示了大类1，子类1，编号1
                                                                                                                          //new NetHandle(1, 1, 2),PassWord.Text);
                if (result.IsSuccess)
                {
                    // 按照上面服务器的代码，此处显示数据为："上传成功！返回的数据：测试数据大类1，命令1，接收到的数据是：发送的数据"
                    //MessageBox.Show(result.Content);
                    //richTextBox1.Text += result.Content + "\r\n";
                    if (result.Content == "1")
                    {
                        richTextBox1.Text += "用户名可以使用" + "\r\n";
                        User = true;
                    }
                    else
                    {
                        richTextBox1.Text += "用户名被占用" + "\r\n";
                        User = false;
                    }
                }
            }
        }
    }
}
