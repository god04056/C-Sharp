using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;


namespace Term_Project
{
    public partial class Form1 : Form
    {
        private string text, str1, str2, str3, equalStr;
        private string[] check;
        private string linkWord;
        private char[] cha1;
        private char[] cha2;
        private int n = 0, sourcestack = 0;
        private List<string> sourceList = new List<string>(); //위의 데이터형태로 리스트 생성
        private List<string> sampleList = new List<string>();
        List<Data> purposeList = new List<Data>();

        private SolidBrush[] Palette = new SolidBrush[10];
        private Pen[] pen = new Pen[10];
        private int Gred, Ggreen, Gblue;
        private float[] valueArray = new float[10];
        int Num = 0;

        public Form1()
        {
            InitializeComponent();
            for (int i = 0; i < 10; i++)
            {
                listBox2.Items.Add("");
            }
            this.Paint += Form_Paint;
        }
        private void checkChat(string chatLog)   //문자열 검사를 실행
        {
            check = chatLog.Split(' ');     //문자열을 ' '<- 띄어쓰기에 따라 나누어 배열에 저장
            for (int i = 0; i < check.Length; i++)  //문자열 검사가 이루어진다.
            {
                text = check[i];
                if (text.Length == 1)       //글자 수 가 1일 경우 스킵
                    continue;
                else
                {
                    pushChat();         //소스가 될 문자열을 리스트에 저장
                    sourcestack++;    //이는 배열에 현재 몇번 들어갔는지 횟수
                }
            }
            checking();
            text = "";
        }
        private void checking()
        {
            for (int j = sourceList.Count - sourcestack; j < sourceList.Count; j++)      //들어간 문자열 배열 갯수만큼 반복
            {
                cha1 = sourceList[j].ToCharArray();         //들어온 문자열을 char형으로 변환해 한글자씩 나눠서 저장
                for (int k = 0; k < sourceList.Count; k++)
                {
                    cha2 = sourceList[k].ToCharArray();     //비교할 문자열을 char형으로 변환해 한글자 씩 나눠서 저장
                    if (cha1[0].Equals(cha2[0]) && cha1[1].Equals(cha2[1])) //얖글자 2글자가 서로 같은 지 확인
                    {
                        if (cha1.Length <= cha2.Length)     //글자수 가 적은 쪽만큼 반복
                            sampling(cha1.Length);      //cha1의 글자수가 적거나 같을 경우
                        else
                            sampling(cha2.Length);      //cha2의 글자수가 적을 경우
                    }
                    else
                        continue;
                }

            }
            cha1 = new char[] { };
            cha2 = new char[] { };
            sourcestack = 0;
        }
        private void sampling(int a)     //글자 수가 적은 쪽만큼 해당 글자를 샘플링하여 리스트에 저장
        {
            while (n < a)
            {
                str1 += cha1[n];
                str2 += cha2[n];

                if (!str1.Equals(str2))       // '한국문화' 와 '한국사람'이 있을 경우, '한국'이라는 글자는 같지만
                {                             // '문화'와 '사람'은 같지 않기에 '한국문', '한국사'일경우 반복문을 벗어난다.
                    break;
                }
                else
                    equalStr = str1;        //'한국'이라는 글자는 동일 하기에 해당 글자를 저장한다.

                n++;
            }
            getSample();
            n = 0;      //반복횟수 초기화
            str1 = "";      //사용된 문자열 초기화
            str2 = "";
            equalStr = "";      //저장시킬 문자 초기화
        }
        private void pushChat()  //리스트 형태로 소스가 될 문자열 입력
        {
            sourceList.Add(text);
        }
        private void getSample()     //소스데이터에서 샘플링된 데이터를 사용할 샘플리스트에 집어 넣는다.
        {
            sampleList.Add(equalStr);       //샘플링된 데이터를 집어넣은 후
            sampleList = sampleList.Distinct().ToList();    //중복된 단어가 있을 시 제거한다.
        }
        private void pushData(int b)      //생성된 데이터와 나온 횟수, 출력여부를 저장
        {
            while (b < sampleList.Count)
            {
                if (purposeList.Count != sampleList.Count)
                {
                    purposeList.Add(new Data() { word = sampleList[b], count = 1 });
                    b++;
                }
                else
                    break;
            }
        }
        private void counting()          //단어가 몇 번 나왔는 지 세어준다.
        {
            purposeList = purposeList.GroupBy(p => p.getWord()).Select(g => g.First()).ToList();    //샘플링 된 데이터가 중복으로 들어가는 문제 방지
            for (int i = 0; i < purposeList.Count; i++)
            {
                str3 = purposeList[i].word;                     //완전히 샘플링된 문자열 데이터를 
                purposeList[i].count = sourceList.FindAll(x => x.Contains(str3)).Count;     //소스데이터에서 해당 단어가 나온 횟수를 세어준다.
            }
        }
        private void sorting()
        {
            purposeList.Sort((x1, x2) => x2.count.CompareTo(x1.count));     //빠른 검색을 위해 count(나온횟수)를 기준으로 내림차순 정렬
        }
        private void load_Data()
        {
            for (int i = 0; i < purposeList.Count; i++)
            {
                if (i == 10)
                    break;
                else
                    listBox2.Items[i] = purposeList[i].word + "\t" + purposeList[i].count.ToString();
            }

        }

