using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace FortDefenders.PathFinder
{
    public sealed class BHeap<T> where T : INode
    {
        private List<T> list;           //Node's container. You can implement your own dynamic array, if you wish.


        public BHeap()
        {
            this.list = new List<T>();
        }

        /// <summary>
        /// Adding node onto the heap and reordering the heap
        /// The heap could be sorted by calling the Sort method too
        /// when needed
        /// </summary>
        /// <param name="node">node to be added</param>
        public void Push(T node)
        {
            this.list.Add(node);
            Int32 count = this.list.Count;

            this.Sort(count - 1);
        }

        /// <summary>
        /// Sorting list of items
        /// </summary>
        /// <param name="index">item's index in a list</param>
        public void Sort(Int32 index)
        { 
            Int32 ind = index + 1;
            while (ind != 1)
            {
                T child = this.list[ind - 1];
                T parent = this.list[ind / 2 - 1];
                if (child.F <= parent.F)
                {
                    this.list[ind / 2 - 1] = child;
                    this.list[ind - 1] = parent;
                    ind = ind / 2;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Is a node already on the heap?
        /// </summary>
        /// <param name="position">Node's position on the map</param>
        /// <returns>true, if on the heap. Otherwise, false</returns>
        public Boolean IsOnHeap(Point position)
        {
            var result = from target in this.list
                         where target.CellCoordinates == position
                         select target;

            return (result.Count() > 0 ? true : false);
        }

        /// <summary>
        /// With this method user is able to retrieve the node with a minimum possible F-cost
        /// After the node is assigned, the last node of the heap will be moved to the first position on the heap
        /// </summary>
        /// <returns>node</returns>
        public T Pop()
        {
            T node = this.list[0];
            Int32 count = this.list.Count;

            this.list[0] = this.list.Last();
            this.list.RemoveAt(count - 1);
            count = this.list.Count;

            this.ReorderHeap(count);
            return node;
        }

        /// <summary>
        /// Reorders heap, after the node poped off the heap 
        /// </summary>
        /// <param name="count">number of items</param>
        private void ReorderHeap(Int32 count)
        {
            Int32 u = 0;
            Int32 v = 1;

            do
            {
                u = v;
                if (2 * u + 1 <= count)
                {
                    if (this.list[u - 1].F >= this.list[2 * u - 1].F) v = 2 * u;
                    if (this.list[v - 1].F >= this.list[2 * u + 1 - 1].F) v = 2 * u + 1;
                }
                else if (2 * u <= this.list.Count)
                {
                    if (this.list[u - 1].F >= this.list[2 * u - 1].F) v = 2 * u;
                }

                if (u != v)
                {
                    T tmp = this.list[u - 1];
                    this.list[u - 1] = this.list[v - 1];
                    this.list[v - 1] = tmp;
                }
                else
                {
                    break;
                }
            }
            while (true);
        }

        /// <summary>
        /// Current number of items on the heap
        /// </summary>
        /// <returns>number of items</returns>
        public Int32 GetCount()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Clears heap
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// Allows user to retrieve node by its position on the map
        /// </summary>
        /// <param name="position">Node's position</param>
        /// <param name="index">out parameter for pushing back the retrieved node on the heap</param>
        /// <returns>node</returns>
        public T GetItemByPosition(Point position, out Int32 index)
        {
            var result = from target in this.list
                         where target.CellCoordinates == position
                         select target;

            T node = result.ElementAt(0);
            index = this.list.IndexOf(node);

            return node;
        }

        /// <summary>
        /// Pushs the node retrieved earlier back on the heap
        /// </summary>
        /// <param name="node">node to push back</param>
        /// <param name="index">index of the node got with calling the GetItemByPosition-method</param>
        public void ChangeExistingItem(T node, Int32 index)
        {
            Int32 ind = index + 1;
            this.list[index] = node;

            this.Sort(index);
        }
    }
}
