using MoreLinq;
using Sliding_Puzzle.Classes.Solvers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Sliding_Puzzle.Classes
{
    class SlidingPuzzle
    {
        #region PuzzleSetup
        public int PuzzleSize { get; private set; }
        public int TotalPuzzleSize { get; private set; }
        public string PuzzleName { get; set; }
        public StorageFolder Folder { get; set; }
        public Grid PuzzleBox { get; private set;} = new Grid();
        private List<Grid> PuzzlePieces = new List<Grid>();
        private Grid WhitePuzzlePiece = new Grid();
        private List<int> PuzzlePiecesAsInt = new List<int>();
        private List<int[,]> CorrectPuzzleLocations = new List<int[,]>();
        private Random Random = new Random();
        private List<BitmapImage> PuzzlePieceImages = new List<BitmapImage>();
        private const int ImageSize = 500;
        private int MoveablePiece { get; set; }
        #endregion PuzzleSetup

        #region PuzzleGame
        public int Moves { get; private set; }
        private List<Direction> Directions { get; set; } = new List<Direction>();
        public int TimeSpent { get; private set; }
        private DispatcherTimer PuzzleTimer { get; set; }
        #endregion

        public SlidingPuzzle(int PuzzleSize, StorageFolder Folder, string PuzzleName)
        {
            this.PuzzleSize = PuzzleSize;
            this.Folder = Folder;
            this.PuzzleName = PuzzleName;
        }

        #region PuzzleCreation
        public async Task GeneratePuzzle()
        {
            await GetAllImages();
            CreatePuzzleConstants();
            CreatePuzzleBoxHolder();
            LoadPuzzleBoxHolder();
        }
        private void CreatePuzzleConstants()
        {
            TotalPuzzleSize = (PuzzleSize * PuzzleSize);
            MoveablePiece = TotalPuzzleSize - 1;
            string output = "";
            for (int i = 0; i < TotalPuzzleSize; i++)
            {
                PuzzlePiecesAsInt.Add(i);
                output += i + " ";
            }
            CalculateCorrectPuzzlePieceLocations();
            Debug.WriteLine(output);
        }
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
            CorrectPuzzleLocations = MultiDimensionalList;
        }
        private void CreatePuzzleBoxHolder()
        {
            PuzzleBox = new Grid();
            PuzzleBox.HorizontalAlignment = HorizontalAlignment.Stretch;
            PuzzleBox.VerticalAlignment = VerticalAlignment.Stretch;
            for (int Row = 0; Row < PuzzleSize; Row++)
            {
                PuzzleBox.RowDefinitions.Add(AddRow());
            }
            for (int Column = 0; Column < PuzzleSize; Column++)
            {
                PuzzleBox.ColumnDefinitions.Add(AddColumn());
            }
        }
        private RowDefinition AddRow()
        {
            RowDefinition Row = new RowDefinition();
            Row.Height = new GridLength((ImageSize / PuzzleSize), GridUnitType.Star);
            return Row;
        }
        private ColumnDefinition AddColumn()
        {
            ColumnDefinition Column = new ColumnDefinition();
            Column.Width = new GridLength((ImageSize / PuzzleSize), GridUnitType.Star);
            return Column;
        }
        private void LoadPuzzleBoxHolder()
        {
            PuzzlePiecesAsInt = PuzzlePiecesAsInt.OrderBy(item => Random.Next()).ToList();
            while (CheckSolvability() == false)
            {
                PuzzlePiecesAsInt.ForEach(inter => Debug.Write(inter + " "));
                Debug.WriteLine("");//
                PuzzlePiecesAsInt = PuzzlePiecesAsInt.OrderBy(item => Random.Next()).ToList();
                Debug.WriteLine("Not solvable game state, creating new game state.");
            }
            PuzzlePiecesAsInt.ForEach(inter => Debug.Write(inter + " "));
            Debug.WriteLine("");//
            for (int i = 0; i < TotalPuzzleSize; i++)
            {
                PuzzlePieces.Add(GeneratePuzzlePiece(PuzzlePiecesAsInt[i]));
            }
            Debug.WriteLine("");//
            int CurrentPuzzlePiece = 0;
            for (int Row = 0; Row < PuzzleSize; Row++)
            {
                for (int Column = 0; Column < PuzzleSize; Column++)
                {
                    PuzzleBox.Children.Add(PuzzlePieces[CurrentPuzzlePiece]);
                    Grid.SetRow(PuzzlePieces[CurrentPuzzlePiece], Row);
                    Grid.SetColumn(PuzzlePieces[CurrentPuzzlePiece], Column);
                    Debug.WriteLine(CurrentPuzzlePiece);
                    CurrentPuzzlePiece++;
                }
            }
        }
        private Grid GenerateWhitePuzzlePiece(int PuzzlePieceCount)
        {
            Grid PuzzlePiece = new Grid();
            PuzzlePiece.HorizontalAlignment = HorizontalAlignment.Stretch;
            PuzzlePiece.VerticalAlignment = VerticalAlignment.Stretch;
            PuzzlePiece.Background = new SolidColorBrush(Colors.White);
            PuzzlePiece.Tag = PuzzlePieceCount;
            //PuzzlePiece.Children.Add(new TextBlock() { Text = PuzzlePieceCount.ToString() });
            return PuzzlePiece;
        }
        private Grid GeneratePuzzlePiece(int PuzzlePieceCount)
        {
            if (PuzzlePieceCount == TotalPuzzleSize - 1)
            {
                WhitePuzzlePiece = GenerateWhitePuzzlePiece(PuzzlePieceCount);
                Debug.Write("White PuzzlePiece ");
                return WhitePuzzlePiece;
            }
            Grid PuzzlePiece = new Grid();
            PuzzlePiece.HorizontalAlignment = HorizontalAlignment.Stretch;
            PuzzlePiece.VerticalAlignment = VerticalAlignment.Stretch;
            PuzzlePiece.Tag = PuzzlePieceCount;
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = PuzzlePieceImages[PuzzlePieceCount];
            PuzzlePiece.Background = brush;
            Button button = new Button();
            button.VerticalAlignment = VerticalAlignment.Stretch;
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.Click += (sender, e) => Move(sender, e, PuzzlePiece);
            //button.Width = Convert.ToInt32(ImageSize / PuzzleSize);
            //button.Height = Convert.ToInt32(ImageSize / PuzzleSize);
            Debug.Write(PuzzlePieceCount + " ");
            //button.Content = PuzzlePieceCount.ToString(); for debbugging with numbers
            PuzzlePiece.Children.Add(button);
            return PuzzlePiece;
        }
        private bool CheckSolvability()
        {
            int inversions = 0;
            List<int> checker = PuzzlePiecesAsInt.ToList();
            checker.Remove(MoveablePiece);
            for (int i = 0; i < checker.Count; i++)
            {
                // Check if a larger number exists after the current
                // place in the array, if so increment inversions.
                for (int j = i + 1; j < checker.Count; j++)
                {
                    if (checker[i] > checker[j])
                    {
                        inversions++;
                    }
                }
            }
            // If inversions is even, the puzzle is solvable.
            return (inversions % 2 == 0);
        }
        public async Task GetAllImages()
        {
            List<BitmapImage> images = new List<BitmapImage>();
            try
            {
                StorageFolder pictureFolder2 = await Folder.GetFolderAsync(PuzzleSize.ToString());
                for (int i = 1; i < (PuzzleSize * PuzzleSize); i++)
                {
                    Debug.WriteLine("Image from folder: " + i.ToString());
                    StorageFile img = await pictureFolder2.GetFileAsync(i.ToString() + ".png");
                    BitmapImage image = new BitmapImage();
                    using (IRandomAccessStream fileStream = await img.OpenAsync(FileAccessMode.Read))
                    {
                        image.SetSource(fileStream);
                    }
                    images.Add(image);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                throw;
            }

            PuzzlePieceImages = images;
        }
        #endregion PuzzleCreation

        #region PuzzleEvents      
        private void Move(object sender, RoutedEventArgs e, Grid PuzzlePiece)
        {
            Debug.WriteLine("-*****-\nClicked puzzle piece : " + PuzzlePiece.Tag.ToString());
            int PuzzlePieceRow = Grid.GetRow(PuzzlePiece);
            int PuzzlePieceColumn = Grid.GetColumn(PuzzlePiece);
            int WhitePuzzlePieceRow = Grid.GetRow(WhitePuzzlePiece);
            int WhitePuzzlePieceColumn = Grid.GetColumn(WhitePuzzlePiece);
            if (PuzzlePieceRow == WhitePuzzlePieceRow || PuzzlePieceColumn == WhitePuzzlePieceColumn)
            {
                int PPR = PuzzlePieceRow - WhitePuzzlePieceRow;
                int PPC = PuzzlePieceColumn - WhitePuzzlePieceColumn;
                if (PPR >= -1 && PPR <= 1 && PPC >= -1 && PPC <= 1)
                {
                    Grid.SetRow(PuzzlePiece, WhitePuzzlePieceRow);
                    Grid.SetColumn(PuzzlePiece, WhitePuzzlePieceColumn);

                    Grid.SetRow(WhitePuzzlePiece, PuzzlePieceRow);
                    Grid.SetColumn(WhitePuzzlePiece, PuzzlePieceColumn);
                    if (Moves == 0)
                    {
                        PuzzleTimer = new DispatcherTimer();
                        PuzzleTimer.Interval = TimeSpan.FromSeconds(1);
                        PuzzleTimer.Tick += UpdateTimer;
                        PuzzleTimer.Start();
                    }
                    Moves++;
                    CheckCompletionAsync();
                }
                else
                {
                    Debug.WriteLine("Wrong move.");
                }
            }
            else
            {
                Debug.WriteLine("Wrong move.");
            }
        }
        public void Move(Grid PuzzlePiece, Grid WhitePuzzlePiece)
        {
            int PuzzlePieceRow = Grid.GetRow(PuzzlePiece);
            int PuzzlePieceColumn = Grid.GetColumn(PuzzlePiece);
            int WhitePuzzlePieceRow = Grid.GetRow(WhitePuzzlePiece);
            int WhitePuzzlePieceColumn = Grid.GetColumn(WhitePuzzlePiece);
            if (PuzzlePieceRow == WhitePuzzlePieceRow || PuzzlePieceColumn == WhitePuzzlePieceColumn)
            {
                int PPR = PuzzlePieceRow - WhitePuzzlePieceRow;
                int PPC = PuzzlePieceColumn - WhitePuzzlePieceColumn;
                if (PPR >= -1 && PPR <= 1 && PPC >= -1 && PPC <= 1)
                {
                    Grid.SetRow(PuzzlePiece, WhitePuzzlePieceRow);
                    Grid.SetColumn(PuzzlePiece, WhitePuzzlePieceColumn);

                    Grid.SetRow(WhitePuzzlePiece, PuzzlePieceRow);
                    Grid.SetColumn(WhitePuzzlePiece, PuzzlePieceColumn);
                    if (Moves == 0)
                    {
                        PuzzleTimer = new DispatcherTimer();
                        PuzzleTimer.Interval = TimeSpan.FromSeconds(1);
                        PuzzleTimer.Tick += UpdateTimer;
                        PuzzleTimer.Start();
                    }
                    Moves++;
                    //CheckCompletionAsync();
                }
            }
        }
        private void UpdateTimer(object sender, object e)
        {
            TimeSpent++;
        }
        private async void CheckCompletionAsync()
        {
            bool Completed = false;
            foreach (Grid PuzzlePiece in PuzzleBox.Children)
            {
                Grid tempPiece = PuzzlePiece;
                int PPR = Grid.GetRow(PuzzlePiece);
                int PPC = Grid.GetColumn(PuzzlePiece);
                int PPN = int.Parse(tempPiece.Tag.ToString());
                Debug.Write("Move: " + Moves +"\nPiece being checked: " + PPN + "\nCurrent location: " + "Row: " + PPR + " Column: " + PPC + "\nCorrect location: Row: " + CorrectPuzzleLocations[PPN][0, 0].ToString() + " Column: " + CorrectPuzzleLocations[PPN][1, 0].ToString() + "\n");

                int[,] location = new int[,] { { PPR }, { PPC } };
                if (CorrectPuzzleLocations[PPN][0, 0] == location[0, 0] && CorrectPuzzleLocations[PPN][1, 0] == location[1, 0])
                {
                    Debug.WriteLine("Piece is correct!\n-*****-");
                    Completed = true;
                }
                else
                {
                    Debug.WriteLine("Piece is not correct and the completion check will stop here.\n-*****-");
                    Completed = false;
                    break;
                }
            }
            if (Completed)
            {
                PuzzleTimer.Stop();
                var dialog = new MessageDialog("De puzzel is af!");
                await dialog.ShowAsync();
                Debug.WriteLine("All puzzlepieces are in the correct location and the puzzle is now completed!");
            }
        }
        #endregion PuzzleEvents

        public void ResetPuzzle()
        {
            PuzzlePieces = new List<Grid>();
            WhitePuzzlePiece = new Grid();
            PuzzlePiecesAsInt = new List<int>();
            PuzzleBox = new Grid();
            if (PuzzleTimer != null)
            {
                PuzzleTimer.Stop();
                PuzzleTimer = new DispatcherTimer();
            }
            Moves = 0;
            TimeSpent = 0;
        }
        public void SolvePuzzle(List<int> PPAI)
        {
            DateTime StartTime = DateTime.Now;
            Astar aStar = new Astar(PPAI, PuzzleSize);
            Directions = aStar.FindPath();
            DateTime EndingTime = DateTime.Now;
            var diff = EndingTime.Subtract(StartTime);
            var res = String.Format("Hours: {0} Minutes: {1} Seconds: {2} Milliseconds: {3}", diff.Hours, diff.Minutes, diff.Seconds, diff.Milliseconds);
            Debug.WriteLine(res);
            Debug.WriteLine("Puzzle solved. Moves: " + Directions.Count);
            foreach (Direction Direction in Directions)
            {
                Debug.Write(Direction + " ");
            }
        }
        public List<Direction> GetSolvingMoves()
        {
            return Directions;
        }
    }
}
