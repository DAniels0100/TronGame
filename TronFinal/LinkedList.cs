using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronFinal
{
    public class LinkedList<T>
    {
        private Node<T> head; // Head of the list
        private Node<T> tail; // Tail of the list

        public Node<T> First => head; // Get the first node
        public Node<T> Last => tail; // Get the last node

        public LinkedList()
        {
            head = null;
            tail = null;
        }

        // Add a new node at the beginning of the list
        public void AddFirst(T value)
        {
            Node<T> newNode = new Node<T>(value);
            if (head == null)
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                newNode.Next = head;
                head = newNode;
            }
        }

        // Add a new node at the end of the list
        public void AddLast(T value)
        {
            Node<T> newNode = new Node<T>(value);
            if (tail == null)
            {
                head = newNode;
                tail = newNode;
            }
            else
            {
                tail.Next = newNode;
                tail = newNode;
            }
        }

        // Remove the first node from the list
        public void RemoveFirst()
        {
            if (head != null)
            {
                head = head.Next;
                if (head == null)
                {
                    tail = null;
                }
            }
        }

        // Remove the last node from the list
        public void RemoveLast()
        {
            if (head == null) return;

            if (head.Next == null)
            {
                head = null;
                tail = null;
                return;
            }

            Node<T> current = head;
            while (current.Next != tail)
            {
                current = current.Next;
            }
            current.Next = null;
            tail = current;
        }

        // Check if the list contains a specific value
        public bool Contains(T value)
        {
            Node<T> current = head;
            while (current != null)
            {
                if (current.Value.Equals(value))
                    return true;
                current = current.Next;
            }
            return false;
        }

        // Get the count of nodes in the list
        public int Count()
        {
            int count = 0;
            Node<T> current = head;
            while (current != null)
            {
                count++;
                current = current.Next;
            }
            return count;
        }

        // Enumerator for the LinkedList class
        public IEnumerator<T> GetEnumerator()
        {
            Node<T> current = head;
            while (current != null)
            {
                yield return current.Value;
                current = current.Next;
            }
        }
    }
}
