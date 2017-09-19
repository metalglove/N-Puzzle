using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sliding_Puzzle.Classes.Solvers
{
    
    public class Node
    {
        public List<int> PuzzleState { get; set; }//The "PuzzleState", the puzzlepieces are represented as numbers in a list
        public int Length { get; set; }//The length of the path from the start node to this node
        public int Distance { get; set; }//The "Distance" from this node to the solution
        public int Value { get; set; }//The value of this node (lower the better). Value = Length + Distance (also known as the cost to travel)
        public Direction Direction { get; set; }
        public Node ParentNode { get; set; }
        public bool IsMoveable { get; set; }
    }
    public class Astar
    {
        #region Properties
        public List<Node> openList = new List<Node>();
        public List<Node> closedList = new List<Node>();
        private List<int> _PuzzleStartState;
        public List<int> PuzzleStartState { get { return _PuzzleStartState; } private set { _PuzzleStartState = value; StartingNode = new Node() { PuzzleState = _PuzzleStartState, Direction = Direction.None }; } }
        public List<int> PuzzleEndState { get; private set; } = new List<int>();
        private List<int[,]> CorrectPuzzlePieceLocations { get; set; } = new List<int[,]>();
        public Node StartingNode { get; set; }
        public Node EndingNode { get; set; }
        private int MoveablePiece { get; set; }
        private int PuzzleSize { get; set; }
        public List<int> LeftList { get; set; } = new List<int>();
        public List<int> RightList { get; set; } = new List<int>();
        public List<int> UpList { get; set; } = new List<int>();
        public List<int> DownList { get; set; } = new List<int>();
        #endregion Properties

        #region Constructors
        public Astar(List<int> StartingState, int Puzzlesize)
        {
            PuzzleSize = Puzzlesize;
            PuzzleStartState = StartingState;
            PuzzleEndState = CalculateEndState();
            MoveablePiece = PuzzleEndState.Last();
            CalculatePuzzleBounds();
            CalculateCorrectPuzzlePieceLocations();
        }
        #endregion Constructors

        #region PuzzleSetup
        private void CalculateCorrectPuzzlePieceLocations()
        {
            List<int[,]> MultiDimensionalList = new List<int[,]>();
            for (int i = 0; i < PuzzleSize; i++)
            {
                for (int j = 0; j < PuzzleSize; j++)
                {
                    MultiDimensionalList.Add(new int[,] { { i }, { j } });
                }
            }
            CorrectPuzzlePieceLocations = MultiDimensionalList;
        }
        private void CalculatePuzzleBounds()
        {
            for (int i = PuzzleSize - 1; i < PuzzleEndState.Count; i += PuzzleSize)
            {
                RightList.Add(i);
            }
            for (int i = 0; i < PuzzleEndState.Count; i += PuzzleSize)
            {
                LeftList.Add(i);
            }
            for (int i = 0; i < PuzzleSize; i++)
            {
                UpList.Add(i);
            }
            for (int i = PuzzleEndState.Count - PuzzleSize; i < PuzzleEndState.Count; i++)
            {
                DownList.Add(i);
            }
        }
        private List<int> CalculateEndState()
        {
            List<int> NewPuzzleEndState = new List<int>();
            for (int i = 0; i < (PuzzleSize * PuzzleSize); i++)
            {
                NewPuzzleEndState.Add(i);
            }
            return NewPuzzleEndState;
        }
        #endregion PuzzleSetup

        public List<Direction> FindPath()
        {
            List<Direction> Moves = new List<Direction>();
            Searcher(StartingNode);
            Node node = EndingNode;
            while (node.ParentNode != null)
            {
                Moves.Add(node.Direction);
                node = node.ParentNode;
            }
            Moves.Reverse();
            return Moves;
        }
        private void Searcher(Node currentNode)
        {
            openList.Add(currentNode);
            while (EndingNode == null)
            {
                int LowestValue = openList.Min(item => item.Value);
                Node BestValueNode = openList.FindAll(node => node.Value.Equals(LowestValue)).First();
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
        private List<Node> GetPossibleNodes(Node fromNode)
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

        #region Moves
        private Node Left(Node fromNode)
        {
            Node LeftOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Left, Length = fromNode.Length + 1 };
            List<int> CurrentPuzzleState = new List<int>();
            CurrentPuzzleState = fromNode.PuzzleState.ToList();

            int Location = 0;
            int WhitePuzzlePieceLocation = new int();
            int LeftOfWhitePuzzlePieceLocation = new int();
            foreach (int item in CurrentPuzzleState)
            {
                if (item == MoveablePiece)
                {
                    WhitePuzzlePieceLocation = Location;
                    LeftOfWhitePuzzlePieceLocation = Location - 1;
                }
                Location++;
            }
            if (!LeftList.Contains(WhitePuzzlePieceLocation))
            {
                CurrentPuzzleState = SwapLocation(CurrentPuzzleState, WhitePuzzlePieceLocation, LeftOfWhitePuzzlePieceLocation);
                LeftOfNode = SetNodeInfo(LeftOfNode, CurrentPuzzleState);
                //Debug.WriteLine("Left");
            }
            else
            {
                LeftOfNode.IsMoveable = false;
            }
            if (CheckCompletion(CurrentPuzzleState) == true)
            {
                LeftOfNode.Direction = Direction.Left;
                EndingNode = LeftOfNode;
            }
            return LeftOfNode;
        }
        private Node Right(Node fromNode)
        {
            Node RightOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Right, Length = fromNode.Length + 1 };
            List<int> CurrentPuzzleState = fromNode.PuzzleState.ToList();

            int Location = 0;
            int WhitePuzzlePieceLocation = new int();
            int RightOfWhitePuzzlePieceLocation = new int();
            foreach (int item in CurrentPuzzleState)
            {
                if (item == MoveablePiece)
                {
                    WhitePuzzlePieceLocation = Location;
                    RightOfWhitePuzzlePieceLocation = Location + 1;
                }
                Location++;
            }
            if (!RightList.Contains(WhitePuzzlePieceLocation))
            {
                CurrentPuzzleState = SwapLocation(CurrentPuzzleState, WhitePuzzlePieceLocation, RightOfWhitePuzzlePieceLocation);
                RightOfNode = SetNodeInfo(RightOfNode, CurrentPuzzleState);
                // Debug.WriteLine("Right");
            }
            else
            {
                RightOfNode.IsMoveable = false;
            }
            if (CheckCompletion(CurrentPuzzleState) == true)
            {
                RightOfNode.Direction = Direction.Right;
                EndingNode = RightOfNode;
            }
            return RightOfNode;
        }
        private Node Up(Node fromNode)
        {
            Node UpOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Up, Length = fromNode.Length + 1 };
            List<int> CurrentPuzzleState = fromNode.PuzzleState.ToList();

            int Location = 0;
            int WhitePuzzlePieceLocation = new int();
            int UpOfWhitePuzzlePieceLocation = new int();
            foreach (int item in CurrentPuzzleState)
            {
                if (item == MoveablePiece)
                {
                    WhitePuzzlePieceLocation = Location;
                    UpOfWhitePuzzlePieceLocation = Location - PuzzleSize;
                }
                Location++;
            }
            if (!UpList.Contains(WhitePuzzlePieceLocation))
            {
                CurrentPuzzleState = SwapLocation(CurrentPuzzleState, WhitePuzzlePieceLocation, UpOfWhitePuzzlePieceLocation);
                UpOfNode = SetNodeInfo(UpOfNode, CurrentPuzzleState);
                // Debug.WriteLine("Up");
            }
            else
            {
                UpOfNode.IsMoveable = false;
            }
            if (CheckCompletion(CurrentPuzzleState) == true)
            {
                UpOfNode.Direction = Direction.Up;
                EndingNode = UpOfNode;
            }
            return UpOfNode;
        }
        private Node Down(Node fromNode)
        {
            Node DownOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Down, Length = fromNode.Length + 1 };
            List<int> CurrentPuzzleState = fromNode.PuzzleState.ToList();

            int Location = 0;
            int WhitePuzzlePieceLocation = new int();
            int DownOfWhitePuzzlePieceLocation = new int();
            foreach (int item in CurrentPuzzleState)
            {
                if (item == MoveablePiece)
                {
                    WhitePuzzlePieceLocation = Location;
                    DownOfWhitePuzzlePieceLocation = Location + PuzzleSize;
                }
                Location++;
            }
            if (!DownList.Contains(WhitePuzzlePieceLocation))
            {
                CurrentPuzzleState = SwapLocation(CurrentPuzzleState, WhitePuzzlePieceLocation, DownOfWhitePuzzlePieceLocation);
                DownOfNode = SetNodeInfo(DownOfNode, CurrentPuzzleState);
                //Debug.WriteLine("Down");
            }
            else
            {
                DownOfNode.IsMoveable = false;
            }
            if (CheckCompletion(CurrentPuzzleState) == true)
            {
                DownOfNode.Direction = Direction.Down;
                EndingNode = DownOfNode;
            }
            return DownOfNode;
        }
        #endregion Moves

        #region Events
        private Node SetNodeInfo(Node CurrentNode, List<int> CurrentPuzzleState)
        {
            CurrentNode.Distance = CalculateDistance(CurrentPuzzleState);
            CurrentNode.Value = CurrentNode.Distance + CurrentNode.Length;
            CurrentNode.PuzzleState = CurrentPuzzleState;
            CurrentNode.IsMoveable = true;
            return CurrentNode;
        }
        private List<int> SwapLocation(List<int> CurrentPuzzleState, int WhitePuzzlePieceLocation, int LRUDWhitePuzzlePieceLocation)
        {
            int SwapValue1 = CurrentPuzzleState[WhitePuzzlePieceLocation];
            int SwapValue2 = CurrentPuzzleState[LRUDWhitePuzzlePieceLocation];
            CurrentPuzzleState[WhitePuzzlePieceLocation] = SwapValue2;
            CurrentPuzzleState[LRUDWhitePuzzlePieceLocation] = SwapValue1;
            return CurrentPuzzleState;
        }
        private int CalculateDistance(List<int> currentPuzzleState)
        {
            int CalculatedDistanceFromSolution = 0;
            for (int currentNumberInList = 0; currentNumberInList < currentPuzzleState.Count; currentNumberInList++)
            {
                int Number = currentPuzzleState[currentNumberInList];

                int[,] NumberLocation = CorrectPuzzlePieceLocations[currentNumberInList];
                int[,] CorrectNumberLocation = CorrectPuzzlePieceLocations[Number];
                int FirstNumber = 0;
                int SecondNumber = 0;
                if (CorrectNumberLocation[0, 0] > NumberLocation[0, 0])
                {
                    FirstNumber = CorrectNumberLocation[0, 0] - NumberLocation[0, 0];
                }
                else
                {
                    FirstNumber = NumberLocation[0, 0] - CorrectNumberLocation[0, 0];
                }
                if (CorrectNumberLocation[1, 0] > NumberLocation[1, 0])
                {
                    SecondNumber = CorrectNumberLocation[1, 0] - NumberLocation[1, 0];
                }
                else
                {
                    SecondNumber = NumberLocation[1, 0] - CorrectNumberLocation[1, 0];
                }
                CalculatedDistanceFromSolution += FirstNumber + SecondNumber;
                if ((FirstNumber + SecondNumber) > 0)
                {
                    CalculatedDistanceFromSolution++;
                }
            }
            return CalculatedDistanceFromSolution;
        }
        private bool CheckCompletion(List<int> list)
        {
            bool returnVal = false;
            if (list.SequenceEqual(PuzzleEndState))
            {
                returnVal = true;
            }
            return returnVal;
        }
        #endregion Events
    }
}
