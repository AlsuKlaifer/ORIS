using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Space.Objects
{
    internal class Comet
    {
        public int Width { get; set; } = 40;
        public int Height { get; set; } = 40;
        public int Speed { get; set; }
        public int HP { get; set; }
        public bool isAlive { get; set; } = true;
        public Point StartLocation { get; set; }
        public PictureBox CometBody { get; private set; } = new PictureBox();

        public Comet(Point startLocation)
        {
            StartLocation = startLocation;
            InitializeObject();
        }

        private void InitializeObject()
        {
            CometBody.Image = Image.FromFile(Path.Join(Directory.GetCurrentDirectory(), @"/Images/comet.png"));

            CometBody.Location = StartLocation;

            CometBody.SizeMode = PictureBoxSizeMode.StretchImage;
            CometBody.Width = Width;
            CometBody.Height = Height;
        }
        public PictureBox Draw()
        {
            return CometBody;
        }

    }
}
