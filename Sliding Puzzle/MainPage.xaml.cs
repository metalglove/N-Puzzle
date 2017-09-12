using System;
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
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Sliding_Puzzle
{
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Puzzle> puzzleList = new ObservableCollection<Puzzle>();

        public MainPage()
        {
            this.InitializeComponent();
            LoadExistingPuzzlesAsync();
            lvPuzzles.ItemsSource = puzzleList;
        }

        private void Puzzle_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            StorageFolder Folder = (StorageFolder)button.Tag;
            int PuzzleSize = int.Parse(button.Content.ToString());
            this.ContentFrame.Navigate(typeof(Views.SlidingPuzzle), new Classes.SlidingPuzzle(PuzzleSize, Folder, Folder.DisplayName));
        }
        private void CreatePuzzleFromImage_Click(object sender, RoutedEventArgs e)
        {
            this.ContentFrame.Navigate(typeof(Views.CreatePuzzle));
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
                puzzleList.Add(new Puzzle(item.DisplayName, item, image, await CheckIfItemExistsAsync(item, "3"), await CheckIfItemExistsAsync(item, "4"), await CheckIfItemExistsAsync(item, "5")));
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

    public class Puzzle
    {
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
        }
        private bool _IsPuzzleSize4Available = false;
        public bool IsPuzzleSize4Available
        {
            get { return _IsPuzzleSize4Available; }
        }
        private bool _IsPuzzleSize5Available = false;
        public bool IsPuzzleSize5Available
        {
            get { return _IsPuzzleSize5Available; }
        }

        public Puzzle(string Name, StorageFolder Folder, BitmapImage Image, bool IsPuzzleSize3Available, bool IsPuzzleSize4Available, bool IsPuzzleSize5Available)
        {
            _Name = Name;
            _Folder = Folder;
            _Image = Image;
            _IsPuzzleSize3Available = IsPuzzleSize3Available;
            _IsPuzzleSize4Available = IsPuzzleSize4Available;
            _IsPuzzleSize5Available = IsPuzzleSize5Available;
        }
    }

}
