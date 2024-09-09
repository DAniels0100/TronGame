using System.Collections.Generic;

namespace TronFinal
{
    public partial class Form1 : Form
    {
        private LinkedList<Bike> player  = new LinkedList<Bike>(); // player
        private List<LinkedList<Bike>> bots = new List<LinkedList<Bike>>(); // Bots list
        private Dictionary<LinkedList<Bike>, string> botsDirections = new Dictionary<LinkedList<Bike>, string>(); // Dictionary to store each random bot's direction
        private Bike power = new Bike();
        
        
        Random random = new Random();

        int playerSpeed = 5;
        int maxWidth;
        int maxHeight;

        int score;
        int highScore;

        

        bool goLeft, goRight, goUp, goDown;

        private System.Windows.Forms.Timer botTimer = new System.Windows.Forms.Timer(); // Timer to change directions of bots snakes
        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer(); // Main game timer 

        public Form1()
        {
            InitializeComponent();
            new Settings();

            // Set up timer for bots
            botTimer.Interval = 3000; 
            botTimer.Tick += ChangeBotsDirection;
            botTimer.Start();

            // Set up main game timer
            gameTimer.Interval = 50; // Set to 100ms for smoother movement
            gameTimer.Tick += TimeEvent;
            gameTimer.Start();
        }

        // identify key press
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

        // identify key release
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

        //start the game
        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        //handle event
        private void TimeEvent(object sender, EventArgs e)
        {
            // Update the direction for the player 
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

            // Move the player 
            handleMovement(player, Settings.Directions);

            // Move all the bots
            foreach (var bot in bots)
            {
                handleMovement(bot, botsDirections[bot]);
            }

            CheckCollisions();
            GameCanvas.Invalidate();
        }

        //handle the player movement
        private void handleMovement(LinkedList<Bike> bike, string direction)
        {
            if (bike.First == null || bike.Last == null) return; // Ensure snake is not null

            var head = bike.First.Value;
            Bike newHead = new Bike { X = head.X, Y = head.Y };

            // Move the head in the current direction
            switch (direction)
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

            // make the bikes go forwards 
            bike.AddFirst(newHead);
            bike.RemoveLast(); 
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
            if (player.First == null) return; // Ensure bike has at least one segment

            var head = player.First.Value;

            // Check for collisions with power-up
            if (head.X == power.X && head.Y == power.Y)
            {
                TakePower();
            }

            // Check for self-collision 
            var current = player.First.Next;
            while (current != null)
            {
                if (head.X == current.Value.X && head.Y == current.Value.Y)
                {
                    StopGame();
                    return;
                }
                current = current.Next;
            }

            // Check collisions with bots
            // Check collisions between player snake and random snakes (bots)
            foreach (var bot in bots.ToList()) // Use ToList() to safely modify the collection during iteration
            {
                var botHead = bot.First?.Value;
                if (botHead == null) continue; // Skip if the snake is empty

                // Check collision between the player snake head and each segment of the random snake
                foreach (var segment in bot)
                {
                    if (head.X == segment.X && head.Y == segment.Y)
                    {
                        StopGame(); // End the game if the player collides with a bot
                        return;
                    }
                }

                // Check for collisions between random snakes themselves
                foreach (var otherBot in bots.ToList())
                {
                    if (otherBot == bot) continue; // Skip checking itself
                    foreach (var otherSegment in otherBot)
                    {
                        if (botHead.X == otherSegment.X && botHead.Y == otherSegment.Y)
                        {
                            bots.Remove(bot); // Remove the snake that collided
                            bots.Remove(otherBot); // Remove the other snake that it collided with
                            return;
                        }
                    }
                }
            }
        }

        private void UpdatePictureBox(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            Brush bikeColour;

            foreach (var segment in player)
            {
                if (segment == player.First.Value)
                {
                    bikeColour = Brushes.Black; // Head of the player's bike
                }
                else
                {
                    bikeColour = Brushes.DarkGreen; // Body of the player's bike
                }

                canvas.FillEllipse(bikeColour, new Rectangle
                (
                    segment.X * Settings.Width,
                    segment.Y * Settings.Height,
                    Settings.Width, Settings.Height
                ));
            }

            // Draw bots
            foreach (var bot in bots)
            {
                foreach (var segment in bot)
                {
                    canvas.FillEllipse(Brushes.Blue, new Rectangle
                    (
                        segment.X * Settings.Width,
                        segment.Y * Settings.Height,
                        Settings.Width, Settings.Height
                    ));
                }
            }

            canvas.FillEllipse(Brushes.DarkRed, new Rectangle
            (
                power.X * Settings.Width,
                power.Y * Settings.Height,
                Settings.Width, Settings.Height
            ));
        }

        //relaunche the game
        private void RestartGame()
        {
            maxWidth = GameCanvas.Width / Settings.Width - 1;
            maxHeight = GameCanvas.Height / Settings.Height - 1;

            player = new LinkedList<Bike>(); // Reset player 
            bots.Clear();
            botsDirections.Clear();

            StartBtn.Enabled = false;

            score = 0;
            Scoretxt.Text = "Score: " + score;

            Bike head = new Bike { X = 10, Y = 5 };
            player.AddFirst(head); // Initialize the player

            for (int i = 0; i < 3; i++)
            {
                Bike body = new Bike();
                player.AddLast(body); // Add the initial body parts of the player
            }

            // Initialize bots
            for (int i = 0; i < 5; i++) // quantity of bots
            {
                LinkedList<Bike> bot = new LinkedList<Bike>();
                Bike randomHead = new Bike { X = random.Next(3, maxWidth), Y = random.Next(3, maxHeight) };
                bot.AddFirst(randomHead);

                for (int j = 0; j < 3; j++) // Initialize each bot with a length of 3
                {
                    Bike body = new Bike { X = randomHead.X, Y = randomHead.Y };
                    bot.AddLast(body);
                }

                bots.Add(bot);

                // Assign a random initial direction to each bot
                string[] directions = { "left", "right", "up", "down" };
                string initialDirection = directions[random.Next(directions.Length)];
                botsDirections[bot] = initialDirection;
            }

            
            //launch the game
            gameTimer.Start();
        }

        private void ChangeBotsDirection(object sender, EventArgs e)
        {
            // Randomly change direction of each bot
            foreach (var bot in bots)
            {
                string[] directions = { "left", "right", "up", "down" };
                string newDirection = directions[random.Next(directions.Length)];
                botsDirections[bot] = newDirection;
            }
        }

        private void TakePower()
        {
            // Spawn a new power-up and grow the snake
            power = new Bike { X = random.Next(10, maxWidth), Y = random.Next(10, maxHeight) };

            // Grow the snake by adding a new segment to the tail
            var tail = player.Last.Value;
            Bike newTail = new Bike { X = tail.X, Y = tail.Y };
            player.AddLast(newTail); player.AddLast(newTail);

            gameTimer.Interval = 200;

            score++;
            Scoretxt.Text = "Score: " + score;
        }

        private void StopGame()
        {
            GameTime.Stop();
            botTimer.Stop();
            StartBtn.Enabled = true;
        }
    }
}
