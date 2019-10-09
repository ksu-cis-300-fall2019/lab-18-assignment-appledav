﻿/* Dictionary.cs
 * Author: Rod Howell
 * Modified by David Apple
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KansasStateUniversity.TreeViewer2;
using Ksu.Cis300.ImmutableBinaryTrees;

namespace Ksu.Cis300.NameLookup
{
    /// <summary>
    /// A generic dictionary in which keys must implement IComparable.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class Dictionary<TKey, TValue> where TKey : IComparable<TKey>
    {
        /// <summary>
        /// The keys and values in the dictionary.
        /// </summary>
        private BinaryTreeNode<KeyValuePair<TKey, TValue>> _elements = null;


        /// <summary>
        /// Gets a drawing of the underlying binary search tree.
        /// </summary>
        public TreeForm Drawing => new TreeForm(_elements, 100);

        /// <summary>
        /// This method will return the minimum value given in 
        /// the binary search tree
        /// </summary>
        /// <param name="t"> the tree we are finding the minimum value </param>
        /// <param name="min"> the smallest key </param>
        /// <returns></returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> RemoveMinimumKey(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out KeyValuePair<TKey, TValue> min)
        {
            if(t.LeftChild != null)
            {
                BinaryTreeNode<KeyValuePair<TKey, TValue>> newTree = new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, RemoveMinimumKey(t.LeftChild, out min), t.RightChild);

                return newTree;
                    
            }
            else
            {
                min = t.Data;

                return t.RightChild;
            }
        }
        /// <summary>
        /// this will remove the smallest value from the tree
        /// </summary>
        /// <param name="key"> the location where we are removing</param>
        /// <param name="t"> the overall tree</param>
        /// <param name="removed"> will be true if the value is removed</param>
        /// <returns></returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Remove(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t, out bool removed)
        {

            if (t == null)
            {
                removed = false;
            }
            else
            {
                int compareTo = t.Data.Key.CompareTo(key);
                if (compareTo < 0)
                {
                    BinaryTreeNode<KeyValuePair<TKey, TValue>> rightUpdate = Remove(key, t.RightChild, out removed);
                    BinaryTreeNode<KeyValuePair<TKey, TValue>> newTree = new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, rightUpdate);
                    return newTree;
                }
                else if (compareTo > 0)
                {
                    BinaryTreeNode<KeyValuePair<TKey, TValue>> leftUpdate = Remove(key, t.LeftChild, out removed);
                    BinaryTreeNode<KeyValuePair<TKey, TValue>> newTree = new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, leftUpdate, t.RightChild);
                    return newTree;
                }
                else
                {
                    removed = true;
                    if (t.RightChild != null && t.LeftChild != null)
                    {

                        BinaryTreeNode<KeyValuePair<TKey, TValue>> updatedRight = RemoveMinimumKey(t.RightChild, out KeyValuePair<TKey, TValue> min);
                        BinaryTreeNode<KeyValuePair<TKey, TValue>> newTree = new BinaryTreeNode<KeyValuePair<TKey, TValue>>(min, t.LeftChild, updatedRight);
                        return newTree;
                    }
                    if (t.RightChild == null)
                    {
                        return t.LeftChild;
                    }
                    return t.RightChild;

                }
            }


            
            return null;
        }

        /// <summary>
        /// will verify if a key exists, and if it does, 
        /// will remove the key
        /// </summary>
        /// <param name="k"> key to be removed</param>
        /// <returns></returns>
        public bool Remove (TKey k)
        {
            CheckKey(k);
            bool forOut;
            _elements = Remove(k, _elements, out forOut);
            return forOut;
        }


        /// <summary>
        /// Checks to see if the given key is null, and if so, throws an
        /// ArgumentNullException.
        /// </summary>
        /// <param name="key">The key to check.</param>
        private static void CheckKey(TKey key)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// Finds the given key in the given binary search tree.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="t">The binary search tree.</param>
        /// <returns>The node containing key, or null if the key is not found.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Find(TKey key, BinaryTreeNode<KeyValuePair<TKey, TValue>> t)
        {
            if (t == null)
            {
                return null;
            }
            else
            {
                int comp = key.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    return t;
                }
                else if (comp < 0)
                {
                    return Find(key, t.LeftChild);
                }
                else
                {
                    return Find(key, t.RightChild);
                }
            }
        }

        /// <summary>
        /// Builds the binary search tree that results from adding the given key and value to the given tree.
        /// If the tree already contains the given key, throws an ArgumentException.
        /// </summary>
        /// <param name="t">The binary search tree.</param>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        /// <returns>The binary search tree that results from adding k and v to t.</returns>
        private static BinaryTreeNode<KeyValuePair<TKey, TValue>> Add(BinaryTreeNode<KeyValuePair<TKey, TValue>> t, TKey k, TValue v)
        {
            if (t == null)
            {
                return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(new KeyValuePair<TKey, TValue>(k, v), null, null);
            }
            else
            {
                int comp = k.CompareTo(t.Data.Key);
                if (comp == 0)
                {
                    throw new ArgumentException();
                }
                else if (comp < 0)
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, Add(t.LeftChild, k, v), t.RightChild);
                }
                else
                {
                    return new BinaryTreeNode<KeyValuePair<TKey, TValue>>(t.Data, t.LeftChild, Add(t.RightChild, k, v));
                }
            }
        }

        /// <summary>
        /// Tries to get the value associated with the given key.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value associated with k, or the default value if
        /// k is not in the dictionary.</param>
        /// <returns>Whether k was found as a key in the dictionary.</returns>
        public bool TryGetValue(TKey k, out TValue v)
        {
            CheckKey(k);
            BinaryTreeNode<KeyValuePair<TKey, TValue>> p = Find(k, _elements);
            if (p == null)
            {
                v = default(TValue);
                return false;
            }
            else
            {
                v = p.Data.Value;
                return true;
            }
        }

        /// <summary>
        /// Adds the given key with the given associated value.
        /// If the given key is already in the dictionary, throws an
        /// InvalidOperationException.
        /// </summary>
        /// <param name="k">The key.</param>
        /// <param name="v">The value.</param>
        public void Add(TKey k, TValue v)
        {
            CheckKey(k);
            _elements = Add(_elements, k, v);
        }
    }
}
