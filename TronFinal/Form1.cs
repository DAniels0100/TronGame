using System.Collections.Generic;

namespace TronFinal
{
    public partial class Form1 : Form
    {
        private LinkedList<Bike> player = new LinkedList<Bike>(); // Player
        private List<LinkedList<Bike>> bots = new List<LinkedList<Bike>>(); // Bots list
        private Dictionary<LinkedList<Bike>, string> botsDirections = new Dictionary<LinkedList<Bike>, string>(); // Bots directions
        private Bike power = new Bike();

        Random random = new Random();

        private Stack<string> powerStack = new Stack<string>(); // Stack to store power-ups
        private bool isPowerActive = false; // To track if a power-up is currently active
        private bool isInvincible = false; // To track if invincibility is active
        private bool isSpeedBoosted = false; // To track if Speed Boost is active

        int playerSpeed = 5;
        int maxWidth;
        int maxHeight;

        private Color[] rainbowColors = new Color[]
        {
            Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet
        };
        private int currentRainbowColorIndex = 0; // Track the current color index

        bool goLeft, goRight, goUp, goDown;

        private System.Windows.Forms.Timer botTimer = new System.Windows.Forms.Timer(); // Timer to change directions of bots
        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer(); // Main game timer 
        private System.Windows.Forms.Timer powerTimer = new System.Windows.Forms.Timer(); // Timer for power duration
         


        public Form1()
        {
            InitializeComponent();
            new Settings();

            // Set up timer for bots
            botTimer.Interval = 3000;
            botTimer.Tick += ChangeBotsDirection;
            botTimer.Start();

            // Set up main game timer
            gameTimer.Interval = 50; 
            gameTimer.Tick += TimeEvent;
            gameTimer.Start();

            powerTimer.Interval = 3000;
            powerTimer.Tick += DeactivatePower; // Define what happens when the timer ends
        }

        // Identify key press
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
            if (e.KeyCode == Keys.Space)
            {
                ActivatePower();
            }
        }

