using System;
using System.Collections.Generic;


namespace RBTree
{
    public class Map<KEY, DATA> where KEY : notnull, IComparable<KEY>
    {
        public Node<KEY, DATA> Root;

        public void Insert(KEY key, DATA data)
        {

            if (Root == null)
            {
                Root = new Node<KEY, DATA>(key, data);

                return;
            }

            Node<KEY, DATA> node = new Node<KEY, DATA>(key, data);

            Node<KEY, DATA> curr = Root;

            Node<KEY, DATA> parent = null;

            while (curr != null)
            {
                parent = curr;

                if (node.Key.CompareTo(curr.Key) < 0)
                {
                    curr = curr.Left;
                }
                else if(node.Key.CompareTo(curr.Key) > 0)
                {
                    curr = curr.Right;
                }
                else
                {
                    throw new Exception("Element not found!");
                }
            }

            node.Parent = parent;

            if (parent == null)
            {
                Root = node;
            }
            else if (node.Key.CompareTo(parent.Key) < 0)
            {
                node.Parent.Left = node;
            }
            else if (node.Key.CompareTo(parent.Key) == 0)
            {
                throw new Exception("An element with this key already exists");

            }
            else
            {
                node.Parent.Right = node;
            }

            node.Color = Color.red;

            node.Left = null;

            node.Right = null;

            BalancingAfterInsertion(node);

            Root.Color = Color.black;

            Root.Parent = null;

        }

        private void LTurn(Node<KEY, DATA> node)
        {
            if (node == null)
            {
                return;
            }

            Node<KEY, DATA> x = node.Right;

            if (x == null)
            {
                return;
            }

            node.Right = x.Left;

            if (x.Left != null)
            {
                x.Left.Parent = node;
            }

            if (node.Parent != null)
            {
                x.Parent = node.Parent;

                if (node == node.Parent.Left)
                {
                    node.Parent.Left = x;
                }
                else
                {
                    node.Parent.Right = x;
                }
            }
            else
            {
                Root = x;
            }

            x.Left = node;

            node.Parent = x;

        }

        private void RTurn(Node<KEY, DATA> node)
        {
            if (node == null)
            {
                return;
            }

            Node<KEY, DATA> x = node.Left;

            if (x == null)
            {
                return;
            }

            node.Left = x.Right;

            if (x.Right != null)
            {
                x.Right.Parent = node;
            }

            if (node.Parent != null)
            {
                x.Parent = node.Parent;

                if (node == node.Parent.Right)
                {
                    node.Parent.Right = x;
                }
                else
                {
                    node.Parent.Left = x;
                }
            }
            else
            {
                Root = x;
            }

            x.Right = node;

            node.Parent = x;
        }

        private Node<KEY, DATA> GrandFather(Node<KEY, DATA> node)
        {
            return node.Parent.Parent;
        }

        private Node<KEY, DATA> Uncle(Node<KEY, DATA> node)
        {
            if (GrandFather(node) == null)
            {
                return null;
            }

            return GrandFather(node).Left != node.Parent ? GrandFather(node).Left : GrandFather(node).Right;
        }

        private void BalancingAfterInsertion(Node<KEY, DATA> node)
        {

            if (node.Parent == null || node.Parent == Color.black)
            {
                return;
            }

            if (Uncle(node) == Color.red)
            {
                node.Parent.Color = Color.black;

                Uncle(node).Color = Color.black;

                if (GrandFather(node) != null && GrandFather(node) != Root)
                {
                    GrandFather(node).Color = Color.red;
                }

                BalancingAfterInsertion(GrandFather(node));
            }
            else
            {
                if (GrandFather(node) != null && GrandFather(node).Right == node.Parent)
                {
                    if (node.Parent.Left == node)
                    {
                        RTurn(node.Parent);

                        node.Color = Color.black;

                        node.Parent.Color = Color.red;

                        LTurn(node.Parent);
                    }
                    else
                    {
                        node.Parent.Color = Color.black;

                        GrandFather(node).Color = Color.red;

                        LTurn(GrandFather(node));
                    }
                }
                else
                {
                    if (node.Parent.Right == node)
                    {
                        LTurn(node.Parent);

                        node.Color = Color.black;

                        node.Parent.Color = Color.red;

                        RTurn(node.Parent);
                    }
                    else if (node.Parent != null && GrandFather(node) != null)
                    {
                        node.Parent.Color = Color.black;

                        GrandFather(node).Color = Color.red;

                        RTurn(GrandFather(node));
                    }
                }
            }

        }

