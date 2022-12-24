using Space.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;

namespace Space
{
    public class Ship
    {
        public int HP { get; set; }
        public int Damage { get; set; }
        public int Speed { get; set; } = 10;
        public int Width { get; set; } = 70;
        public int Height { get; set; } = 70;
        public Point StartLocation { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }

        public PictureBox Body { get; set; } = new PictureBox();
        private PictureBox shield = new PictureBox();
        private Timer timer = new Timer();
        private int reloadTimer;
        private bool canUseShield = true;
        private Label timerLabel;

        public Ship(Point startLocation, int screenWidth, int screenHeight)
        {
            StartLocation = startLocation;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            InitializeObject();

            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
        }

        private void InitializeObject()
        {
            Body.Image = Image.FromFile(Path.Join(Directory.GetCurrentDirectory(), @"/Images/ship.png"));

            shield.Image = Image.FromFile(Path.Join(Directory.GetCurrentDirectory(), @"/Images/barrier.png"));
            shield.SizeMode = PictureBoxSizeMode.Zoom;
            shield.Width = Width + 20;
            shield.Height = Height + 20;
            shield.Location = new Point(-10,-10);

            Body.Location = StartLocation;

            Body.SizeMode = PictureBoxSizeMode.Zoom;
            Body.Width = Width;
            Body.Height = Height;
        }
        public PictureBox Draw()
        {
            return Body;
        }
        public void MoveUp()
        {
            if (Body.Location.Y + Speed + 0.1 * Height >= 0)
                Body.Location = new Point(Body.Location.X, Body.Location.Y - Speed);

        }
        public void MoveDown()
        {
            if (Body.Location.Y + Speed + 1.2 * Height <= ScreenHeight)
                Body.Location = new Point(Body.Location.X, Body.Location.Y + Speed);
        }
        public void MoveLeft()
        {
            if (Body.Location.X - Speed >= 0)
                Body.Location = new Point(Body.Location.X - Speed, Body.Location.Y);
        }
        public void MoveRight()
        {
            if (Body.Location.X + Speed + Width <= ScreenWidth)
                Body.Location = new Point(Body.Location.X + Speed, Body.Location.Y);
        }
        public Bullet Attack()
        {
            return new Bullet(new Point(Body.Location.X + Width/2 - 5, Body.Location.Y));
        }

        public void UseShield(Label label)
        {
            if (!canUseShield)
                return;

            timerLabel = label;

            Body.Controls.Add(shield);
            reloadTimer = 10;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            reloadTimer--;
            timerLabel.Text = "00:" + (reloadTimer<10 ? "0"+reloadTimer : reloadTimer);
            if (canUseShield) {
                if(reloadTimer <= 0)
                {
                    Body.Controls.Remove(shield);
                    reloadTimer = 60;
                    canUseShield = false;
                }
            } else
            {
                if (reloadTimer <= 0)
                {
                    canUseShield = true;
                timer.Stop();
            }
            }
        }
    }
}
