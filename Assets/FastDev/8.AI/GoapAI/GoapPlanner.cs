using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FastDev
{
    public class GoapPlanner
    {
        public class GoapNode
        {
            public GoapNode Parent;
            public int Cost;
            public IGoapAction GoapAction;

            public HashSet<KeyValuePair<string, object>> State;

            public GoapNode(GoapNode parent, int cost, HashSet<KeyValuePair<string, object>> state, IGoapAction action)
            {
                this.Parent = parent;
                this.Cost = cost;
                this.State = state;
                this.GoapAction = action;
            }
        }

        public Stack<IGoapAction> Plan(IGoapAgent agent, HashSet<IGoapAction> availableActions, HashSet<KeyValuePair<string, object>> worldState, HashSet<KeyValuePair<string, object>> goal)
        {
            foreach (IGoapAction a in availableActions)
            {
                a.Reset();
            }

            HashSet<IGoapAction> usableActions = new HashSet<IGoapAction>();
            foreach (IGoapAction a in availableActions)
            {
                if (a.CheckProceduralPrecondition(agent))
                    usableActions.Add(a);
            }

            List<GoapNode> findNodes = new List<GoapNode>();

            GoapNode start = new GoapNode(null, 0, worldState, null);
            bool success = BuildGraph(start, findNodes, usableActions, goal);

            if (!success)
            {
                return null;
            }

            GoapNode cheapest = null;
            foreach (var leaf in findNodes)
            {
                if (cheapest == null)
                    cheapest = leaf;
                else
                {
                    if (leaf.Cost < cheapest.Cost)
                        cheapest = leaf;
                }
            }

            Stack<IGoapAction> stack = new Stack<IGoapAction>();

            string debug = "";
            GoapNode curNode = cheapest;

            while (curNode != null)
            {
                if (curNode.GoapAction != null)
                {
                    stack.Push(curNode.GoapAction);

                    debug += "<--" + curNode.GoapAction.ToString();
                }
                curNode = curNode.Parent;
            }

            Debug.Log("GoapPlan:" + debug);
            return stack;
        }


        private bool BuildGraph(GoapNode parent, List<GoapNode> findNodes, HashSet<IGoapAction> availableActions, HashSet<KeyValuePair<string, object>> goal)
        {
            bool foundOne = false;
            foreach (var action in availableActions)
            {
                //����ǰ��ִ������
                if (Contains(parent.State, action.Preconditions))
                {
                    HashSet<KeyValuePair<string, object>> currentState = Combines(parent.State, action.Effects);

                    GoapNode goapNode = new GoapNode(parent, parent.Cost + action.Cost, currentState, action);

                    //��״̬����Ŀ��״̬
                    if (Contains(currentState, goal))
                    {
                        findNodes.Add(goapNode);
                        foundOne = true;
                    }
                    else
                    {
                        HashSet<IGoapAction> subset = ActionSubset(availableActions, action);
                        bool found = BuildGraph(goapNode, findNodes, subset, goal);
                        if (found)
                            foundOne = true;
                    }
                }
            }
            return foundOne;
        }


        /// <summary>
        /// a ���� b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private bool Contains(HashSet<KeyValuePair<string, object>> a, HashSet<KeyValuePair<string, object>> b)
        {
            foreach (var v in b)
            {
                bool result = false;

                foreach (var vv in a)
                {
                    if (vv.Equals(v))
                    {
                        result = true;
                        break;
                    }
                }

                if (result == false)
                {
                    return false;
                }
            }

            return true;
        }

        private HashSet<KeyValuePair<string, object>> Combines(HashSet<KeyValuePair<string, object>> a, HashSet<KeyValuePair<string, object>> b)
        {
            HashSet<KeyValuePair<string, object>> state = new HashSet<KeyValuePair<string, object>>();

            foreach (KeyValuePair<string, object> v in a)
            {
                state.Add(new KeyValuePair<string, object>(v.Key, v.Value));
            }

            foreach (var v in b)
            {
                bool exists = false;
                foreach (var vv in state)
                {
                    if (vv.Key == v.Key)
                    {
                        exists = true;
                        break;
                    }
                }

                if (exists)
                {
                    state.RemoveWhere((KeyValuePair<string, object> kvp) => { return kvp.Key.Equals(v.Key); });
                    KeyValuePair<string, object> updated = new KeyValuePair<string, object>(v.Key, v.Value);
                    state.Add(updated);
                }
                else
                {
                    state.Add(new KeyValuePair<string, object>(v.Key, v.Value));
                }
            }

            return state;
        }

        private HashSet<IGoapAction> ActionSubset(HashSet<IGoapAction> actions, IGoapAction removeAction)
        {
            HashSet<IGoapAction> subset = new HashSet<IGoapAction>();
            foreach (IGoapAction a in actions)
            {
                if (!a.Equals(removeAction))
                    subset.Add(a);
            }
            return subset;
        }
    }
}