        public Node<KEY, DATA> Find(KEY key)
        {
            Node<KEY, DATA> current = Root;

            if (current == null)
            {
                throw new Exception("Array is empty!");
            }

            while (current != null)
            {
                if (key.CompareTo(current.Key) > 0)
                {
                    current = current.Right;
                }
                else if (key.CompareTo(current.Key) < 0)
                {
                    current = current.Left;
                }
                else
                {
                    return current;
                }

            }

            throw new Exception("Item not found!");

        }

        public void Remove(KEY key)
        {
            Node<KEY, DATA> node;

            try
            {
                node = Find(key);
            }
            catch (Exception e)
            {
                throw;
            }

            if (node == Root && node.Right == null && node.Left == null)
            {
                Root = null;

                return;
            }

            if (node.Left != null && node.Right != null)
            {
                Node<KEY, DATA> closestNode = ClosestNode(node);

                Node<KEY, DATA> temp = closestNode;

                closestNode = node;

                node = temp;

                Swap(node, closestNode);

                node.Parent.Key = node.Parent.Key;

            }

            if ((node.Right != null) || (node.Left != null))
            {
                Swap(node, node.Right != null ? node.Right : node.Left);

                node.Left = null;

                node.Right = null;

            }
            else
            {
                RecoveryRemove(node);

                if (node.Parent.Left == node)
                {
                    node.Parent.Left = null;

                }
                else if (node.Parent.Right == node)
                {
                    node.Parent.Right = null;

                }

            }
        }


        private void RecoveryRemove(Node<KEY, DATA> node)
        {
            if (node.Color == Color.red)
            {
                return;
            }

            if (node.Parent == Color.red)
            {
                if (Brother(node)?.Left == Color.black && Brother(node)?.Right == Color.black)
                {
                    node.Parent.Color = Color.black;

                    Brother(node).Color = Color.red;
                }

                else if (Brother(node)?.Left == Color.red && node.Parent.Right == node)
                {
                    node.Parent.Color = Color.black;

                    Brother(node).Color = Color.red;

                    Brother(node).Left.Color = Color.black;

                    RTurn(node.Parent);
                }
                else if (Brother(node)?.Right == Color.red && node.Parent.Left == node)
                {
                    node.Parent.Color = Color.black;

                    Brother(node).Color = Color.red;

                    Brother(node).Right.Color = Color.black;

                    LTurn(node.Parent);
                }

                else if (Brother(node)?.Left == Color.black && Brother(node)?.Right == Color.red && node.Parent.Right == node)
                {
                    node.Parent.Color = Color.black;

                    LTurn(Brother(node));

                    RTurn(node.Parent);
                }
                else if (Brother(node)?.Right == Color.black && Brother(node)?.Left == Color.red && node.Parent.Left == node)
                {
                    node.Parent.Color = Color.black;

                    RTurn(Brother(node));

                    LTurn(node.Parent);
                }
                return;
            }

            if (Brother(node) != null && Brother(node) == Color.red)
            {
                if (Brother(node)?.Right.Left == Color.black && Brother(node)?.Right.Right == Color.black && node.Parent.Right == node)
                {
                    Brother(node).Color = Color.black;

                    Brother(node).Right.Color = Color.red;

                    RTurn(node.Parent);
                }
                else if (Brother(node)?.Left.Right == Color.black && Brother(node)?.Left.Left == Color.black && node.Parent.Left == node)
                {
                    Brother(node).Color = Color.black;

                    Brother(node).Left.Color = Color.red;

                    LTurn(node.Parent);
                }

                if (Brother(node).Right != null && Brother(node).Right.Left == Color.red && node.Parent.Right != null && node.Parent.Right == node)
                {
                    Brother(node).Right.Left.Color = Color.black;

                    LTurn(Brother(node));

                    RTurn(node.Parent);
                }
                else if (Brother(node)?.Left != null && Brother(node)?.Left.Right == Color.red && node.Parent.Left == node)
                {
                    Brother(node).Left.Right.Color = Color.black;

                    RTurn(Brother(node));

                    LTurn(node.Parent);
                }
            }
            else
            {
                if (Brother(node)?.Right == Color.red && node.Parent.Right == node)
                {
                    Brother(node).Right.Color = Color.black;

                    LTurn(Brother(node));

                    RTurn(node.Parent);
                }
                else if (Brother(node)?.Left == Color.red && node.Parent.Left == node)
                {
                    Brother(node).Left.Color = Color.black;

                    RTurn(Brother(node));

                    LTurn(node.Parent);
                }

                else if (Brother(node)?.Left == Color.red && Brother(node)?.Right == Color.black && node.Parent.Right == node)
                {
                    node.Parent.Color = Color.red;

                    RTurn(node.Parent);
                }
                else if (Brother(node)?.Right == Color.red && Brother(node)?.Left == Color.black && node.Parent.Left == node)
                {
                    node.Parent.Color = Color.red;

                    LTurn(node.Parent);
                }

                else if (Brother(node)?.Left == Color.black && Brother(node)?.Right == Color.black)
                {
                    if (Brother(node) != null && node.Parent != null)
                    {
                        Brother(node).Color = Color.red;

                        RecoveryRemove(node.Parent);
                    }
                }
            }
        }

