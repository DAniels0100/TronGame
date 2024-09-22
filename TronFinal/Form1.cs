using System.Collections.Generic;
using System.Windows.Forms;

namespace TronFinal
{
    public partial class Form1 : Form
    {
        private LinkedList<Bike> player = new LinkedList<Bike>(); // Player
        private List<LinkedList<Bike>> bots = new List<LinkedList<Bike>>(); // Bots list
        private Dictionary<LinkedList<Bike>, string> botsDirections = new Dictionary<LinkedList<Bike>, string>(); // Bots directions
        private List<Bike> powers = new List<Bike>(); // Lista para almacenar los poderes
        private List<Bike> items = new List<Bike>(); // Lista para almacenar los ítems
        private Dictionary<Bike, string> itemTypes = new Dictionary<Bike, string>(); // Mapa para almacenar los tipos de ítems
        private string currentItemType; // Para almacenar el tipo del ítem actual

        Random random = new Random();


        private Label PowerStackLabel;
        private Stack<string> powerStack = new Stack<string>(); // Stack para almacenar los poderes
        private bool isPowerActive = false; // Para verificar si un poder está activo
        private bool isInvincible = false; // Para verificar si la invencibilidad está activa
        private bool isSpeedBoosted = false; // Para verificar si el aumento de velocidad está activo

        int playerSpeed = 1;
        int maxWidth;
        int maxHeight;
        int gasQuantity; // Cantidad inicial de gas

        private Color[] rainbowColors = new Color[]
        {
        Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet
        };
        private int currentRainbowColorIndex = 0; // Para seguir el índice de colores

        bool goLeft, goRight, goUp, goDown;

        private System.Windows.Forms.Timer botTimer = new System.Windows.Forms.Timer(); // Temporizador para cambiar direcciones de los bots
        private System.Windows.Forms.Timer gameTimer = new System.Windows.Forms.Timer(); // Temporizador principal del juego
        private System.Windows.Forms.Timer powerTimer = new System.Windows.Forms.Timer(); // Temporizador para la duración de los poderes
        private System.Windows.Forms.Timer spawnTimer = new System.Windows.Forms.Timer(); // Temporizador para la aparición de ítems y poderes

        public Form1()
        {
            InitializeComponent();
            new Settings();

            // Configurar temporizador para generar ítems y poderes cada 3 segundos
            spawnTimer.Interval = 6000; // 
            spawnTimer.Tick += SpawnItemOrPower;
            spawnTimer.Start();

            // Configurar otros temporizadores
            botTimer.Interval = 3000;
            botTimer.Tick += ChangeBotsDirection;
            botTimer.Start();

            gameTimer.Interval = 50;
            gameTimer.Tick += TimeEvent;
            gameTimer.Start();

            powerTimer.Interval = 3000;
            powerTimer.Tick += DeactivatePower;

            // Initialize the power stack label
            PowerStackLabel = new Label();
            PowerStackLabel.Location = new Point(500, 32);
            PowerStackLabel.Size = new Size(300, 100);
            PowerStackLabel.Font = new Font("SimSun-ExtB", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            PowerStackLabel.Text = "Powers: ";
            Controls.Add(PowerStackLabel);

            UpdatePowerStackDisplay();
        }

        // Función para generar ítems o poderes de forma gradual
        private void SpawnItemOrPower(object sender, EventArgs e)
        {
            // Alterna entre generar ítems y poderes de forma aleatoria
            if (random.Next(2) == 0)
            {
                GenerateItems();
            }
            else
            {
                TakePower();
            }
        }

        // Identificar la tecla presionada
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
            if (e.KeyCode == Keys.R)
            {
                RotatePowerStack(); // Rotate powers with "R"
            }
        }

