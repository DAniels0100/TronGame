using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronFinal
{
    public class Item
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Type { get; set; } // e.g., "speed", "length", etc.

        public Item(int x, int y, string type)
        {
            X = x;
            Y = y;
            Type = type;
        }

        // Check for collision with the player
        public bool CollidesWith(Bike playerHead)
        {
            return this.X == playerHead.X && this.Y == playerHead.Y;
        }
    }
}
