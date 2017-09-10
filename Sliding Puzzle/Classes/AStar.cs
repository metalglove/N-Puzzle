using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sliding_Puzzle.Classes
{
    
    class AStar
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

        private List<State> OpenSet = new List<State>();
        private List<State> ClosedSet = new List<State>();
        public AStar(List<int> StartingState)
        {
            this.StartingState = StartingState;
        }
    }
    class State
    {

    }

}
