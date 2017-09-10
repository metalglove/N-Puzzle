using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sliding_Puzzle.Classes
{
    class Solver
    {
        private List<int> StartingState { get; set; }
        private List<int> FinalState = new List<int>
        {
            0,  1,  2,  3,  4,
            5,  6,  7,  8,  9,
            10, 11, 12, 13, 14,
            15, 16, 17, 18, 19,
            20, 21, 22, 23, 24
        };
        private List<int[,]> CorrectPuzzleLocations = new List<int[,]>()
        {
            new int[,]{ { 0 }, { 0 } },
            new int[,]{ { 0 }, { 1 } },
            new int[,]{ { 0 }, { 2 } },
            new int[,]{ { 0 }, { 3 } },
            new int[,]{ { 0 }, { 4 } },
            new int[,]{ { 1 }, { 0 } },
            new int[,]{ { 1 }, { 1 } },
            new int[,]{ { 1 }, { 2 } },
            new int[,]{ { 1 }, { 3 } },
            new int[,]{ { 1 }, { 4 } },
            new int[,]{ { 2 }, { 0 } },
            new int[,]{ { 2 }, { 1 } },
            new int[,]{ { 2 }, { 2 } },
            new int[,]{ { 2 }, { 3 } },
            new int[,]{ { 2 }, { 4 } },
            new int[,]{ { 3 }, { 0 } },
            new int[,]{ { 3 }, { 1 } },
            new int[,]{ { 3 }, { 2 } },
            new int[,]{ { 3 }, { 3 } },
            new int[,]{ { 3 }, { 4 } },
            new int[,]{ { 4 }, { 0 } },
            new int[,]{ { 4 }, { 1 } },
            new int[,]{ { 4 }, { 2 } },
            new int[,]{ { 4 }, { 3 } },
            new int[,]{ { 4 }, { 4 } }// is the white puzzle piece
        };
        private List<int[,]> UnMovablePuzzleLocations = new List<int[,]>();
        public Solver(List<int> StartingState)
        {
            this.StartingState = StartingState;
            Solve();
        }
        public void Solve()
        {
            int CorrectPosition = 0;
            while (StartingState != FinalState)
            {
                int i = 0;
                int WhitePuzzlePieceLocation = new int();
                int PuzzlePieceLocation = new int();
                foreach (int item in StartingState)
                {
                    if (item == FinalState[CorrectPosition])
                    {
                        PuzzlePieceLocation = i;
                    }
                    if (item == 24)
                    {
                        WhitePuzzlePieceLocation = i;
                    }
                    i++;
                }
                int[,] PPCurrentLocation = CorrectPuzzleLocations[PuzzlePieceLocation];
                int[,] PPCorrectLocation = CorrectPuzzleLocations[CorrectPosition];
                int[,] WPPCurrentLocation = CorrectPuzzleLocations[PuzzlePieceLocation];

                if (PPCurrentLocation[0,0] == PPCorrectLocation[0,0] && PPCurrentLocation[1,0] == PPCorrectLocation[1,0])
                {
                    UnMovablePuzzleLocations.Add(PPCurrentLocation);
                    CorrectPosition++;
                }
                else
                {
                    int CorrectRow = PPCorrectLocation[0, 0];
                    int CorrectColumn = PPCorrectLocation[1, 0];
                    int CurrentRow = PPCurrentLocation[0, 0];
                    int CurrentColumn = PPCurrentLocation[1, 0];

                    int RowDifference = CorrectRow - CurrentRow;
                    int ColumnDifference = CorrectColumn - CurrentColumn;

                    MoveVertical(RowDifference);
                    MoveHorizontal(ColumnDifference);
                }
            }
        }
        private void MoveHorizontal(int columnDifference)
        {
            throw new NotImplementedException();
        }
        private void MoveVertical(int rowDifference)
        {
            throw new NotImplementedException();
        }
    }
}
