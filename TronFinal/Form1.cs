using System.Collections.Generic;
using System.Windows.Forms;

namespace TronFinal
{
    public partial class Form1 : Form
    {
        private LinkedList<Bike> player = new LinkedList<Bike>(); // Player
        private List<LinkedList<Bike>> bots = new List<LinkedList<Bike>>(); // Bots list
        private Dictionary<LinkedList<Bike>, string> botsDirections = new Dictionary<LinkedList<Bike>, string>(); // Bots directions
        private Bike power = new Bike();
        private Bike item = new Bike();
        private string currentItemType; // To store the type of the current item

        Random random = new Random();

        
        private Stack<string> powerStack = new Stack<string>(); // Stack to store power-ups
        private bool isPowerActive = false; // To track if a power-up is currently active
        private bool isInvincible = false; // To track if invincibility is active
        private bool isSpeedBoosted = false; // To track if Speed Boost is active

        int playerSpeed = 5;
        int maxWidth;
        int maxHeight;
        int gasQuantity = 1000; // Starting gas quantity

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

            gasQuantity -= 1;
            if (gasQuantity <= 0)
            {
                StopGame(); // Stop the game if gas runs out
            }
            GameCanvas.Invalidate();
        }

        // Start the game
        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }
        private void RestartGame()
        {
            // create a new grid
            Grid grid = new Grid();
            for (int i = 1; i <= 100; i++)
            {
                grid.AddNode(i);
            }

            gasQuantity = 1000;
            powerStack.Clear();

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
        private void StopGame()
        {
            gameTimer.Stop();
            botTimer.Stop();
            StartBtn.Enabled = true;
        }

        // Handle event
        

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
        private void ChangeBotsDirection(object sender, EventArgs e)
        {
            foreach (var bot in bots)
            {
                string[] directions = { "left", "right", "up", "down" };
                string newDirection = directions[random.Next(directions.Length)];
                botsDirections[bot] = newDirection;
            }
        }

        private void CheckCollisions()
        {
            if (player.First == null) return; // Ensure player bike has at least one segment

            var head = player.First.Value; // Player's head

            if (!isInvincible)
            {
                // Check for self-collision (player collides with itself)
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
                

                // Check for collisions with bot heads
                foreach (var bot in bots.ToList()) // Use ToList() to modify the collection safely
                {
                    var botHead = bot.First?.Value;
                    if (botHead == null) continue; // Ensure bot has a head

                    foreach (var segment in bot)
                    {
                        if (head.X == segment.X && head.Y == segment.Y)
                        {
                            StopGame(); // End the game if the player collides with a bot
                            return;
                        }
                    }
                    // If bot's head collides with any part of the player, remove the bot
                    foreach (var playerSegment in player)
                    {
                        if (botHead.CollidesWith(playerSegment))
                        {
                            bots.Remove(bot); // Remove the bot from the game
                            return; // Exit the collision check after removing the bot
                        }
                    }

                    // Check for collisions between bots
                    foreach (var otherBot in bots.ToList())
                    {
                        if (otherBot == bot) continue; // Skip self-collision check for the same bot
                        foreach (var otherSegment in otherBot)
                        {
                            if (botHead.X == otherSegment.X && botHead.Y == otherSegment.Y)
                            {
                                bots.Remove(bot); // Remove the bot that collided
                                return;
                            }
                        }
                    }
                }
            }
            // Check for collisions with power-up
            if (head.CollidesWith(power))
            {
                    TakePower();
            }

            // Check for collisions with items
            if (head.CollidesWith(item))
            {
                ApplyItemEffect(currentItemType); // Apply effect based on item type
                GenerateItem(); // Generate a new item
            }
        }
        

        private void UpdatePictureBox(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;

            // Rainbow colors for invincibility
            Color[] rainbowColors = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };
            int colorIndex = 0; // To cycle through the rainbow colors

            Brush bikeColour;

            foreach (var segment in player)
            {
                // Check if the player is invincible to apply rainbow effect
                if (isInvincible)
                {
                    // Cycle through rainbow colors for each segment
                    bikeColour = new SolidBrush(rainbowColors[colorIndex % rainbowColors.Length]);
                    colorIndex++; // Move to the next color in the array
                }
                else
                {
                    // Regular bike colors
                    bikeColour = segment == player.First.Value ? Brushes.Black : Brushes.Blue;
                }

                // Draw the segment as a rectangle
                canvas.FillRectangle(bikeColour, new Rectangle(segment.X * Settings.Width, segment.Y * Settings.Height, Settings.Width, Settings.Height));
            }

            // Draw bots
            foreach (var bot in bots)
            {
                foreach (var segment in bot)
                {
                    canvas.FillRectangle(Brushes.Orange, new Rectangle(segment.X * Settings.Width, segment.Y * Settings.Height, Settings.Width, Settings.Height));
                }
            }

            // Draw the power-up
            canvas.FillEllipse(Brushes.White, new Rectangle(power.X * Settings.Width, power.Y * Settings.Height, Settings.Width, Settings.Height));


            // Draw items
            Brush itemBrush;
            switch (currentItemType)
            {
                case "length":
                    itemBrush = Brushes.Green;
                    break;
                case "gas":
                    itemBrush = Brushes.Blue;
                    break;
                case "bomb":
                    itemBrush = Brushes.Red;
                    break;
                default:
                    itemBrush = Brushes.Gray; // Default case
                    break;
            }

            canvas.FillRectangle(itemBrush, new Rectangle(item.X * Settings.Width, item.Y * Settings.Height, Settings.Width, Settings.Height));
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
            item = new Bike(random.Next(30, maxWidth), random.Next(30, maxHeight));
            var tail = player.Last.Value;
            Bike newTail = new Bike(tail.X, tail.Y);
            player.AddLast(newTail);
            player.AddLast(newTail);
        }

        private void GenerateItem()
        {
            item = new Bike(random.Next(1, maxWidth), random.Next(1, maxHeight));
            string[] itemTypes = { "length", "gas", "bomb" };
            currentItemType = itemTypes[random.Next(itemTypes.Length)];
        }
        private void ApplyItemEffect(string itemType)
        {
            switch (itemType)
            {
                case "length":
                    var tail = player.Last.Value;
                    Bike newTail = new Bike(tail.X, tail.Y);
                    player.AddLast(newTail); // Increase length
                    player.AddLast(newTail);
                    break;
                case "gas":
                    HandleGas();
                    break;
                case "bomb":
                    // Implement bomb effect (e.g., eliminate a bot)
                    HandleBombEffect();
                    break;
            }
        }
        private void HandleBombEffect()
        {
            StopGame();
        }
        private void HandleGas() 
        {
            gasQuantity = 100;
        }

    }
}
