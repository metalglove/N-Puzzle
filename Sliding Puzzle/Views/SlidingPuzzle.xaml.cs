using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Sliding_Puzzle.Classes;
using System.Threading.Tasks;
using Sliding_Puzzle.Classes.Solvers;
using System.Diagnostics;
using Windows.UI.Core;

namespace Sliding_Puzzle.Views
{
    public sealed partial class SlidingPuzzle : Page
    {
        private Classes.SlidingPuzzle Puzzle { get; set; }
        private DispatcherTimer CheckTimeSpent = new DispatcherTimer();
        private List<Direction> Moves = new List<Direction>();
        private Grid PuzzleGrid { get; set; }
        private Grid newPuzzleGrid { get; set; }
        public SlidingPuzzle()
        {
            this.InitializeComponent();
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Puzzle = e.Parameter as Classes.SlidingPuzzle;
            await Puzzle.GeneratePuzzle();
            SetPuzzle();
            StartTimer();
            base.OnNavigatedTo(e);
        }
        private void StartTimer()
        {
            CheckTimeSpent.Interval = TimeSpan.FromMilliseconds(500);
            CheckTimeSpent.Tick += SetPuzzleTime;
            CheckTimeSpent.Start();
        }
        private void SetPuzzleTime(object sender, object e)
        {
            TimerTextBlock.Text = "Time played: " + Puzzle.TimeSpent.ToString();
        }
        private void SetPuzzle()
        {
            PuzzleGrid = Puzzle.PuzzleBox;
            PuzzleGrid.Name = "GeneratedPuzzleGrid";
            Grid.SetColumn(PuzzleGrid, 0);
            SlidingPuzzleGrid.Children.Add(PuzzleGrid);
        }
        private async Task ClearPuzzleAsync()
        {
            SlidingPuzzleGrid.Children.Remove(PuzzleGrid);
            Puzzle.ResetPuzzle();
            await Puzzle.GeneratePuzzle();
        }
        private async void ResetButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            await ClearPuzzleAsync();
            SetPuzzle();
            lsMoves.Items.Clear();
        }
        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            Puzzle.SolvePuzzle(GetPuzzlePiecesAsInt());
            Moves = Puzzle.GetSolvingMoves();
            Task.Run(() => PerformSolvingMoves());
            lsMoves.Items.Clear();
            foreach (Direction item in Moves)
            {
                lsMoves.Items.Add(item.ToString());
            }
        }
        private async void PerformSolvingMoves()
        {
            foreach (Direction Direction in Moves)
            {
                
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => 
                {
                    Tuple<Grid, Grid> PuzzlePiecess = GetPuzzlePieceToMove(Direction);
                    Puzzle.Move(PuzzlePiecess.Item2);
                    Debug.WriteLine("Moved: " + PuzzlePiecess.Item2.Tag.ToString() + " and " + PuzzlePiecess.Item1.Tag.ToString());
                });
                await Task.Delay(200);
            }
        }
        private Tuple<Grid,Grid> GetPuzzlePieceToMove(Direction Direction)
        {
            int wpp = 0;
            foreach (Grid PuzzlePiece in PuzzleGrid.Children)
            {
                if (PuzzlePiece.Tag.ToString() == (Puzzle.TotalPuzzleSize - 1).ToString())
                {
                    Grid witpuzelstukje = PuzzlePiece;
                    Grid puzelstukje = new Grid();
                    int pRow = Grid.GetRow(PuzzlePiece);
                    int pColumn = Grid.GetColumn(PuzzlePiece);
                    switch (Direction)
                    {
                        case Direction.Left:
                            pColumn--;
                            break;
                        case Direction.Right:
                            pColumn++;
                            break;
                        case Direction.Up:
                            pRow--;
                            break;
                        case Direction.Down:
                            pRow++;
                            break;
                        default:
                            break;
                    }

                    foreach (UIElement item in PuzzleGrid.Children)
                    {
                        Grid piec = item as Grid;
                        if (Grid.GetRow(piec) == pRow && Grid.GetColumn(piec) == pColumn)
                        {
                            puzelstukje = piec;
                        }
                    }

                    //PuzzleGrid.Children.RemoveAt(iTo);
                    //PuzzleGrid.Children.RemoveAt(wpp);
                    //if (iTo> wpp)
                    //{
                    //    PuzzleGrid.Children.Insert(wpp, puzelstukje);
                    //    PuzzleGrid.Children.Insert(iTo, witpuzelstukje);                      
                    //}
                    //else
                    //{
                    //    PuzzleGrid.Children.Insert(iTo, witpuzelstukje);
                    //    PuzzleGrid.Children.Insert(wpp, puzelstukje);                     
                    //}


                    return new Tuple<Grid, Grid>(witpuzelstukje, puzelstukje);       
                }
                wpp++;
            }
            throw new ArgumentException("PuzzlePiece not found", "wpp");
        }
        private List<int> GetPuzzlePiecesAsInt()
        {
            PuzzleGrid = SlidingPuzzleGrid.FindName("GeneratedPuzzleGrid") as Grid;
            List<int> PuzzlePiecesAsInt = new List<int>();
            foreach (Grid piece in PuzzleGrid.Children)
            {
                PuzzlePiecesAsInt.Add(int.Parse(piece.Tag.ToString()));
            }
            return PuzzlePiecesAsInt;
        }
        //int stepcount = 0;
        //private void stepsolver_Click(object sender, RoutedEventArgs e)
        //{
        //    Tuple<Grid, Grid> PuzzlePiecess = GetPuzzlePieceToMove(Moves[stepcount]);
        //    Puzzle.Move(PuzzlePiecess.Item2);
        //    this.UpdateLayout();
        //    //Task.Delay(2000);
        //    Debug.WriteLine("Moved: " + PuzzlePiecess.Item2.Tag.ToString() + " and " + PuzzlePiecess.Item1.Tag.ToString());
        //    stepcount++;
        //    step.Text = Moves[stepcount].ToString();
        //}
    }
}
