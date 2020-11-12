using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        int bitmapWidth, bitmapHeight, adjustment, rudder, speed;
        Bitmap bitmap;
        Graphics graphics;
        const int rectangleWidth = 10, rectangleHeight = 10;
        LinkedList<Rectangle> snake;
        Size[] direction;
        Random random;
        Rectangle food;

        public Form1()
        {
            InitializeComponent();
            bitmapWidth = pictureBox1.ClientSize.Width;
            bitmapHeight = pictureBox1.ClientSize.Height;
            bitmap = new Bitmap(bitmapWidth, bitmapHeight);
            graphics = Graphics.FromImage(bitmap);
            snake = new LinkedList<Rectangle>();
            direction = new Size[4] { new Size(0, -10), new Size(10, 0), new Size(0, 10), new Size(-10, 0) };
            random = new Random();
            food = new Rectangle(0, 0, rectangleWidth, rectangleHeight);
            speed = timer1.Interval;
        }

        void FoodAppear()
        {
            do
            {
                food.X = random.Next(bitmapWidth / rectangleWidth) * rectangleWidth;
                food.Y = random.Next(bitmapHeight / rectangleHeight) * rectangleHeight;
            }
            while (snake.Contains(food));
            RenewPicture();
        }

        void RenewPicture()
        {
            graphics.Clear(Color.Transparent);
            foreach (Rectangle body in snake)
                graphics.FillRectangle(Brushes.Black, body);
            graphics.FillRectangle(Brushes.Red, food);
            pictureBox1.Image = bitmap;
        }

        void HeadImpact()
        {
            timer1.Stop();
            graphics.DrawString("You Died", new Font(Font.FontFamily, 20, FontStyle.Bold), Brushes.Gray, 50, 90);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            progressBar1.Value = 0;
            rudder = 0;
            snake.Clear();
            snake.AddFirst(new Rectangle(bitmapWidth / rectangleWidth / 2 * rectangleWidth, bitmapHeight / rectangleHeight / 2 * rectangleHeight, rectangleWidth, rectangleHeight));
            FoodAppear();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Rectangle tail = snake.Last.Value;
            adjustment = rudder;
            tail.Location = Point.Add(snake.First.Value.Location, direction[adjustment]);
            snake.AddFirst(tail);

            Rectangle head = snake.First.Value;
            if (head.Location == food.Location)
            {
                if (++progressBar1.Value == progressBar1.Maximum)
                {
                    decimal level = numericUpDown1.Value + 1;
                    if (level > numericUpDown1.Maximum)
                        progressBar1.Value = 0;
                    else
                        numericUpDown1.Value = level;
                }
                FoodAppear();
                return;
            }

            snake.RemoveLast();
            RenewPicture();
            int headX = head.X, headY = head.Y;
            if (headX < 0 || headX >= bitmapWidth || headY < 0 || headY >= bitmapHeight)
            {
                HeadImpact();
                return;
            }
            if (snake.Count > 4)
            {
                LinkedListNode<Rectangle> body = snake.First.Next.Next.Next.Next;
                while (body != null)
                {
                    if (head.Location == body.Value.Location)
                    {
                        HeadImpact();
                        return;
                    }
                    body = body.Next;
                }
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            progressBar1.Value = 0;
            int level = (int)numericUpDown1.Value;
            progressBar1.Maximum = level;
            timer1.Interval = speed - speed / (int)numericUpDown1.Maximum * (level - 1);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Left:
                    rudder = (adjustment + 3) % 4;
                    break;
                case Keys.Right:
                    rudder = (adjustment + 1) % 4;
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
