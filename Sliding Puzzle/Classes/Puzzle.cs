using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.ComponentModel;

namespace Sliding_Puzzle.Classes
{
    public class Puzzle : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _Name;
        public string Name
        {
            get { return _Name; }
        }
        private StorageFolder _Folder;
        public StorageFolder Folder
        {
            get { return _Folder; }
        }
        private BitmapImage _Image;
        public BitmapImage Image
        {
            get { return _Image; }
        }
        private bool _IsPuzzleSize3Available = false;
        public bool IsPuzzleSize3Available 
        {
            get { return _IsPuzzleSize3Available; }
            set { _IsPuzzleSize3Available = value; }
        }
        private bool _IsPuzzleSize4Available = false;
        public bool IsPuzzleSize4Available
        {
            get { return _IsPuzzleSize4Available; }
            set { _IsPuzzleSize4Available = value; }
        }
        private bool _IsPuzzleSize5Available = false;
        public bool IsPuzzleSize5Available
        {
            get { return _IsPuzzleSize5Available; }
            set { _IsPuzzleSize5Available = value; }
        }
        private bool _IsPuzzleSize6Available = false;
        public bool IsPuzzleSize6Available
        {
            get { return _IsPuzzleSize6Available; }
            set { _IsPuzzleSize6Available = value; }
        }

        public Puzzle(string Name, StorageFolder Folder, BitmapImage Image, bool IsPuzzleSize3Available, bool IsPuzzleSize4Available, bool IsPuzzleSize5Available, bool IsPuzzleSize6Available)
        {
            _Name = Name;
            _Folder = Folder;
            _Image = Image;
            _IsPuzzleSize3Available = IsPuzzleSize3Available;
            _IsPuzzleSize4Available = IsPuzzleSize4Available;
            _IsPuzzleSize5Available = IsPuzzleSize5Available;
            _IsPuzzleSize6Available = IsPuzzleSize6Available;
        }
    }

}
