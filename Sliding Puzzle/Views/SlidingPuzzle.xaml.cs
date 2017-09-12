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

namespace Sliding_Puzzle.Views
{
    public sealed partial class SlidingPuzzle : Page
    {
        private Classes.SlidingPuzzle Puzzle { get; set; }
        private DispatcherTimer CheckTimeSpent = new DispatcherTimer();
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
            SlidingPuzzleGrid.Children.Add(Puzzle.GetPuzzle());
        }
        private async Task ClearPuzzleAsync()
        {
            SlidingPuzzleGrid.Children.Clear();
            Puzzle.ResetPuzzle();
            await Puzzle.GeneratePuzzle();
        }
        private async void ResetButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            await ClearPuzzleAsync();
            SetPuzzle();
        }
        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            Puzzle.SolvePuzzle();
        }
    }
}
