using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TronFinal
{
    public class Grid
    {
            internal NodeGrid head;
            internal NodeGrid lastRowStart;
            internal int count = 0;

            public void AddNode(int data)
            {
                NodeGrid node = new NodeGrid(data);

                if (this.head == null)
                {
                    // Initialize the grid with the first node
                    this.head = node;
                    this.lastRowStart = node;
                }
                else
                {
                    // Find the last node in the current row
                    NodeGrid current = lastRowStart;
                    while (current.Next != null)
                    {
                        current = current.Next;
                    }

                    // Add the new node to the row
                    current.Next = node;
                    node.Previous = current;

                    // Link vertically if not in the first row
                    if (count >= 10)
                    {
                        NodeGrid nodeAbove = lastRowStart;
                        for (int i = 0; i < count % 10; i++)
                        {
                            nodeAbove = nodeAbove.Next;
                        }

                        node.Up = nodeAbove;
                        nodeAbove.Down = node;
                    }

                    // Update lastRowStart when a new row is started
                    if (count % 10 == 9)
                    {
                        lastRowStart = node; // This will be the start of the new row
                    }
                }

                count++;
            }
        }
}
