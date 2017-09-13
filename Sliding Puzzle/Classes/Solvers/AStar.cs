using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sliding_Puzzle.Classes.Solvers
{
    
    class AStar
    {
        #region Properties
        private List<int> StartingState { get; set; }
        private List<int> FinalState = new List<int>
        {
            0,  1,  2,
            3,  4,  5,
            6,  7,  8,
        };
        private int[][,] CorrectPuzzlePieceLocations = new int[][,]
        {
            new int[,]{ { 0 }, { 0 } }, new int[,]{ { 0 }, { 1 } }, new int[,]{ { 0 }, { 2 } },
            new int[,]{ { 1 }, { 0 } }, new int[,]{ { 1 }, { 1 } }, new int[,]{ { 1 }, { 2 } },
            new int[,]{ { 2 }, { 0 } }, new int[,]{ { 2 }, { 1 } }, new int[,]{ { 2 }, { 2 } }// white puzzle piece
        };
        #endregion Properties

        private List<State> OpenSet = new List<State>();
        private List<State> ClosedSet = new List<State>();
        public AStar(List<int> StartingState)
        {
            this.StartingState = StartingState;
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
    }
    class State
    {

    }

}
