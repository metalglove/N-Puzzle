using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Sliding_Puzzle.Classes
{
    public class PuzzleList : ObservableCollection<Puzzle>, INotifyPropertyChanged
    {
        private static PuzzleList instance;

        private PuzzleList() { }

        public static PuzzleList Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PuzzleList();
                }
                return instance;
            }
        }

    }
}
