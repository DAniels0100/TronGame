using System.Collections.Generic;

namespace TronFinal
{
    public partial class Form1 : Form
    {
        private LinkedList<Bike> snake = new LinkedList<Bike>(); // Using LinkedList instead of List
        private Bike power = new Bike();

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
            // Setting directions
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

            MoveSnake();
            CheckCollisions();

            GameCanvas.Invalidate();
        }

        private void MoveSnake()
        {
            // Get the current head of the snake
            var head = snake.First.Value;
            Bike newHead = new Bike { X = head.X, Y = head.Y };

            // Move the head in the current direction
            switch (Settings.Directions)
            {
                case "left":
                    newHead.X--;
                    break;
                case "right":
                    newHead.X++;
                    break;
                case "up":
                    newHead.Y--;
                    break;
                case "down":
                    newHead.Y++;
                    break;
            }

            WrapAroundEdges(newHead);

            // Add new head to the snake
            snake.AddFirst(newHead);

            // Remove the tail of the snake
            snake.RemoveLast();
        }

        private void WrapAroundEdges(Bike head)
        {
            // Handle edge wrapping logic
            if (head.X < 0) head.X = maxWidth;
            if (head.X > maxWidth) head.X = 0;
            if (head.Y < 0) head.Y = maxHeight;
            if (head.Y > maxHeight) head.Y = 0;
        }

        private void CheckCollisions()
        {
            var head = snake.First.Value;

            // Check for collisions with power-up
            if (head.X == power.X && head.Y == power.Y)
            {
                TakePower();
            }

            // Check for self-collision
            var current = snake.First.Next; // Start checking from the second node
            while (current != null)
            {
                if (head.X == current.Value.X && head.Y == current.Value.Y)
                {
                    StopGame();
                    break;
                }
                current = current.Next;
            }
        }

        private void UpdatePictureBox(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush bikeColour;

            foreach (var segment in snake)
            {
                if (segment == snake.First.Value)
                {
                    bikeColour = Brushes.Black; // Head of the snake
                }
                else
                {
                    bikeColour = Brushes.DarkGreen; // Body of the snake
                }

                canvas.FillEllipse(bikeColour, new Rectangle
                (
                    segment.X * Settings.Width,
                    segment.Y * Settings.Height,
                    Settings.Width, Settings.Height
                ));
            }

            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
            (
                power.X * Settings.Width,
                power.Y * Settings.Height,
                Settings.Width, Settings.Height
            ));
        }

        private void RestartGame()
        {
            maxWidth = GameCanvas.Width / Settings.Width - 1;
            maxHeight = GameCanvas.Height / Settings.Height - 1;

            snake.Clear();

            StartBtn.Enabled = false;

            score = 0;
            Scoretxt.Text = "Score: " + score;

            Bike head = new Bike { X = 10, Y = 5 };
            snake.AddFirst(head); // Initialize the head of the snake

            for (int i = 0; i < 10; i++)
            {
                Bike body = new Bike();
                snake.AddLast(body); // Add the initial body parts of the snake
            }

            power = new Bike { X = random.Next(3, maxWidth), Y = random.Next(3, maxHeight) };

            GameTime.Start();
        }

        private void TakePower()
        {
            // Spawn a new power-up and grow the snake
            power = new Bike { X = random.Next(3, maxWidth), Y = random.Next(3, maxHeight) };

            // Grow the snake by adding a new segment to the tail
            var tail = snake.Last.Value;
            Bike newTail = new Bike { X = tail.X, Y = tail.Y };
            snake.AddLast(newTail);

            score++;
            Scoretxt.Text = "Score: " + score;
        }

        private void StopGame()
        {
            GameTime.Stop();
            StartBtn.Enabled = true;
        }
    }
}