        // Identificar la tecla soltada
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left) goLeft = false;
            if (e.KeyCode == Keys.Right) goRight = false;
            if (e.KeyCode == Keys.Up) goUp = false;
            if (e.KeyCode == Keys.Down) goDown = false;
        }

        private void TimeEvent(object sender, EventArgs e)
        {
            if (goLeft) Settings.Directions = "left";
            if (goRight) Settings.Directions = "right";
            if (goDown) Settings.Directions = "down";
            if (goUp) Settings.Directions = "up";

            MoveBikes(player, Settings.Directions);

            foreach (var bot in bots)
            {
                MoveBikes(bot, botsDirections[bot]);
            }

            CheckCollisions();

            gasQuantity -= 1;
            CombustibleLabel.Text = "Combustible: " + (gasQuantity / 10).ToString();
            if (gasQuantity <= 0)
            {
                StopGame();
            }
            GameCanvas.Invalidate();
        }

        private void StartGame(object sender, EventArgs e)
        {
            RestartGame();
        }

        private void RestartGame()
        {
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
            UpdatePowerStackDisplay();
            gameTimer.Start();
        }

        private void StopGame()
        {
            gameTimer.Stop();
            botTimer.Stop();
            StartBtn.Enabled = true;
        }

        private void MoveBikes(LinkedList<Bike> bikeList, string direction)
        {
            if (bikeList.First == null || bikeList.Last == null) return;

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
            if (player.First == null) return;

            var head = player.First.Value;

            if (!isInvincible)
            {
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

                foreach (var bot in bots.ToList())
                {
                    var botHead = bot.First?.Value;
                    if (botHead == null) continue;

                    foreach (var segment in bot)
                    {
                        if (head.X == segment.X && head.Y == segment.Y)
                        {
                            StopGame();
                            return;
                        }
                    }
                    foreach (var playerSegment in player)
                    {
                        if (botHead.CollidesWith(playerSegment))
                        {
                            bots.Remove(bot);
                            return;
                        }
                    }
                    foreach (var otherBot in bots.ToList())
                    {
                        if (otherBot == bot) continue;
                        foreach (var otherSegment in otherBot)
                        {
                            if (botHead.X == otherSegment.X && botHead.Y == otherSegment.Y)
                            {
                                bots.Remove(bot);
                                bots.Remove(otherBot);
                                return;
                            }
                        }
                    }
                }
            }

            foreach (var power in powers.ToList())
            {
                if (head.CollidesWith(power))
                {
                    powers.Remove(power);
                    TakePower();
                    UpdatePowerStackDisplay();
                }
            }

            foreach (var item in items.ToList())
            {
                if (head.CollidesWith(item))
                {
                    ApplyItemEffect(itemTypes[item]);
                    items.Remove(item);
                }
            }
        }

        private void UpdatePictureBox(object sender, PaintEventArgs e)
        {
            Graphics canvas = e.Graphics;
            Brush bikeColour;

            // Dibujar la moto del jugador
            foreach (var segment in player)
            {
                if (isInvincible)
                {
                    bikeColour = new SolidBrush(rainbowColors[currentRainbowColorIndex % rainbowColors.Length]);
                    currentRainbowColorIndex++;
                }
                else
                {
                    bikeColour = segment == player.First.Value ? Brushes.LightSkyBlue : Brushes.Blue;
                }

                canvas.FillRectangle(bikeColour, new Rectangle(segment.X * Settings.Width, segment.Y * Settings.Height, Settings.Width, Settings.Height));
            }

            // Dibujar los bots
            foreach (var bot in bots)
            {
                foreach (var segment in bot)
                {
                    Brush botColour = segment == bot.First.Value ? Brushes.Orange : Brushes.OrangeRed;
                    canvas.FillRectangle(botColour, new Rectangle(segment.X * Settings.Width, segment.Y * Settings.Height, Settings.Width, Settings.Height));
                }
            }

            // Dibujar los poderes (si están en el mapa)
            foreach (var power in powers)
            {
                canvas.FillEllipse(Brushes.White, new Rectangle(power.X * Settings.Width, power.Y * Settings.Height, Settings.Width, Settings.Height));
            }

            // Dibujar los ítems con colores diferentes según su tipo
            foreach (var item in items)
            {
                Brush itemColour;

                // Verificar el tipo de ítem y asignar el color correspondiente
                switch (itemTypes[item])
                {
                    case "length":
                        itemColour = Brushes.Green; // Color verde para "length"
                        break;
                    case "gas":
                        itemColour = Brushes.Purple; // Color amarillo para "gas"
                        break;
                    case "bomb":
                        itemColour = Brushes.Red; // Color rojo para "bomb"
                        break;
                    default:
                        itemColour = Brushes.Gray; // Color gris si no se reconoce el tipo
                        break;
                }

                canvas.FillEllipse(itemColour, new Rectangle(item.X * Settings.Width, item.Y * Settings.Height, Settings.Width, Settings.Height));
            }
        }

        private void ApplyItemEffect(string itemType)
        {
            currentItemType = itemType;

            switch (itemType)
            {
                case "length":
                    AddLength();
                    break;
                case "gas":
                    AddGas();
                    break;
                case "bomb":
                    BombEffect();
                    break;
            }
        }

        private void AddLength()
        {
            Bike tail = player.Last.Value;
            Bike newSegment = new Bike(tail.X, tail.Y);
            player.AddLast(newSegment);
            player.AddLast(newSegment);
            player.AddLast(newSegment);
        }

        private void AddGas()
        {
            gasQuantity = 1000;
        }

        private void BombEffect()
        {
            StopGame();
        }

        private void ActivatePower()
        {
            if (powerStack.Count > 0 && !isPowerActive)
            {
                string power = powerStack.Pop();
                isPowerActive = true;

                switch (power)
                {
                    case "speed_boost":
                        ActivateSpeedBoost();
                        break;
                    case "invincible":
                        ActivateInvincibility();
                        break;
                }

                powerTimer.Start();
                UpdatePowerStackDisplay();
            }
        }

        private void TakePower()
        {
            Bike newPower = new Bike(random.Next(10, maxWidth), random.Next(10, maxHeight));
            powers.Add(newPower);

            string[] possiblePowers = { "speed_boost", "invincible" };
            string powerUp = possiblePowers[random.Next(possiblePowers.Length)];
            if (powerStack.Count < 3)
            {
                powerStack.Push(powerUp);
            }
            else
            { 
                powerStack.Pop();
                powerStack.Push(powerUp);
            }
            
        }

        private void GenerateItems()
        {
            Bike newItem = new Bike(random.Next(1, maxWidth), random.Next(1, maxHeight));
            items.Add(newItem);

            string[] itemTypesArray = { "length", "gas", "bomb" };
            string itemType = itemTypesArray[random.Next(itemTypesArray.Length)];
            itemTypes[newItem] = itemType;
        }

        private void ActivateSpeedBoost()
        {
            gameTimer.Interval=20;
            isSpeedBoosted = true;
        }

        private void ActivateInvincibility()
        {
            isInvincible = true;
        }

        private void DeactivatePower(object sender, EventArgs e)
        {
            isPowerActive = false;
            isInvincible = false;
            isSpeedBoosted = false;
            gameTimer.Interval=50;
            powerTimer.Stop();
        }
        private void UpdatePowerStackDisplay()
        {
            PowerStackLabel.Text = "Powers: ";

            foreach (var power in powerStack)
            {
                PowerStackLabel.Text += power + " ";
            }
        }
        private void RotatePowerStack()
        {
            if (powerStack.Count > 1)
            {
                string topPower = powerStack.Pop();
                var tempQueue = new Queue<string>(powerStack);
                tempQueue.Enqueue(topPower);
                powerStack = new Stack<string>(tempQueue.Reverse());

                UpdatePowerStackDisplay();
            }
        }
        private void ApplyTopPower()
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

                UpdatePowerStackDisplay(); // Update stack after applying the power
            }
        }
    }

}
