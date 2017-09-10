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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Puzzle = e.Parameter as Classes.SlidingPuzzle;
            SetPuzzle();
            StartTimer();
            base.OnNavigatedTo(e);//
        }

        private void StartTimer()
        {
            CheckTimeSpent.Interval = TimeSpan.FromMilliseconds(500);
            CheckTimeSpent.Tick += SetPuzzleTime;
            CheckTimeSpent.Start();
        }
        private void SetPuzzleTime(object sender, object e)
        {
            TimerTextBlock.Text = "Speeltijd: " + Puzzle.TimeSpent.ToString();
        }
        private void SetPuzzle()
        {
            SlidingPuzzleGrid.Children.Add(Puzzle.GetPuzzle());
        }
        private void ClearPuzzle()
        {
            SlidingPuzzleGrid.Children.Clear();
            Puzzle.ResetPuzzle();
        }
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            ClearPuzzle();
            SetPuzzle();
        }

        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            Puzzle.SolvePuzzle();
        }
    }
}
