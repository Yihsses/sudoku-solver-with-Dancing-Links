using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{   
    public partial class Form1 : Form
    {    
        public class dlx
    {
        int n, m, idx;
        int[]  size = new int[10000];
        int[] first = new int[10000];
       int[] Left = new int[10000] , Righ = new int[10000] , Up = new int[10000] , Down = new int[10000];
        int[] col = new int[10000] , row = new int[10000], record_ans = new int[10000];
        public void build(int rr, int c) //構建交叉十字循環雙向鏈
        {
            n = rr;
            m = c;
            for (int i = 0; i<= c; i++)
            {
                Left[i] = i - 1; //標元素Ci指向左邊
                Righ[i] = i + 1; //標元素Ci指向右邊
                Down[i] = i; //因為還沒插入任何元素所以指向自己
                Up[i] = i;//
            }

            Left[0] = c; //建構Head元素左指向最後標元素
            Righ[c] = 0;//建構最後標元素只回Head元素
            idx = c;//
            for (int i = 0; i<= first.Length - 1; i++) //初始化
            {
                first[i] = 0;
            }
            for (int i = 0; i <= size.Length - 1; i++)
            {
                size[i] = 0;
            }
        }
        public void insert(int rr , int  c )//插入元素在十字循環雙向鏈裡
        {
  idx += 1 ;
            col[idx] = c;
            row[idx] = rr;
            size[c] += 1;
            Down[idx] = Down[c];
            Up[Down[c]] = idx;
            Up[idx] = c;
            Down[c] = idx;
            if (first[rr] == 0)
            {  //如果那一行沒有任何一個元素
                //紀錄插入那行首個元素的idx方便後續元素的插入
                first[rr] = idx;
                Left[idx] = idx;
                Righ[idx] = idx;
            }
            else
            {
                //插入元素在第一個元素的右邊
                Righ[idx] = Righ[first[rr]];
                Left[Righ[first[rr]]] = idx;
                Left[idx] = first[rr];
                Righ[first[rr]] = idx;
            }
  }
        public void remove(int c) // 從十字循環雙向鏈刪除有關C的列與行 
        {
            // 刪除其實並不是實際刪除元素其實還在只是改變指向
            Left[Righ[c]] = Left[c];
            Righ[Left[c]] = Righ[c];
            int a = Down[c]; // 從上到下
            while (a != c)
            {
                int b = Righ[a];
                while (b != a)
                {
                    Up[Down[b]] = Up[b];
                    Down[Up[b]] = Down[b];
                    size[col[b]] -= 1;
                    b = Righ[b]; // 從左到右
                }
                a = Down[a];
            }
        }
        public void recover(int c) // 從十字循環雙向鏈恢復元素指向
        {
            // 根據刪除順序逆推回去
            int a = Up[c]; // 從下到上
            while (a != c)
            {
                int b = Left[a];
                while (b != a)
                {
                    Up[Down[b]] = b;
                    Down[Up[b]] = b;
                    size[col[b]] += 1;
                    b = Left[b]; // 從右到左
                }
                a = Up[a];
            }
            Left[Righ[c]] = c;
            Righ[Left[c]] = c;
        }

        public bool dance(int run,  ref int[,] ans) // 開始遞迴、刪除、回復，直到找到精準覆蓋解
        {
            if (Righ[0] == 0)
            {
                for (var i = 1; i <= run - 1; i++)
                {
                    // 回復成數讀版面 計算行列以及值
                    int re = (record_ans[i] - 1) / 81 + 1;
                    int ce = ((record_ans[i] - 1) / 9) % 9 + 1;
                    int w = (record_ans[i] - 1) % 9 + 1;
                    ans[re - 1, ce - 1] = w;
                }
                return true;
            }
            int cc = Righ[0];
            int a = Righ[0];
            a = Righ[a];
            // 找哪列元素最少從那一列開始刪
            while (a != 0)
            {
                if (size[a] < size[cc])
                    cc = a;
                a = Righ[a];
            }
            remove(cc);
            a = Down[cc];

            while (a != cc)
            {
                record_ans[run] = row[a]; // 紀錄解答
                int b = Righ[a];
                while (b != a) // 把有指向這行的列首元素進行刪除
                {
                    remove(col[b]);
                    b = Righ[b];
                }
                if (dance(run + 1, ref ans))
                    return true;
                b = Left[a];
                while (b != a) // 沒找到解進行恢復刪除另一行
                {
                    recover(col[b]);
                    b = Left[b];
                }
                a = Down[a];
            }
            recover(cc);
            return false; // 沒找到解回上一層
        }

    }
        string[,] map = new string[9, 9];
        TextBox[,] sudoku_number = new TextBox[9, 9];
        public static int asc(string S)
        {
            int N = Convert.ToInt32(S[0]);
            return N;
        }
        public void sudoku_insert(int r, int c, int w , ref dlx solver)
        {
            int row = (r - 1) * 81 + (c - 1) * 9 + w; // 數字1-9 列1-9 行 1-9 所對應的行在哪                                         // 定義4個約束條件
            int c1 = (r - 1) * 9 + w; // 約束1:每個格子只能填一個數字
            int c2 = 81 + (c - 1) * 9 + w; // 約束2:每列1-9都要填一遍
            int c3 = 162 + (((r - 1) / 3 + 1) + ((c - 1) / 3) * 3 - 1) * 9 + w; // 約束3:每宮1-9的這9個數字都得填一遍
            int c4 = 243 + (r - 1) * 9 + c; // 約束4:每行1-9都要填一遍
            solver.insert(row, c1);
            solver.insert(row, c2);
            solver.insert(row, c3);
            solver.insert(row, c4);
        }
      void sudoku_KeyDown(object sender, KeyEventArgs e) // 允許刪除
        {
            if (e.KeyData == Keys.Back)
            {
                e.Handled = false;
                (sender as TextBox).Text = "" ;
            }
            else
                e.Handled = true;
        }
       void sudoku_KeyPress(object sender, KeyPressEventArgs e) // 限定只能輸入數字以及數獨原則判斷
        {
            string[] xy = (sender as TextBox).Name.ToString().Split(' ');
            if (sudoku_number[Convert.ToInt32( xy[0]), Convert.ToInt32(xy[1])].Text.Length == 1)
            {
                e.Handled = true;
                return;
            }
            if (e.KeyChar == '0')
            {
                MessageBox.Show("不能輸入0",  "解數獨");
                e.Handled = true;
                return;
            }
            for (var x = 0; x <= 8; x++)
            {
                if (Convert.ToInt32( xy[0]) != x & Convert.ToString(e.KeyChar) != "" )
                {
                    if ( sudoku_number[x, Convert.ToInt32(xy[1])].Text == Convert.ToString(e.KeyChar))
                    {
                        MessageBox.Show("長寬不能有重複", "解數獨");
                        e.Handled = true;
                        return;
                    }
                }
            }
            for (var y = 0; y <= 8; y++)
            {
                if (Convert.ToInt32(xy[1]) != y & Convert.ToString(e.KeyChar) != "")
                {
                    if (sudoku_number[Convert.ToInt32(xy[0]), y].Text == Convert.ToString(e.KeyChar))
                    {
                        MessageBox.Show("長寬不能有重複", "解數獨");
                        e.Handled = true;
                        return;
                    }
                }
            }
            for (var x = 3 * (Convert.ToInt32(xy[0]) / 3); x <= 3 * (Convert.ToInt32(xy[0]) / 3) + 2; x++)
            {
                for (var y = 3 * (Convert.ToInt32(xy[1]) / 3); y <= 3 * (Convert.ToInt32(xy[1]) / 3) + 2; y++)
                {
                    if (sudoku_number[x, y].Text ==Convert.ToString( e.KeyChar))
                    {
                      MessageBox.Show("每一宮不能有重複","解數獨");
                        e.Handled = true;
                        return;
                    }
                }
            }
            if (asc(Convert.ToString( e.KeyChar)) < asc("0") | asc(Convert.ToString(e.KeyChar)) > asc("9"))
                e.Handled = true;
            else
                e.Handled = false;
        }



        public Form1()
        {
            InitializeComponent();
            // 建立textbox 以及 PictureBox 物件陣列
            int lx, ly;
            PictureBox[] lin = new PictureBox[101];
            int lin_index = 0;
            lx = 85;
            ly = 50;
            for (var y = 0; y <= 8; y++)
            {
                for (var x = 0; x <= 8; x++)
                {
                    if (x >= 3 & x % 3 == 0)
                        lx += 10;
                    // 屬性設定
                    sudoku_number[x, y] = new TextBox();
                    sudoku_number[x, y].Multiline = true;
                    sudoku_number[x, y].Font = new Font("font", 20);
                    sudoku_number[x, y].Size = new Size(50, 50);
                    sudoku_number[x, y].Location = new Point(lx, ly);
                    sudoku_number[x, y].TextAlign = HorizontalAlignment.Center;
                    sudoku_number[x, y].BorderStyle = BorderStyle.FixedSingle;
                    sudoku_number[x, y].MaxLength = 1;
                    sudoku_number[x, y].Name = x + " " + y;
                     sudoku_number[x, y].KeyDown +=new KeyEventHandler(sudoku_KeyDown);
                   sudoku_number[x, y].KeyPress += new KeyPressEventHandler(sudoku_KeyPress);
               
                    Controls.Add(sudoku_number[x, y]);
                    if (x == 8)
                        lx += 60;
                    if (((x == 0 | x % 3 == 0) & y == 0) | x == 8)
                    {
                        // 畫黑線
                        lin[lin_index] = new PictureBox();
                        lin[lin_index].Size = new Size(10, 50 * 9 + 50);
                        lin[lin_index].Location = new Point(lx - 10, 50 - 10);
                        Bitmap bmp = new Bitmap(10, 50 * 9 + 50);
                        Graphics g = Graphics.FromImage(bmp);
                        g.FillRectangle(Brushes.Black, 0, 0, 10, 50 * 9 + 40);
              
                        lin[lin_index].Image =  bmp;
                        Controls.Add(lin[lin_index]);
                    }
                    lx += 50;
                }
                lx = 85;
                if ((y + 1) % 3 == 0 & y >= 2)
                {
                    // 畫黑線
                    ly += 60;
                    lin[lin_index] = new PictureBox();
                    lin[lin_index].Size = new Size(50 * 9 + 30, 10);
                    lin[lin_index].Location = new Point(lx, ly - 10);
                    Bitmap bmp = new Bitmap(50 * 9 + 30, 25);
                    Graphics g = Graphics.FromImage(bmp);
                    g.FillRectangle(Brushes.Black, 0, 0, 50 * 9 + 30, 10);
                    lin[lin_index].Image = bmp;
                    Controls.Add(lin[lin_index]);
                }
                else if (y == 0)
                {
                    // 畫黑線
                    lin[lin_index] = new PictureBox();
                    lin[lin_index].Size = new Size(50 * 9 + 30, 10);
                    lin[lin_index].Location = new Point(lx, ly - 10);
                    Bitmap bmp = new Bitmap(50 * 9 + 30, 25);
                    Graphics g = Graphics.FromImage(bmp);
                    g.FillRectangle(Brushes.Black, 0, 0, 50 * 9 + 30, 10);
                    lin[lin_index].Image = bmp;
                    Controls.Add(lin[lin_index]);

                    ly += 50;
                }
                else
                    ly += 50;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
         
        }

        private void button1_Click(object sender, EventArgs e)
        {
            dlx solver = new dlx();
            for (var y = 0; y <= 8; y++)
            {
                for (var x = 0; x <= 8; x++)
                    map[y, x] = sudoku_number[x, y].Text;
            }
            solver.build(729, 324); // 有四個約束每一個約束有81列81*4 = 324列
                                    // 每一個格子可以填9個數字每一行有9個格子一共有9列 因此9*9*9 =729行

            // 插入元素在十字循環雙向鏈
            for (var i = 1; i <= 9; i++)
            {
                for (var j = 1; j <= 9; j++)
                {
                    if ( map[i - 1, j - 1] != "")
                    {
                        sudoku_insert(i, j, Convert.ToInt32(map[i - 1, j - 1]),ref solver);
                        continue;
                    }
                    else
                        sudoku_number[j - 1, i - 1].ForeColor = Color.Red;
                    for (var k = 1; k <= 9; k++)
                        sudoku_insert(i, j, k, ref solver);
                }
            }
            int[,] map2 = new int[9, 9];
            for (int y = 0; y <=8; y++)
                for (int x = 0; x <=8; x++)
                 if (map[y, x] == "") map2[y, x] =0; else   map2[y, x] = Convert.ToInt32(map[y, x]);
                         
            if (solver.dance(1, ref map2))
            {
                for (var y = 0; y <= 8; y++)
                {
                    for (var x = 0; x <= 8; x++)
                        sudoku_number[x, y].Text =Convert.ToString( map2[y, x]);
                }
            }
            else
              MessageBox.Show("找不到解",  "解數獨");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int y = 0; y <= 8; y++)
            {
                for (int x = 0; x <= 8; x++)
                {
                    sudoku_number[x, y].ForeColor = Color.Black;
                    sudoku_number[x, y].Text = "";
                    map[x, y] = "";
                }
            }
        }
    }
}
