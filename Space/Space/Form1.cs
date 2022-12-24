using Space.Objects;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
//using Timer = System.Threading.Timer;
using Timer = System.Windows.Forms.Timer;

namespace Space
{
    public partial class Form1 : Form
    {
        Ship ship;
        Timer colliderTimer = new Timer();


        List<Comet> comets = new List<Comet>();
        List<Bullet> bullets = new List<Bullet>();
        public Form1()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Visible = false;
            btnStart.Enabled = false;

            ship = new Ship(new Point(Width / 2 - 50, Height - 200), Width, Height);
            Controls.Add(ship.Draw());

            int cometsWidth = 62;
            int cometsCount = new Random().Next(20, 28);
            for (int i = 0; i < cometsCount; i++)
            {
                Comet comet = new Comet(new Point(i%13 * cometsWidth, (i/13 + 1) * cometsWidth));
                //Comet comet = new Comet(new Point(new Random().Next(0, Width), new Random().Next(0, (int)(Height / 2))), new Random().Next(10, 50));
                Controls.Add(comet.Draw());
                comets.Add(comet);
            }

            colliderTimer.Interval = 100;
            colliderTimer.Tick += ColliderTimer_Tick;
            colliderTimer.Start();
        }

        private void ColliderTimer_Tick(object? sender, EventArgs e)
        {
            foreach (Comet c in comets)
            {
                foreach (Bullet b in bullets)
                {
                    if(b.isAlive && c.isAlive)
                        if (c.CometBody.Bounds.IntersectsWith(b.BulletBody.Bounds))
                        {
                            Controls.Remove(b.Draw());
                            Controls.Remove(c.Draw());
                            c.isAlive = false;
                            b.isAlive = false;
                        }
                    //if (ship.Body.Bounds.IntersectsWith(c.CometBody.Bounds))
                    //    gameOver();
                }
            }

            comets.RemoveAll(c => !c.isAlive);
            bullets.RemoveAll(b => !b.isAlive);
            if (comets.Count == 0)
                Win();
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys.Down):
                    ship.MoveDown();
                    break;
                case (Keys.Up):
                    ship.MoveUp();
                    break;
                case (Keys.Left):
                    ship.MoveLeft();
                    break;
                case (Keys.Right):
                    ship.MoveRight();
                    break;
                case (Keys.Space):
                    BulletFly();
                    break;
                case(Keys.ShiftKey):
                    ship.UseShield(timerLabel);
                    break;
                default:
                    MessageBox.Show($"{e.KeyCode}");
                    break;
            }
        }

        private void BulletFly()
        {
            Bullet bullet = ship.Attack();
            Controls.Add(bullet.Draw());
            bullet.MoveUp();
            bullets.Add(bullet);
        }

        private void Form_KeyDown(object sender, KeyPressEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        public void Win()
        {
            labelWin.Visible = true;
            labelWin.Text = "You won!!!";
        }

        public void gameOver()
        {
            labelWin.Visible = true;
            labelWin.Text = "Game over";
        }
    }
}