using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sliding_Puzzle.Classes.Solvers
{
    class Move
    {
        public int CalculatedDistanceFromSolution { get; set; }
        public Direction Direction { get; set; }
    }
    class SolverV2
    {
        #region Properties
        private static int CompletedDistance = 0;
        private bool IsCompleted = false;
        private List<int> FinalState = new List<int>
        {
            0,  1,  2,
            3,  4,  5,
            6,  7,  8
        };
        private int[][,] CorrectPuzzlePieceLocations = new int[][,]
        {
            new int[,]{ { 0 }, { 0 } }, new int[,]{ { 0 }, { 1 } }, new int[,]{ { 0 }, { 2 } },
            new int[,]{ { 1 }, { 0 } }, new int[,]{ { 1 }, { 1 } }, new int[,]{ { 1 }, { 2 } },
            new int[,]{ { 2 }, { 0 } }, new int[,]{ { 2 }, { 1 } }, new int[,]{ { 2 }, { 2 } }// white puzzle piece
        };
        private List<int> CurrentState = new List<int>();
        #endregion Properties
        private Direction LastDirection = Direction.None;
        public SolverV2(List<int> StartingState)
        {
            CurrentState = StartingState;
            
            while (IsCompleted == false)
            {
                List<int> TempState = new List<int>();
                Move BestMove = DetermineNextMove();
                switch (BestMove.Direction)
                {
                    case Direction.Left:
                        TempState = Left(CopyState(CurrentState));
                        Debug.WriteLine("Move: Left + Distance: " + BestMove.CalculatedDistanceFromSolution);
                        LastDirection = Direction.Left;
                        break;
                    case Direction.Right:
                        TempState = Right(CopyState(CurrentState));
                        Debug.WriteLine("Move: Right + Distance: " + BestMove.CalculatedDistanceFromSolution);
                        LastDirection = Direction.Right;
                        break;
                    case Direction.Up:
                        TempState = Up(CopyState(CurrentState));
                        Debug.WriteLine("Move: Up + Distance: " + BestMove.CalculatedDistanceFromSolution);
                        LastDirection = Direction.Up;
                        break;
                    case Direction.Down:
                        TempState = Down(CopyState(CurrentState));
                        Debug.WriteLine("Move: Down + Distance: " + BestMove.CalculatedDistanceFromSolution);
                        LastDirection = Direction.Down;
                        break;
                    case Direction.None:
                        Debug.WriteLine("Magic happened.");
                        break;
                    default:
                        Debug.WriteLine("Default in switch case hit.");
                        break;
                }
                CurrentState = TempState;
            }
        }
        #region Events
        private Move DetermineNextMove()
        {
            List<Move> Moves = new List<Move> { Left(), Right(), Up(), Down() };
            int LowestDistance = Moves.Select(item => item.CalculatedDistanceFromSolution).ToList().Min();
            Move BestMove = Moves.Where(item => item.CalculatedDistanceFromSolution == LowestDistance).First();// First because there could be the same distances and it does not matter what one will be chosen
            if (BestMove.Direction == LastDirection)
            {
                Moves.Remove(BestMove);
                BestMove = Moves.Where(item => item.CalculatedDistanceFromSolution == LowestDistance).First();
            }
            return BestMove;
        }
        private int CalculateDistanceFromSolution(List<int> CurrentStateList)
        {
            int CalculatedDistanceFromSolution = 0;
            for (int currentNumberInList = 0; currentNumberInList < CurrentStateList.Count; currentNumberInList++)
            {
                int Number = CurrentStateList[currentNumberInList];

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
            }

            return CalculatedDistanceFromSolution;
        }
        private bool CheckCompletion(List<int> startingStateLeft)
        {
            bool bReachedFinalState = false;
            for (int i = 0; i < startingStateLeft.Count; i++)
            {
                if (startingStateLeft[i] == FinalState[i])
                {
                    bReachedFinalState = true;
                }
                else
                {
                    return false;
                }
            }
            return bReachedFinalState;
        }
        private List<int> CopyState(List<int> State)
        {
            List<int> NewState = State.ToList();// new List<int>();
            //foreach (int item in State)
            //{
            //    NewState.Add(item);
            //}
            return NewState;
        }

        #region DetermineMoves
        private Move Left()
        {
            Move ReturnLeft = new Move()
            {
                CalculatedDistanceFromSolution = 0,
                Direction = Direction.Left
            };
            //copy list
            List<int> CopyOfCurrentState = CopyState(CurrentState);
            //calculate the distance
            ReturnLeft.CalculatedDistanceFromSolution = CalculateDistanceFromSolution(Left(CopyOfCurrentState));
            //return the distance
            return ReturnLeft;
        }
        private Move Right()
        {
            Move ReturnRight = new Move()
            {
                CalculatedDistanceFromSolution = 0,
                Direction = Direction.Right
            };
            //copy list
            List<int> CopyOfCurrentState = CopyState(CurrentState);
            //calculate the distance
            ReturnRight.CalculatedDistanceFromSolution = CalculateDistanceFromSolution(Right(CopyOfCurrentState));
            //return the distance
            return ReturnRight;
        }
        private Move Up()
        {
            Move ReturnUp = new Move()
            {
                CalculatedDistanceFromSolution = 0,
                Direction = Direction.Up
            };
            //copy list
            List<int> CopyOfCurrentState = CopyState(CurrentState);
            //calculate the distance
            ReturnUp.CalculatedDistanceFromSolution = CalculateDistanceFromSolution(Up(CopyOfCurrentState));
            //return the distance
            return ReturnUp;
        }
        private Move Down()
        {
            Move ReturnDown = new Move()
            {
                CalculatedDistanceFromSolution = 0,
                Direction = Direction.Down
            };
            //copy list
            List<int> CopyOfCurrentState = CopyState(CurrentState);
            //calculate the distance
            ReturnDown.CalculatedDistanceFromSolution = CalculateDistanceFromSolution(Down(CopyOfCurrentState));
            //return the distance
            return ReturnDown;
        }
        #endregion DetermineMoves 

        #region PerformMoves
        private List<int> Left(List<int> StartingStateLeft)
        {
            //Check for completion
            if (CheckCompletion(StartingStateLeft) == true)
            {
                IsCompleted = true;
            }
            else
            {
                //Find WhitePuzzle piece
                //Find the left position of the WhitePuzzlepiece
                int i = 0;
                int WhitePuzzlePieceLocation = new int();
                int LeftOfPuzzlePieceLocation = new int();
                foreach (int item in StartingStateLeft)
                {
                    if (item == 8)
                    {
                        WhitePuzzlePieceLocation = i;
                        LeftOfPuzzlePieceLocation = i - 1;
                    }
                    i++;
                }
                //if the position is able to move, move it
                if (WhitePuzzlePieceLocation != 0 && WhitePuzzlePieceLocation != 3 && WhitePuzzlePieceLocation != 6)
                {
                    int SwapValue1 = StartingStateLeft[WhitePuzzlePieceLocation];
                    int SwapValue2 = StartingStateLeft[LeftOfPuzzlePieceLocation];
                    StartingStateLeft[WhitePuzzlePieceLocation] = SwapValue2;
                    StartingStateLeft[LeftOfPuzzlePieceLocation] = SwapValue1;
                }
            }
            return StartingStateLeft;
        }
        private List<int> Right(List<int> StartingStateRight)
        {
            //Check for completion
            if (CheckCompletion(StartingStateRight) == true)
            {
                IsCompleted = true;
            }
            else
            {
                //Find WhitePuzzle piece
                //Find the right position of the WhitePuzzlepiece
                int i = 0;
                int WhitePuzzlePieceLocation = new int();
                int RightOfPuzzlePieceLocation = new int();
                foreach (int item in StartingStateRight)
                {
                    if (item == 8)
                    {
                        WhitePuzzlePieceLocation = i;
                        RightOfPuzzlePieceLocation = i + 1;
                    }
                    i++;
                }
                //if the position is able to move, move it
                if (WhitePuzzlePieceLocation != 2 && WhitePuzzlePieceLocation != 5 && WhitePuzzlePieceLocation != 8)
                {
                    int SwapValue1 = StartingStateRight[WhitePuzzlePieceLocation];
                    int SwapValue2 = StartingStateRight[RightOfPuzzlePieceLocation];
                    StartingStateRight[WhitePuzzlePieceLocation] = SwapValue2;
                    StartingStateRight[RightOfPuzzlePieceLocation] = SwapValue1;
                }
            }
            return StartingStateRight;
        }
        private List<int> Up(List<int> StartingStateUp)
        {
            //Check for completion
            if (CheckCompletion(StartingStateUp) == true)
            {
                IsCompleted = true;
            }
            else
            {
                //Find WhitePuzzle piece
                //Find the up position of the WhitePuzzlepiece
                int i = 0;
                int WhitePuzzlePieceLocation = new int();
                int UpOfPuzzlePieceLocation = new int();
                foreach (int item in StartingStateUp)
                {
                    if (item == 8)
                    {
                        WhitePuzzlePieceLocation = i;
                        UpOfPuzzlePieceLocation = i - 2;
                    }
                    i++;
                }
                //if the position is able to move, move it
                if (WhitePuzzlePieceLocation != 0 && WhitePuzzlePieceLocation != 1 && WhitePuzzlePieceLocation != 2)
                {
                    int SwapValue1 = StartingStateUp[WhitePuzzlePieceLocation];
                    int SwapValue2 = StartingStateUp[UpOfPuzzlePieceLocation];
                    StartingStateUp[WhitePuzzlePieceLocation] = SwapValue2;
                    StartingStateUp[UpOfPuzzlePieceLocation] = SwapValue1;
                }
            }
            return StartingStateUp;
        }
        private List<int> Down(List<int> StartingStateDown)
        {
            //Check for completion
            if (CheckCompletion(StartingStateDown) == true)
            {
                IsCompleted = true;
            }
            else
            {
                //Find WhitePuzzle piece
                //Find the down position of the WhitePuzzlepiece
                int i = 0;
                int WhitePuzzlePieceLocation = new int();
                int DownOfPuzzlePieceLocation = new int();
                foreach (int item in StartingStateDown)
                {
                    if (item == 8)
                    {
                        WhitePuzzlePieceLocation = i;
                        DownOfPuzzlePieceLocation = i + 2;
                    }
                    i++;
                }
                //if the position is able to move, move it
                if (WhitePuzzlePieceLocation != 6 && WhitePuzzlePieceLocation != 7 && WhitePuzzlePieceLocation != 8)
                {
                    int SwapValue1 = StartingStateDown[WhitePuzzlePieceLocation];
                    int SwapValue2 = StartingStateDown[DownOfPuzzlePieceLocation];
                    StartingStateDown[WhitePuzzlePieceLocation] = SwapValue2;
                    StartingStateDown[DownOfPuzzlePieceLocation] = SwapValue1;
                }
            }
            return StartingStateDown;
        }
        #endregion PerformMoves

        #endregion Events

        #region Functions
        public List<Direction> GetMoves()
        {
            return new List<Direction>();
        }
        #endregion Functions
    }
}
