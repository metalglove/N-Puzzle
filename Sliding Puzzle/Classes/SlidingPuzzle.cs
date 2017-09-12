using MoreLinq;
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
        public string PuzzleName { get; set; }
        public StorageFolder Folder { get; set; }
        private Grid PuzzleBox = new Grid();
        private List<Grid> PuzzlePieces = new List<Grid>();
        private Grid WhitePuzzlePiece = new Grid();
        private List<int> PuzzlePiecesAsInt = new List<int>();
        private int[][,] CorrectPuzzleLocations = new int[][,]
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
        private Random Random = new Random();
        private List<BitmapImage> PuzzlePieceImages = new List<BitmapImage>();
        #endregion PuzzleSetup

        #region PuzzleGame
        public int Moves { get; private set; }
        public int TimeSpent { get; private set; }
        private DispatcherTimer PuzzleTimer { get; set; }
        #endregion
        public SlidingPuzzle(int PuzzleSize, string PuzzleName)
        {
            this.PuzzleSize = PuzzleSize;
            this.PuzzleName = PuzzleName;
            GeneratePuzzle();
        }
        public SlidingPuzzle(int PuzzleSize, StorageFolder Folder, string PuzzleName)
        {
            this.PuzzleSize = PuzzleSize;
            this.Folder = Folder;
            this.PuzzleName = PuzzleName;

          
            //GeneratePuzzle();
        }

        #region PuzzleCreation
        public async void GeneratePuzzle()
        {
            await GetAllImages();
            CreatePuzzleBoxHolder();
            LoadPuzzleBoxHolder();
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
            Row.Height = new GridLength(100, GridUnitType.Auto);
            return Row;
        }
        private ColumnDefinition AddColumn()
        {
            ColumnDefinition Column = new ColumnDefinition();
            Column.Width = new GridLength(100, GridUnitType.Auto);
            return Column;
        }
        private void LoadPuzzleBoxHolder()
        {
           
            int PuzzleCount = (PuzzleSize * PuzzleSize);
            string output = "";
            for (int i = 0; i < PuzzleCount; i++)
            {
                PuzzlePiecesAsInt.Add(i);
                output += i + " ";
            }
            Debug.WriteLine(output);
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
            for (int i = 0; i < PuzzleCount; i++)
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
            if (PuzzlePieceCount == (PuzzleSize * PuzzleSize) - 1)
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
            //brush.ImageSource = GetImageFromStorageFolderAsync((PuzzlePieceCount + 1)).Result;
            //brush.ImageSource = new BitmapImage(new Uri("ms-appx:///SlidingPuzzles/" + PuzzleName +"/" + (PuzzlePieceCount + 1) + ".png", UriKind.RelativeOrAbsolute));
            //PuzzlePiece.Background = new SolidColorBrush(Colors.Red);
            PuzzlePiece.Background = brush;
            Button button = new Button();
            //button.Background = brush;
            button.VerticalAlignment = VerticalAlignment.Center;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.Click += (sender, e) => Move(sender, e, PuzzlePiece);
            button.Width = 100;
            button.Height = 100;
            Debug.Write(PuzzlePieceCount + " ");
            //button.Content = PuzzlePieceCount.ToString();
            PuzzlePiece.Children.Add(button);

            return PuzzlePiece;
        }
        private bool CheckSolvability()
        {
            int inversions = 0;
            for (int i = 0; i < PuzzlePiecesAsInt.Count - 1; i++)
            {
                // Check if a larger number exists after the current
                // place in the array, if so increment inversions.
                for (int j = i + 1; j < PuzzlePiecesAsInt.Count; j++)
                {
                    if (PuzzlePiecesAsInt[i] > PuzzlePiecesAsInt[j])
                    {
                        inversions++;
                    }
                }
                // Determine if the distance of the blank space from the bottom 
                // right is even or odd, and increment inversions if it is odd.
                if (PuzzlePiecesAsInt[i] == 24 && i % 2 == 1)
                {
                    inversions++;
                }
            }
            // If inversions is even, the puzzle is solvable.
            return (inversions % 2 == 0);
        }
        private async Task<BitmapImage> GetImageFromStorageFolderAsync(int puzzlepiece)
        {
            Debug.WriteLine("Image from folder: " + puzzlepiece.ToString());
            BitmapImage image = new BitmapImage();
            try
            {
                StorageFolder pictureFolder2 = await Folder.GetFolderAsync(PuzzleSize.ToString());
                StorageFile img = await pictureFolder2.GetFileAsync(puzzlepiece + ".png");
                /*using (IRandomAccessStream fileStream = img.OpenAsync(FileAccessMode.Read).GetResults())
                {
                    image.SetSource(fileStream);
                }
                return image;*/
                image = new BitmapImage(new Uri(img.Path, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
                throw;
            }
            
            return image;

        }
        public async Task GetAllImages()
        {
            List<BitmapImage> images = new List<BitmapImage>();
            try
            {
                StorageFolder pictureFolder2 = await Folder.GetFolderAsync(PuzzleSize.ToString());
                for (int i = 1; i < 25; i++)
                {
                    Debug.WriteLine("Image from folder: " + i.ToString());
                    StorageFile img = await pictureFolder2.GetFileAsync(i.ToString() + ".png");
                    BitmapImage image = new BitmapImage(new Uri(img.Path, UriKind.Absolute));
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
            //GeneratePuzzle();
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

        public Grid GetPuzzle()
        {
            return PuzzleBox;
        }
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
            GeneratePuzzle();
        }
        public void SolvePuzzle()
        {
            Tree TreeSolver = new Tree(PuzzlePiecesAsInt);
            
        }
    }
}