        private Node<KEY, DATA> ClosestNode(Node<KEY, DATA> node)
        {
            Node<KEY, DATA> curr = node.Right;

            while ((curr != null) && (curr.Left != null))
            {
                curr = curr.Left;
            }

            return curr;
        }

        public List<KEY> GetKeys()
        {
            var keys = new List<KEY>();

            AddKey(Root, keys);

            return keys;
        }

        public List<DATA> GetValues()
        {
            var values = new List<DATA>();

            AddValue(Root, values);

            return values;
        }

        private void AddKey(Node<KEY, DATA> node, List<KEY> keys)
        {
            if (node == null)
            {
                return;
            }

            AddKey(node.Left, keys);

            keys.Add(node.Key);

            AddKey(node.Right, keys);
        }

        private void AddValue(Node<KEY, DATA> node, List<DATA> values)
        {
            if (node == null)
            {
                return;
            }

            AddValue(node.Left, values);

            values.Add(node.Data);

            AddValue(node.Right, values);
        }

        private Node<KEY, DATA> Brother(Node<KEY, DATA> node)
        {
            //if(node.Parent != null)
            return node.Parent.Left != node ? node.Parent.Left : node.Parent.Right;

            //return null;

        }

        private void Swap(Node<KEY, DATA> node1, Node<KEY, DATA> node2)
        {
            KEY tempKey = node1.Key;
            node1.Key = node2.Key;
            node2.Key = tempKey;

            DATA tempData = node1.Data;
            node1.Data = node2.Data;
            node2.Data = tempData;

        }

        public void Clear()
        {
            Clear(Root);

        }

        private void Clear(Node<KEY, DATA> node)
        {
            if (node == null)
                return;

            Clear(node.Right);

            node.Right = null;

            node.Left = null;

            Clear(node.Left);

            if (node == Root)
            {
                Root = null;

            }

        }


        public void Print()
        {
            Print(Root);
        }

        private void Print(Node<KEY, DATA> node)
        {
            if (node == null)

                return;

            Print(node.Left);

            Console.WriteLine($" цвет - {node.Color}, ключ - {node.Key}, данные - {node.Data}");

            Print(node.Right);
        }
    }


}