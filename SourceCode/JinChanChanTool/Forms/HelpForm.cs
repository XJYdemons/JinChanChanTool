using System.Net.Mail;
using System.Net;

namespace JinChanChanTool
{
    public partial class HelpForm : Form
    {
        /// <summary>
        /// 用户反馈窗口
        /// </summary>
        public HelpForm()
        {
            InitializeComponent();
            this.ShowIcon = false;//隐藏图标

        }

        // 配置发件邮箱（需要开启SMTP服务的邮箱）
        private const string FromEmail = "2695648907@qq.com"; // 开发者邮箱
        private const string FromPassword = "ayuiexycndboddbh"; // 邮箱密码/应用密码
        private const string SmtpServer = "smtp.qq.com"; // SMTP服务器地址
        private const int SmtpPort = 587; // 端口号

        /// <summary>
        /// 点击发送反馈按钮，收集用户输入的反馈内容和发送者信息，通过SMTP发送邮件到开发者邮箱。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;//防止重复点击
            Cursor = Cursors.WaitCursor;//显示等待光标
            // 输入验证
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                MessageBox.Show("请输入反馈内容");
                return;
            }
            if (string.IsNullOrWhiteSpace(textBox2.Text))
            {
                MessageBox.Show("请输入发送者");
                return;
            }
            // 发送邮件
            try
            {
                using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(FromEmail, FromPassword);
                    client.Timeout = 10000; // 新增超时控制

                    MailMessage message = new MailMessage
                    {
                        From = new MailAddress(FromEmail),
                        Subject = $"用户反馈 - {textBox2.Text} - {DateTime.Now:yyyy-MM-dd HH:mm}",
                        Body = $"版本号：v4.5.1\n反馈内容：{textBox1.Text}",
                        IsBodyHtml = false
                    };
                    message.To.Add(FromEmail); // 发送给自己

                    client.Send(message);
                }
                MessageBox.Show("反馈发送成功！");
                textBox1.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发送失败: {ex.Message}");
            }
            finally
            {
                button1.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void HelpForm_Load(object sender, EventArgs e)
        {

        }
    }
}
