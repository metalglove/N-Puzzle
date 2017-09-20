using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sliding_Puzzle.Classes.Solvers
{
    public class BFS : SolvingBase
    {
        #region Properties
        private Queue<Node> QueuedNodes = new Queue<Node>();
        private List<Node> DeQueuedNodes = new List<Node>();
        private bool EndingNodeIsReached = false;
        #endregion Propeties

        public BFS(List<int> StartingState, int PuzzleSize) : base(StartingState, PuzzleSize)
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
            QueuedNodes.Enqueue(StartingNode);
            while (EndingNodeIsReached == false)
            {
                //new Task(() => GenerateNodes(QueuedNodes.Dequeue())).Start(); 
                GenerateNodes(QueuedNodes.Dequeue());
                Debug.WriteLine("<--        **** Nodes in Queue: " + QueuedNodes.Count + " Nodes ran: " + DeQueuedNodes.Count + " ****       -->");
            }
        }

        private void GenerateNodes(Node node)
        {
            DeQueuedNodes.Add(node);
            List<Node> possibleNodes = new List<Node>();
            switch (node.Direction)
            {
                case Direction.Left:
                    possibleNodes = new List<Node>() { Left(node), Up(node), Down(node) };
                    break;
                case Direction.Right:
                    possibleNodes = new List<Node>() { Right(node), Up(node), Down(node) };
                    break;
                case Direction.Up:
                    possibleNodes = new List<Node>() { Left(node), Right(node), Up(node) };
                    break;
                case Direction.Down:
                    possibleNodes = new List<Node>() { Left(node), Right(node), Down(node) };
                    break;
                case Direction.None:
                    possibleNodes = new List<Node>() { Left(node), Right(node), Up(node), Down(node) };
                    break;
                default:
                    break;
            }

            possibleNodes.FindAll(state => state.IsMoveable == true).ForEach(item => 
            {
                if (QueuedNodes.DefaultIfEmpty(null).Where(btem => btem.PuzzleState.SequenceEqual(item.PuzzleState)) != null)
                {
                    if (!DeQueuedNodes.Exists(atem => atem.PuzzleState.SequenceEqual(item.PuzzleState)))
                    {
                        QueuedNodes.Enqueue(item);
                    }
                }
            });
        }
    }
}
