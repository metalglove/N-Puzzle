using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sliding_Puzzle.Classes.Solvers
{
    public abstract class SolvingBase
    {
        #region Properties
        private List<int> PuzzleEndState = new List<int>();
        public Node StartingNode { get; private set; }
        public Node EndingNode { get; private set; }
        private int MoveablePiece;
        private int PuzzleSize;
        public List<int> LeftList { get; private set; } = new List<int>();
        public List<int> RightList { get; private set; } = new List<int>();
        public List<int> UpList { get; private set; } = new List<int>();
        public List<int> DownList { get; private set; } = new List<int>();
        #endregion Properties

        public SolvingBase(List<int> StartingState, int Puzzlesize)
        {
            PuzzleSize = Puzzlesize;
            StartingNode = new Node() { PuzzleState = StartingState, Direction = Direction.None };
            PuzzleEndState = CalculateEndState();
            MoveablePiece = PuzzleEndState.Last();
            CalculatePuzzleBounds();
        }

        #region PuzzleSetup
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

        #region Moves
        public Node Left(Node fromNode)
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
        public Node Right(Node fromNode)
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
        public Node Up(Node fromNode)
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
        public Node Down(Node fromNode)
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
            CurrentNode.Distance = CalculateDistanceV2(CurrentPuzzleState);
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
        private int CalculateDistanceV2(List<int> currentPuzzleState)
        {
            int Distance = 0;
            for (int currentNumberInList = 0; currentNumberInList < currentPuzzleState.Count; currentNumberInList++)
            {
                int CurrentNumber = currentPuzzleState[currentNumberInList];
                if (currentNumberInList != CurrentNumber)
                {
                    Distance += (Math.Abs((currentNumberInList % 3) - (CurrentNumber % 3)) + Math.Abs((currentNumberInList / 3) - (CurrentNumber / 3))) + 1;
                }
            }
            return Distance;
        }
        private bool CheckCompletion(List<int> list) => list.SequenceEqual(PuzzleEndState);
        #endregion Events
    }
}
