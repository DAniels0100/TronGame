using System.Collections.Generic;

namespace TronFinal
{
    public partial class Form1 : Form
    {

        private List<Bike> bike = new List<Bike>();
        private Bike Power = new Bike();

        int maxWidth;
        int maxHeight;

        int score;
        int highScore;

        Random random = new Random();

        bool goLeft, goRight, goUp, goDown;


        public Form1()
        {
            InitializeComponent();
            new Settings();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left && Settings.Directions != "right")
            {
                goLeft = true;
            }
            if (e.KeyCode == Keys.Right && Settings.Directions != "left")
            {
                goRight = true;
            }
            if (e.KeyCode == Keys.Up && Settings.Directions != "down")
            {
                goUp = true;
            }
            if (e.KeyCode == Keys.Down && Settings.Directions != "up")
            {
                goDown = true;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void TimeEvent(object sender, EventArgs e)
        {
            
            //setting directions
            if (goLeft) 
            {
                Settings.Directions = "left";
            }
            if (goRight)
            {
                Settings.Directions = "right";
            }
            if (goDown)
            {
                Settings.Directions = "down";
            }
            if (goUp)
            {
                Settings.Directions = "up";
            }
            
            //move bike
            for (int i = bike.Count - 1; i >= 0; i--)
            {
                if (i == 0)
                {
                    switch (Settings.Directions)
                    {
                        case "left":
                            bike[i].X--;
                            break;
                        case "right":
                            bike[i].X++;
                            break;
                        case "up":
                            bike[i].Y--;
                            break;
                        case "down":
                            bike[i].Y++;
                            break;
                    }


                    if (bike[i].X < 0)
                    {
                        bike[i].X = maxWidth;
                    }
                    if (bike[i].X > maxWidth)
                    {
                        bike[i].X = 0;
                    }
                    if (bike[i].Y < 0)
                    {
                        bike[i].Y = maxHeight;
                    }
                    if (bike[i].Y > maxHeight)
                    {
                        bike[i].Y = 0;
                    }

                    if (bike[i].X==Power.X && bike[i].Y == Power.Y) 
                    {
                        TakePower();
                    }

                    for (int j = 1; j < bike.Count; j++)
                    {
                        if (bike[i].X == bike[j].X && bike[i].Y == bike[j].Y)
                        {
                            StopGame();
                        }
                    }
                }
                else
                {
                    bike[i].X = bike[i-1].X;
                    bike[i].Y = bike[i-1].Y;
                }
            }

            GameCanvas.Invalidate();
        }

        private void UpdatePictureBox(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush bikeColour;

            for (int i = 0; i < bike.Count; i++)
            {
                if (i == 0)
                {
                    bikeColour = Brushes.Black;
                }
                else
                {
                    bikeColour = Brushes.DarkGreen;
                }

                canvas.FillEllipse(bikeColour, new Rectangle
                    (
                    bike[i].X * Settings.Width,
                    bike[i].Y * Settings.Height,
                    Settings.Width, Settings.Height
                    ));
            }


            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
            (
            Power.X * Settings.Width,
            Power.Y * Settings.Height,
            Settings.Width, Settings.Height
            ));
        }

        private void RestartGame()
        {
            maxWidth = GameCanvas.Width / Settings.Width - 1; 
            maxHeight = GameCanvas.Height / Settings.Height - 1;
            

            bike.Clear();

            StartBtn.Enabled = false;

            score = 0;
            Scoretxt.Text = "Score: " + score;

            Bike head = new Bike { X = 10, Y = 5};
            bike.Add(head); //head of the bike movement

            for (int i = 0; i < 10; i++) 
            {
                Bike body = new Bike();
                bike.Add(body);
            }

            Power = new Bike { X = random.Next(3, maxWidth), Y = random.Next(3, maxHeight) };

            GameTime.Start();
        }

        private void TakePower()
        {
            Power = new Bike { X = random.Next(3, maxWidth), Y = random.Next(3, maxHeight) };
        }

        private void StopGame()
        {
            GameTime.Stop();
            StartBtn.Enabled=true;


        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