        private void MakeColor()
        {
            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                Gred = rand.Next(80, 250);
                Ggreen = rand.Next(80, 250);
                Gblue = rand.Next(80, 250);
                Palette[i] = new SolidBrush(Color.FromArgb(255, Gred, Ggreen, Gblue));
                pen[i] = new Pen(Color.FromArgb(255, Gred, Ggreen, Gblue));
            }
        }
        private void Form_Load()
        {
            for (int i = 0; i < purposeList.Count; i++)
            {
                if (i == 10)
                    break;
                else
                {
                    int value = purposeList[i].count;
                    this.valueArray[i] = (float)value;
                }
            }
        }
        private void Form_Paint(object sendor, PaintEventArgs e)
        {
            e.Graphics.Clear(BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int size = 250;
            int left = 12;
            int top = ClientSize.Height/2;

            Rectangle rectangle = new Rectangle(left, top, size, size);
            MakeColor();
            DrawPieChart(e.Graphics, rectangle, this.Palette, this.pen, this.valueArray);
        }
        private void DrawPieChart(Graphics graphics, Rectangle rectangle, SolidBrush[] Palette, Pen[] pen, float[] valueArray)
        {
            float totalValue = valueArray.Sum();
            float startAngle = 0;
            for (int i = 0; i < valueArray.Length; i++)
            {
                float sweepAngle = valueArray[i] * 360f / totalValue;
                graphics.FillPie(Palette[i % Palette.Length], rectangle, startAngle, sweepAngle);
                graphics.DrawPie(pen[i % pen.Length], rectangle, startAngle, sweepAngle);

                startAngle += sweepAngle;
            }
        }
        private void make_Label()
        {
           Label[] label1 = new Label[10];
            Random rand = new Random();
            int Lx, Ly;
            float Sum = valueArray.Sum();

            for(int i=0; i<purposeList.Count; i++)
            {
                if (i == 10)
                    break;
                else
                {
                    Controls.Remove(label1[i]);
                }

            }
            for (int i = 0; i < purposeList.Count; i++)
            {
                if (i == 10)
                    break;
                else
                {
                    label1[i] = new Label();
                    label1[i].AutoSize = true;
                    label1[i].Size = new System.Drawing.Size(20, 40);
                    label1[i].BackColor = Color.LightGray;
                    label1[i].Name = "linkLabel" + i + 1;
                    label1[i].Text = purposeList[i].word;
                    linkWord = label1[i].Text;
                    Controls.Add(label1[i]);
                }
            }

            label1[0].Location = new System.Drawing.Point(ClientSize.Width / 2, ClientSize.Height / 2);
            for (int i = 1; i < purposeList.Count; i++)
            {
                if (i == 10)
                    break;
                else
                {
                    Lx = rand.Next(336, 872);
                    Ly = rand.Next(100, ClientSize.Height - 100);
                    label1[i].Location = new System.Drawing.Point(Lx, Ly);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Controls.Clear();
            listBox1.Items.Add(textBox1.Text);
            checkChat(textBox1.Text);
            pushData(Num);
            sorting();
            counting();
            textBox1.Text = "";
            load_Data();
            Form_Load();
            make_Label();
            Refresh();
        }
       
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                listBox1.Items.Add(textBox1.Text);
                checkChat(textBox1.Text);
                pushData(Num);
                sorting();
                counting();
                textBox1.Text = "";
                load_Data();
                Form_Load();
                make_Label();
                Refresh();
            }
        }

    }
    class Data
    {
        public string word { get; set; }       //저장할 문자를 문자열
        public int count { get; set; }          //해당 문자열이 나온 횟수

        public string getWord()
        {
            return word;
        }
    }
}
