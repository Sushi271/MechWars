using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.Utils
{
    public class BinaryHeap<KeyT, NodeT> : IEnumerable<NodeT>
        where KeyT : IComparable<KeyT>
        where NodeT : class, IHeapNode<KeyT>
    {
        List<NodeT> array;
        int levels;

        public bool Empty { get { return array.Count == 0; } }

        public BinaryHeap()
        {
            array = new List<NodeT>();
            levels = 0;
        }

        // === operations ===

        public void Insert(NodeT node)
        {
            if (array.Count == RequiredCapacity)
                Extend();

            var idx = array.Count;
            array.Add(node);
            node.HeapIndex = idx;
            var parentIdx = Parent(idx);

            while (idx > 0 && Key(parentIdx).CompareTo(node.Key) > 0)
            {
                Swap(idx, parentIdx);
                idx = parentIdx;
                parentIdx = Parent(idx);
            }
        }

        public NodeT Extract()
        {
            return ExtractSpecific(Node(0));
        }

        public NodeT ExtractSpecific(NodeT node)
        {
            if (array.Count == 0) return null;

            int idx = node.HeapIndex;
            node.HeapIndex = -1;
            Assign(idx, Node(Last));
            array.RemoveAt(Last);
            if (MinimumLevels < levels) Shrink();
            
            var leftIdx = LeftChild(idx);
            var rightIdx = RightChild(idx);
            while (Exists(leftIdx) && Key(leftIdx).CompareTo(Key(idx)) < 0 ||
                Exists(rightIdx) && Key(rightIdx).CompareTo(Key(idx)) < 0)
            {
                var smallerIdx = 
                    !Exists(rightIdx) || Key(leftIdx).CompareTo(Key(rightIdx)) <= 0
                    ? leftIdx
                    : rightIdx;

                Swap(idx, smallerIdx);
                idx = smallerIdx;
                leftIdx = LeftChild(idx);
                rightIdx = RightChild(idx);
            }

            return node;
        }

        public void Correct(NodeT node)
        {
            Insert(ExtractSpecific(node));
        }

        public NodeT Minimum { get { return array.Count == 0 ? null : array[0]; } }

        void Swap(int idx1, int idx2)
        {
            var node = array[idx1];
            array[idx1] = array[idx2];
            array[idx2] = node;

            array[idx1].HeapIndex = idx1;
            array[idx2].HeapIndex = idx2;
        }

        // === indexing ===

        int Parent(int idx)
        {
            return (idx - 1) / 2;
        }

        int LeftChild(int idx)
        {
            return 2 * idx + 1;
        }

        int RightChild(int idx)
        {
            return LeftChild(idx) + 1;
        }

        int Last { get { return array.Count - 1; } }

        bool Exists(int idx)
        {
            return 0 <= idx && idx < array.Count;
        }

        NodeT Node(int idx)
        {
            return array[idx];
        }

        KeyT Key(int idx)
        {
            return Node(idx).Key;
        }

        void Assign(int idx, NodeT node)
        {
            array[idx] = node;
            node.HeapIndex = idx;
        }

        // === memory ===

        int RequiredCapacity { get { return (int)Math.Pow(2, levels) - 1; } }
        int MinimumLevels { get { return (int)Math.Ceiling(Math.Log(array.Count + 1, 2)); } }

        void Extend()
        {
            levels++;
            array.Capacity = RequiredCapacity;
        }

        void Shrink()
        {
            levels--;
            array.Capacity = RequiredCapacity;
        }

        public override string ToString()
        {
            if (Empty) return "EMPTY";
            return string.Join("-", (from n in array select n.Key.ToString()).ToArray());
        }

        public IEnumerator<NodeT> GetEnumerator()
        {
            return array.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public interface IHeapNode<KeyT>
        where KeyT : IComparable<KeyT>
    {
        KeyT Key { get; }
        int HeapIndex { get; set; }
    }
}