        // Identify key release
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) goLeft = false;
            if (e.KeyCode == Keys.Right) goRight = false;
            if (e.KeyCode == Keys.Up) goUp = false;
            if (e.KeyCode == Keys.Down) goDown = false;
        }

        // Start the game
        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        // Handle event
        private void TimeEvent(object sender, EventArgs e)
        {
            // Update the direction for the player
            if (goLeft) Settings.Directions = "left";
            if (goRight) Settings.Directions = "right";
            if (goDown) Settings.Directions = "down";
            if (goUp) Settings.Directions = "up";

            // Move the player
            MoveBikes(player, Settings.Directions);

            // Move all the bots
            foreach (var bot in bots)
            {
                MoveBikes(bot, botsDirections[bot]);
            }

            CheckCollisions();
            GameCanvas.Invalidate();
        }

        // Handle the player movement
        private void MoveBikes(LinkedList<Bike> bikeList, string direction)
        {
            if (bikeList.First == null || bikeList.Last == null) return; // Ensure the bike is not null

            var head = bikeList.First.Value;
            Bike newHead = new Bike(head.X, head.Y);
            newHead.Move(direction);
            newHead.WrapAroundEdges(maxWidth, maxHeight);

            bikeList.AddFirst(newHead);
            bikeList.RemoveLast();
        }

        private void CheckCollisions()
        {
            if (player.First == null) return; // Ensure bike has at least one segment

            var head = player.First.Value;

            if (!isInvincible)
            {
                // Check for collisions with power-up
                if (head.CollidesWith(power))
                {
                    TakePower();
                }

                // Check for self-collision
                var current = player.First.Next;
                while (current != null)
                {
                    if (head.CollidesWith(current.Value))
                    {
                        StopGame();
                        return;
                    }
                    current = current.Next;
                }

                // Check collisions with bots
                foreach (var bot in bots.ToList())
                {
                    var botHead = bot.First?.Value;
                    if (botHead == null) continue;

                    foreach (var segment in bot)
                    {
                        if (head.CollidesWith(segment))
                        {
                            StopGame();
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
                // Check if the player is invincible to apply rainbow effect
                if (isInvincible && segment == player.First.Value)
                {
                    // Cycle through rainbow colors for the head
                    bikeColour = new SolidBrush(rainbowColors[currentRainbowColorIndex]);

                    // Move to the next color for the next tick
                    currentRainbowColorIndex = (currentRainbowColorIndex + 1) % rainbowColors.Length;
                }
                else
                {
                    // Regular bike colors
                    bikeColour = segment == player.First.Value ? Brushes.Black : Brushes.DarkGreen;
                }

                canvas.FillEllipse(bikeColour, new Rectangle(segment.X * Settings.Width, segment.Y * Settings.Height, Settings.Width, Settings.Height));

            }

            // Draw bots
            foreach (var bot in bots)
            {
                foreach (var segment in bot)
                {
                    canvas.FillEllipse(Brushes.Blue, new Rectangle(segment.X * Settings.Width, segment.Y * Settings.Height, Settings.Width, Settings.Height));
                }
            }

            // Draw the power-up
            canvas.FillEllipse(Brushes.White, new Rectangle(power.X * Settings.Width, power.Y * Settings.Height, Settings.Width, Settings.Height));
        }

        private void RestartGame()
        {
            // create a new grid
            Grid grid = new Grid();
            for (int i = 1; i <= 100; i++) 
            {
                grid.AddNode(i);
            }


            maxWidth = GameCanvas.Width / Settings.Width - 1;
            maxHeight = GameCanvas.Height / Settings.Height - 1;

            player = new LinkedList<Bike>();
            bots.Clear();
            botsDirections.Clear();

            StartBtn.Enabled = false;



            Bike head = new Bike(10, 5);
            player.AddFirst(head);

            for (int i = 0; i < 3; i++)
            {
                Bike body = new Bike();
                player.AddLast(body);
            }

            for (int i = 0; i < 5; i++)
            {
                LinkedList<Bike> bot = new LinkedList<Bike>();
                Bike randomHead = new Bike(random.Next(3, maxWidth), random.Next(3, maxHeight));
                bot.AddFirst(randomHead);

                for (int j = 0; j < 3; j++)
                {
                    Bike body = new Bike(randomHead.X, randomHead.Y);
                    bot.AddLast(body);
                }

                bots.Add(bot);
                string[] directions = { "left", "right", "up", "down" };
                string initialDirection = directions[random.Next(directions.Length)];
                botsDirections[bot] = initialDirection;
            }

            gameTimer.Start();
        }

        private void ChangeBotsDirection(object sender, EventArgs e)
        {
            foreach (var bot in bots)
            {
                string[] directions = { "left", "right", "up", "down" };
                string newDirection = directions[random.Next(directions.Length)];
                botsDirections[bot] = newDirection;
            }
        }

        private void ActivatePower()
        {
            if (powerStack.Count > 0 && !isPowerActive)
            {
                string powerUp = powerStack.Pop();

                if (powerUp == "speed_boost")
                {
                    gameTimer.Interval = 20; // Apply power-up
                    isPowerActive = true;
                    isSpeedBoosted = true;
                    powerTimer.Start(); // Start the 5-second timer
                }
                else if (powerUp == "invincible")
                {
                    isInvincible = true; // Make the player invincible
                    isPowerActive = true;
                    powerTimer.Start(); // Start the 5-second timer
                }

                isPowerActive = true; // Mark that a power is active
                powerTimer.Start(); // Start the 5-second timer
            }
        }

        // Deactivate the power-up after 5 seconds
        private void DeactivatePower(object sender, EventArgs e)
        {

            // Revert power-up effects after 5 seconds
            if (isInvincible)
            {
                isInvincible = false; // Deactivate invincibility
                currentRainbowColorIndex = 0; // Reset rainbow color index
            }

            if (isSpeedBoosted)
            {
                gameTimer.Interval = 50; // Revert the speed boost
            }

            isPowerActive = false; // Mark power as inactive
            powerTimer.Stop(); // Stop the timer
        }

        private void TakePower()
        {
            power = new Bike(random.Next(10, maxWidth), random.Next(10, maxHeight));

            // Randomly assign a power-up (either speed boost or invincibility)
            string[] possiblePowers = { "speed_boost", "invincible" };
            string powerUp = possiblePowers[random.Next(possiblePowers.Length)];
            powerStack.Push(powerUp);
        }

        private void TakeItem()
        {
            power = new Bike(random.Next(10, maxWidth), random.Next(10, maxHeight));
            var tail = player.Last.Value;
            Bike newTail = new Bike(tail.X, tail.Y);
            player.AddLast(newTail);
            player.AddLast(newTail);
        }

        private void StopGame()
        {
            gameTimer.Stop();
            botTimer.Stop();
            StartBtn.Enabled = true;
        }
    }
}
