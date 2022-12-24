using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using Timer = System.Windows.Forms.Timer;

namespace Space.Objects
{
    public class Bullet
    {
        public int Width { get; set; } = 10;
        public int Height { get; set; } = 10;
        public int Speed { get; set; } = 5;
        public int Damage { get; set; } = 10;
        public Point StartLocation { get; set; }
        public bool isAlive { get; set; } = true;
        public PictureBox BulletBody { get; private set; } = new PictureBox();

        private Timer timer = new Timer();

        public Bullet(Point startLocation)
        {
            StartLocation = startLocation;
            InitializeObject();
        }

        private void InitializeObject()
        {
            BulletBody.Image = Image.FromFile(Path.Join(Directory.GetCurrentDirectory(), @"/Images/bullet.png"));

            BulletBody.Location = StartLocation;

            BulletBody.BackColor = Color.Transparent;

            BulletBody.SizeMode = PictureBoxSizeMode.StretchImage;
            BulletBody.Width = Width;
            BulletBody.Height = Height;
        }
        public PictureBox Draw()
        {
            return BulletBody;
        }
        public void MoveUp()
        {
            timer.Interval = 15;
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (BulletBody.Location.Y <= 0)
                timer.Stop();
            BulletBody.Location = new Point(BulletBody.Location.X, BulletBody.Location.Y - Speed);
        }
    }
}
