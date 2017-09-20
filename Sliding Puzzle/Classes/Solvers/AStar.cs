using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sliding_Puzzle.Classes.Solvers
{
    public class Astar : SolvingBase
    {
        public List<Node> openList = new List<Node>();
        public List<Node> closedList = new List<Node>();
        public Astar(List<int> StartingState, int Puzzlesize) : base (StartingState, Puzzlesize)
        {
            
        }

        public List<Direction> FindPath()
        {
            List<Direction> Moves = new List<Direction>();
            Search();
            Node node = EndingNode;
            while (node.ParentNode != null)
            {
                Moves.Add(node.Direction);
                node = node.ParentNode;
            }
            Moves.Reverse();
            return Moves;
        }
        private void Search()
        {
            openList.Add(StartingNode);
            while (EndingNode == null)
            {
                int LowestValue = openList.Min(item => item.Value);
                Node BestValueNode = openList.First(node => node.Value.Equals(LowestValue));
                openList.Remove(BestValueNode);
                List<Node> possibleNodes = GetPossibleNodes(BestValueNode);
                closedList.Add(BestValueNode);
                foreach (Node possibleNode in possibleNodes)
                {
                    if (!openList.Exists(item => item.PuzzleState.SequenceEqual(possibleNode.PuzzleState)))
                    {
                        if (!closedList.Exists(item => item.PuzzleState.SequenceEqual(possibleNode.PuzzleState)))
                        {
                            openList.Add(possibleNode);
                        }
                    }
                }
                Debug.WriteLine("openlist = " + openList.Count + "| closedlist = " + closedList.Count + "| bestvaluenode " + BestValueNode.Value);
            }
        }
        public List<Node> GetPossibleNodes(Node fromNode)
        {
            List<Node> possibleNodes = new List<Node>();
            switch (fromNode.Direction)
            {
                case Direction.Left:
                    possibleNodes = new List<Node>() { Left(fromNode), Up(fromNode), Down(fromNode) };
                    break;
                case Direction.Right:
                    possibleNodes = new List<Node>() { Right(fromNode), Up(fromNode), Down(fromNode) };
                    break;
                case Direction.Up:
                    possibleNodes = new List<Node>() { Left(fromNode), Right(fromNode), Up(fromNode) };
                    break;
                case Direction.Down:
                    possibleNodes = new List<Node>() { Left(fromNode), Right(fromNode), Down(fromNode) };
                    break;
                case Direction.None:
                    possibleNodes = new List<Node>() { Left(fromNode), Right(fromNode), Up(fromNode), Down(fromNode) };
                    break;
                default:
                    break;
            }
            possibleNodes.RemoveAll(node => node.IsMoveable == false);
            return possibleNodes;
        }
    }
}
