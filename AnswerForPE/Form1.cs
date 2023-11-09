using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnswerForPE
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //List<string> list ;
        Dictionary<string, Entry> dict = new Dictionary<string, Entry>();
        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            //richTextBox1.Enabled = false;
            toolStripStatusLabel2.Alignment = ToolStripItemAlignment.Right;
            StreamReader file=null;
            try
            {
                file = File.OpenText("data_enhance.txt");
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error!can not find data_enhance.txt");
            }
            
            var txt_raw = file.ReadToEnd();
            var splited = txt_raw.Split(new string[] { "\r\n\r\n", }, StringSplitOptions.RemoveEmptyEntries);
/*            var list =new List<string>();
            foreach (var i in splited)
            {
                bool isAlreadyAdd = false;
                var split_symbol = new string[] { "A\r\n", "B\r\n", "C\r\n", "D\r\n", "正确\r\n", "错误\r\n" };
                foreach (var s in split_symbol)
                {
                    var slice = i.Split(new string[] { s }, StringSplitOptions.RemoveEmptyEntries);
                    if (slice.Length > 1)
                    {
                        isAlreadyAdd = true;
                        for (int count = 0; count < slice.Length - 1; count++)
                        {
                            list.Add(slice[count] + s);
                        }
                        list.Add(slice.Last());
                        break;
                    }
                }
                if (!isAlreadyAdd)
                {
                    list.Add(i);
                }
            }*/

            foreach (var i in splited)
            {
                if (i == "\r\n")
                {
                    continue;
                }
                int index=i.IndexOf("\r\n");
                string question = i.Substring(0, index);
                string answer = i.Substring(index, i.Length-index);
                if (dict.ContainsKey(question))
                {
                    if (dict[question].Prority < 1)
                    {
                        Entry entry1 = new Entry(question, answer);
                        if (entry1.Prority > 0)
                        {
                            dict[question] = entry1;
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    Entry entry1 = new Entry(question, answer);
                    dict[question] = entry1;
                }
            }
       

            label.Text = "关键字";
            textBox1.Focus();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                richTextBox1.Text = "在输入框中输入题目关键字以查询";
                return;
            }
            //GCSettings.LatencyMode = GCLatencyMode.Batch;
            richTextBox1.Text = "";
            StringBuilder sb = new StringBuilder();
            int count = 0;
            foreach (var item in dict)
            {
                if (item.Key.Contains(textBox1.Text)||item.Value.Answer.Contains(textBox1.Text))
                {
                    sb.Append(item.Key);
                    //sb.Append('\n');
                    sb.Append(item.Value.Answer);
                    sb.Append("\n\n");
                    count++;
                }
            }
            richTextBox1.Text += sb.ToString();
            toolStripStatusLabel2.Text = $"共找到{count}个答案";
            //GCSettings.LatencyMode = GCLatencyMode.Interactive;
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel3_Click(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //不使用shell启动
            p.StartInfo.RedirectStandardInput = true;//喊cmd接受标准输入
            p.StartInfo.RedirectStandardOutput = false;//不想听cmd讲话所以不要他输出
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示窗口
            p.Start();

            //向cmd窗口发送输入信息 后面的&exit告诉cmd运行好之后就退出
            p.StandardInput.WriteLine("start " + "https://github.com/57UU" + "&exit");
            //p.StandardInput.WriteLine("iexplore.exe " + url + "&exit");
            p.StandardInput.AutoFlush = true;
            //p.WaitForExit();//等待程序执行完退出进程
            p.Close();
        }
    }
    class Entry
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public int Prority { get; private set; } = 0;
        public Entry(string question, string answer)
        {
            Question = question;

            int index = answer.IndexOf("标准答案");
            if (index != -1)
            {
                int index2 = answer.IndexOf("你的答案");
                if (index2 != -1)
                {
                    Answer = answer.Substring(0, index2);
                    Answer += answer.Substring(index);
                }
                else
                {
                    Answer = answer;
                }
 
                Prority = 1;
            }
            else
            {
                Answer = answer;
            }

        }
        public override int GetHashCode()
        {
            return Question.GetHashCode();
        }
    }
}