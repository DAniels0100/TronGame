using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronFinal
{
    public class Bike
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Image Image { get; set; }

        public Bike(int x = 0, int y = 0)
        {
            X = x;
            Y = y;
        }

        // Moves the bike in the specified direction
        public void Move(string direction)
        {
            switch (direction)
            {
                case "left":
                    X--;
                    break;
                case "right":
                    X++;
                    break;
                case "up":
                    Y--;
                    break;
                case "down":
                    Y++;
                    break;
            }
        }

        // Handles wrapping the bike around the edges of the game area
        public void WrapAroundEdges(int maxWidth, int maxHeight)
        {
            if (X < 0) X = maxWidth;
            if (X > maxWidth) X = 0;
            if (Y < 0) Y = maxHeight;
            if (Y > maxHeight) Y = 0;
        }

        // Checks if the bike collides with another bike
        public bool CollidesWith(Bike other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
    }
}
