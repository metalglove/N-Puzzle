﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sliding_Puzzle.Classes.Solvers
{

    public enum NodeState { Untested, Open, Closed, Duplicate }
    public class Node
    {
        public List<int> PuzzleState { get; set; }//The "PuzzleState", the puzzlepieces are represented as numbers in a list
        public int Length { get; set; }//The length of the path from the start node to this node
        public int Distance { get; set; }//The "Distance" from this node to the solution
        public int Value { get; set; }//The value of this node (lower the better). Value = Length + Distance (also known as the cost to travel)
        public NodeState State { get; set; }
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
        public List<int> PuzzleEndState { get; private set; } = new List<int>()
        {
            0, 1, 2,
            3, 4, 5,
            6, 7, 8
        };
        private readonly int[][,] CorrectPuzzlePieceLocations = new int[][,]
        {
            new int[,]{ { 0 }, { 0 } }, new int[,]{ { 0 }, { 1 } }, new int[,]{ { 0 }, { 2 } },
            new int[,]{ { 1 }, { 0 } }, new int[,]{ { 1 }, { 1 } }, new int[,]{ { 1 }, { 2 } },
            new int[,]{ { 2 }, { 0 } }, new int[,]{ { 2 }, { 1 } }, new int[,]{ { 2 }, { 2 } }// white puzzle piece
        };
        public Node StartingNode { get; set; }
        public Node EndingNode { get; set; }
        #endregion Properties

        #region Constructors
        public Astar(List<int> StartingState)
        {
            PuzzleStartState = StartingState;
        }
        public Astar(List<int> StartingState, List<int> EndingState)
        {
            PuzzleStartState = StartingState;
            PuzzleEndState = EndingState;
        }
        #endregion Constructors

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
                            possibleNode.State = NodeState.Open;
                            openList.Add(possibleNode);
                        }
                        else
                        {
                            possibleNode.State = NodeState.Closed;
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
            Node LeftOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Left, Length = fromNode.Length + 1, State = NodeState.Untested };
            List<int> CurrentPuzzleState = new List<int>();
            CurrentPuzzleState = fromNode.PuzzleState.ToList();

            int Location = 0;
            int WhitePuzzlePieceLocation = new int();
            int LeftOfWhitePuzzlePieceLocation = new int();
            foreach (int item in CurrentPuzzleState)
            {
                if (item == 8)
                {
                    WhitePuzzlePieceLocation = Location;
                    LeftOfWhitePuzzlePieceLocation = Location - 1;
                }
                Location++;
            }
            if (WhitePuzzlePieceLocation != 0 && WhitePuzzlePieceLocation != 3 && WhitePuzzlePieceLocation != 6)
            {
                CurrentPuzzleState = SwapLocation(CurrentPuzzleState, WhitePuzzlePieceLocation, LeftOfWhitePuzzlePieceLocation);
                LeftOfNode = SetNodeInfo(LeftOfNode, CurrentPuzzleState);
                //Debug.WriteLine("Left");
            }
            else
            {
                LeftOfNode.IsMoveable = false;
                LeftOfNode.State = NodeState.Closed;
            }
            if (CheckCompletion(CurrentPuzzleState) == true)
            {
                LeftOfNode.Direction = Direction.None;
                EndingNode = LeftOfNode;
            }
            return LeftOfNode;
        }
        private Node Right(Node fromNode)
        {
            Node RightOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Right, Length = fromNode.Length + 1, State = NodeState.Untested };
            List<int> CurrentPuzzleState = fromNode.PuzzleState.ToList();

            int Location = 0;
            int WhitePuzzlePieceLocation = new int();
            int RightOfWhitePuzzlePieceLocation = new int();
            foreach (int item in CurrentPuzzleState)
            {
                if (item == 8)
                {
                    WhitePuzzlePieceLocation = Location;
                    RightOfWhitePuzzlePieceLocation = Location + 1;
                }
                Location++;
            }
            if (WhitePuzzlePieceLocation != 2 && WhitePuzzlePieceLocation != 5 && WhitePuzzlePieceLocation != 8)
            {
                CurrentPuzzleState = SwapLocation(CurrentPuzzleState, WhitePuzzlePieceLocation, RightOfWhitePuzzlePieceLocation);
                RightOfNode = SetNodeInfo(RightOfNode, CurrentPuzzleState);
                // Debug.WriteLine("Right");
            }
            else
            {
                RightOfNode.IsMoveable = false;
                RightOfNode.State = NodeState.Closed;
            }
            if (CheckCompletion(CurrentPuzzleState) == true)
            {
                RightOfNode.Direction = Direction.None;
                EndingNode = RightOfNode;
            }
            return RightOfNode;
        }
        private Node Up(Node fromNode)
        {
            Node UpOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Up, Length = fromNode.Length + 1, State = NodeState.Untested };
            List<int> CurrentPuzzleState = fromNode.PuzzleState.ToList();

            int Location = 0;
            int WhitePuzzlePieceLocation = new int();
            int UpOfWhitePuzzlePieceLocation = new int();
            foreach (int item in CurrentPuzzleState)
            {
                if (item == 8)
                {
                    WhitePuzzlePieceLocation = Location;
                    UpOfWhitePuzzlePieceLocation = Location - 3;
                }
                Location++;
            }
            if (WhitePuzzlePieceLocation != 0 && WhitePuzzlePieceLocation != 1 && WhitePuzzlePieceLocation != 2)
            {
                CurrentPuzzleState = SwapLocation(CurrentPuzzleState, WhitePuzzlePieceLocation, UpOfWhitePuzzlePieceLocation);
                UpOfNode = SetNodeInfo(UpOfNode, CurrentPuzzleState);
                // Debug.WriteLine("Up");
            }
            else
            {
                UpOfNode.IsMoveable = false;
                UpOfNode.State = NodeState.Closed;
            }
            if (CheckCompletion(CurrentPuzzleState) == true)
            {
                UpOfNode.Direction = Direction.None;
                EndingNode = UpOfNode;
            }
            return UpOfNode;
        }
        private Node Down(Node fromNode)
        {
            Node DownOfNode = new Node() { ParentNode = fromNode, Direction = Direction.Down, Length = fromNode.Length + 1, State = NodeState.Untested };
            List<int> CurrentPuzzleState = fromNode.PuzzleState.ToList();

            int Location = 0;
            int WhitePuzzlePieceLocation = new int();
            int DownOfWhitePuzzlePieceLocation = new int();
            foreach (int item in CurrentPuzzleState)
            {
                if (item == 8)
                {
                    WhitePuzzlePieceLocation = Location;
                    DownOfWhitePuzzlePieceLocation = Location + 3;
                }
                Location++;
            }
            if (WhitePuzzlePieceLocation != 6 && WhitePuzzlePieceLocation != 7 && WhitePuzzlePieceLocation != 8)
            {
                CurrentPuzzleState = SwapLocation(CurrentPuzzleState, WhitePuzzlePieceLocation, DownOfWhitePuzzlePieceLocation);
                DownOfNode = SetNodeInfo(DownOfNode, CurrentPuzzleState);
                //Debug.WriteLine("Down");
            }
            else
            {
                DownOfNode.IsMoveable = false;
                DownOfNode.State = NodeState.Closed;
            }
            if (CheckCompletion(CurrentPuzzleState) == true)
            {
                DownOfNode.Direction = Direction.None;
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
            CurrentNode.State = NodeState.Open;
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
