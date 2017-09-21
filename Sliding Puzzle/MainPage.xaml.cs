﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using MoreLinq;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.Collections.ObjectModel;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using System.ComponentModel;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Sliding_Puzzle
{
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            lvPuzzles.ItemsSource = Classes.PuzzleList.Instance;
            LoadExistingPuzzlesAsync();
        }
        private async void Puzzle_ClickAsync(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            StorageFolder Folder = (StorageFolder)button.Tag;
            int PuzzleSize = int.Parse(button.Content.ToString());
            if (await CheckIfItemExistsAsync(Folder, PuzzleSize.ToString()) == true)
            {
                this.ContentFrame.Navigate(typeof(Views.SlidingPuzzle), new Classes.SlidingPuzzle(PuzzleSize, Folder, Folder.DisplayName));
            }
            else
            {
                ContentDialog dg = new ContentDialog()
                {
                    Title = "Error!",
                    Content = "The puzzle folder does not contain the desired puzzlesize. The application will now shutdown.",
                    CloseButtonText = "Ok",
                    RequestedTheme = ElementTheme.Dark
                };
                await dg.ShowAsync();
                App.Current.Exit();
            }

        }
        private void CreatePuzzleFromImage_Click(object sender, RoutedEventArgs e)
        {
            this.ContentFrame.Navigate(typeof(Views.CreatePuzzle), Classes.PuzzleList.Instance);
        }
        private async void LoadExistingPuzzlesAsync()
        {
            StorageFolder pictureFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("SlidingPuzzles", CreationCollisionOption.OpenIfExists);
            IReadOnlyList<StorageFolder> folderList = await pictureFolder.GetFoldersAsync();
            foreach (StorageFolder item in folderList)
            {
                if (await CheckIfItemExistsAsync(item, item.DisplayName + ".png") == false)
                {
                    continue;
                }
                BitmapImage image = await GetImageFromStorageFolderAsync(item);
                Classes.PuzzleList.Instance.Add(new Classes.Puzzle(item.DisplayName, item, image, await CheckIfItemExistsAsync(item, "3"), await CheckIfItemExistsAsync(item, "4"), await CheckIfItemExistsAsync(item, "5"), await CheckIfItemExistsAsync(item, "6")));
            }
        }
        private async Task<BitmapImage> GetImageFromStorageFolderAsync(StorageFolder Folder)
        {
            BitmapImage image = new BitmapImage();
            StorageFile img = await Folder.GetFileAsync(Folder.DisplayName + ".png");
            using (IRandomAccessStream fileStream = await img.OpenAsync(FileAccessMode.Read))
            {
                image.SetSource(fileStream);
            }
            return image;
        }
        private async Task<bool> CheckIfItemExistsAsync(StorageFolder Folder, string itemName)
        {
            var item = await Folder.TryGetItemAsync(itemName);
            return item != null;
        }
    }

}
