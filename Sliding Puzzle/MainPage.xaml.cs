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
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Puzzle> puzzleList = new ObservableCollection<Puzzle>();
        public int PuzzleSize = 5;
        public MainPage()
        {
            this.InitializeComponent();
            LoadExistingPuzzlesAsync();
            lvPuzzles.ItemsSource = puzzleList;
        }

        private void Puzzle_Click(object sender, RoutedEventArgs e)
        {
            Button send = sender as Button;
            this.ContentFrame.Navigate(typeof(Views.SlidingPuzzle), new Classes.SlidingPuzzle(PuzzleSize, send.Tag.ToString()));
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
                BitmapImage image = await GetImageFromStorageFolderAsync(item);
                puzzleList.Add(new Puzzle(item.DisplayName, item, image));
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

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Image image = sender as Image;
            StorageFolder file = (StorageFolder)image.Tag;
            this.ContentFrame.Navigate(typeof(Views.SlidingPuzzle), new Classes.SlidingPuzzle(5, file, file.DisplayName));
        }
    }

    public class Puzzle
    {
        private string _Name;
        public string Name { get { return _Name; } }
        private StorageFolder _Folder;
        public StorageFolder Folder { get { return _Folder; } }
        private BitmapImage _Image;
        public BitmapImage Image { get { return _Image; } }
        public Puzzle(string Name, StorageFolder Folder, BitmapImage Image)
        {
            _Name = Name;
            _Folder = Folder;
            _Image = Image;
        }
    }

}
