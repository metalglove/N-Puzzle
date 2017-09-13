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
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Sliding_Puzzle.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CreatePuzzle : Page
    {
       // public Classes.PuzzleList puzzleList;

        List<WriteableBitmap> images3 = new List<WriteableBitmap>();
        List<WriteableBitmap> images4 = new List<WriteableBitmap>();
        List<WriteableBitmap> images5 = new List<WriteableBitmap>();
        List<WriteableBitmap> images6 = new List<WriteableBitmap>();
        BitmapImage image = new BitmapImage();
        StorageFolder ImageFolder;
        StorageFile imagefile;
        string imagename = "";
        string imagepath = "";
        int size = 0;
        
        public CreatePuzzle()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //puzzleList = e.Parameter as Classes.PuzzleList;
            
            base.OnNavigatedTo(e);
        }
        private async void btPickImage_Click(object sender, RoutedEventArgs e)
        {
            if (images3.Count > 0 && images4.Count > 0 && images5.Count > 0 && images6.Count > 0)
            {
                images3.Clear();
                images4.Clear();
                images5.Clear();
                images6.Clear();
            }
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            imagefile = await picker.PickSingleFileAsync();
            if (imagefile != null)
            {
                imagename = imagefile.DisplayName;
                imagepath = imagefile.Path;
                using (IRandomAccessStream fileStream = await imagefile.OpenAsync(FileAccessMode.Read))
                {
                    images3 = await CutImageInPiecesAsync(fileStream, 3);
                    images4 = await CutImageInPiecesAsync(fileStream, 4);
                    images5 = await CutImageInPiecesAsync(fileStream, 5);
                    images6 = await CutImageInPiecesAsync(fileStream, 6);
                    image.SetSource(fileStream);
                    await SaveIRandomAccessStreamToFileAsync(fileStream, imagename);
                    StorageFolder pictureFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("SlidingPuzzles", CreationCollisionOption.OpenIfExists);
                    ImageFolder = await pictureFolder.GetFolderAsync(imagename);
                    if (Classes.PuzzleList.Instance.Any(puzzle => puzzle.Name != imagename))
                    {
                        Classes.PuzzleList.Instance.Add(new Classes.Puzzle(ImageFolder.DisplayName, ImageFolder, image, false, false, false, false));
                    }
                    imgForPuzzle.Source = image;
                    cbPuzzlesizes.IsEnabled = true;
                }
            }
        }
        private void cbPuzzlesizes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            size = int.Parse((e.AddedItems[0] as ComboBoxItem).Content.ToString());
            CreatePuzzleGridLines(size);
            btCreateCustomPuzzle.IsEnabled = true;
        }
        private void CreatePuzzleGridLines(int puzzlesize)
        {
            if (DrawGrid.Children.Count > 0)
            {
                DrawGrid.Children.Clear();
            }
            int linesSize = 500 / puzzlesize;
            int linesXY = puzzlesize - 1;
            List<Line> XLinesForCanvas = new List<Line>();
            List<Line> YLinesForCanvas = new List<Line>();
            for (int i = 0; i < linesXY; i++)
            {
                XLinesForCanvas.Add(CreateLine(0, 0, 500, 0));
                YLinesForCanvas.Add(CreateLine(0, 500, 0, 0));
            }
            int XLineCount = 1;
            foreach (Line item in XLinesForCanvas)
            {
                Canvas.SetTop(item, linesSize * XLineCount);
                Canvas.SetLeft(item, 0);
                XLineCount++;
            }
            int YLineCount = 1;
            foreach (Line item in YLinesForCanvas)
            {
                Canvas.SetTop(item, 0);
                Canvas.SetLeft(item, linesSize * YLineCount);
                YLineCount++;
            }
            foreach (Line item in XLinesForCanvas)
            {
                DrawGrid.Children.Add(item);
            }
            foreach (Line item in YLinesForCanvas)
            {
                DrawGrid.Children.Add(item);
            }
        }
        private Line CreateLine(double X1, double Y1, double X2, double Y2)
        {
            Line line = new Line();
            line.Stroke = new SolidColorBrush(Colors.Black);
            line.StrokeThickness = 3;
            line.X1 = X1;
            line.Y1 = Y1;
            line.X2 = X2;
            line.Y2 = Y2;
            return line;
        }
        private async void BtCreateCustomPuzzle_ClickAsync(object sender, RoutedEventArgs e)
        {
            bool result = false;
            switch (size)
            {
                case 3:
                    Debug.WriteLine("3");
                    int i3 = 0;
                    images3.ForEach(async image => await SaveBitmapToFileAsync(image, imagename, ++i3, size));
                    if (Classes.PuzzleList.Instance.Any(puzzle => puzzle.Name == imagename))
                        while (result == false) { result = await CheckIfItemExistsAsync(ImageFolder, "3"); }
                        Classes.PuzzleList.Instance.First(puzzle => puzzle.Name == imagename).IsPuzzleSize3Available = result;
                    break;
                case 4:
                    Debug.WriteLine("4");
                    int i4 = 0;
                    images4.ForEach(async image => await SaveBitmapToFileAsync(image, imagename, ++i4, size));
                    if (Classes.PuzzleList.Instance.Any(puzzle => puzzle.Name == imagename))
                        while (result == false) { result = await CheckIfItemExistsAsync(ImageFolder, "4"); }
                        Classes.PuzzleList.Instance.First(puzzle => puzzle.Name == imagename).IsPuzzleSize4Available = result;
                    break;
                case 5:
                    Debug.WriteLine("5");
                    int i5 = 0;
                    images5.ForEach(async image => await SaveBitmapToFileAsync(image, imagename, ++i5, size));
                    if (Classes.PuzzleList.Instance.Any(puzzle => puzzle.Name == imagename))
                        while (result == false) { result = await CheckIfItemExistsAsync(ImageFolder, "5"); }
                        Classes.PuzzleList.Instance.First(puzzle => puzzle.Name == imagename).IsPuzzleSize5Available = result;
                    break;
                case 6:
                    Debug.WriteLine("6");
                    int i6 = 0;
                    images6.ForEach(async image => await SaveBitmapToFileAsync(image, imagename, ++i6, size));
                    if(Classes.PuzzleList.Instance.Any(puzzle => puzzle.Name == imagename))
                        while (result == false) { result = await CheckIfItemExistsAsync(ImageFolder, "6"); }
                        Classes.PuzzleList.Instance.First(puzzle => puzzle.Name == imagename).IsPuzzleSize6Available = result;
                    break;
                default:
                    break;
            }
            ContentDialog dg = new ContentDialog()
            {
                Title = "Puzzle is created",
                Content = "Puzzle has succesfully been created!",
                CloseButtonText = "Ok"
            };
            await dg.ShowAsync();
        }
        private async Task<List<WriteableBitmap>> CutImageInPiecesAsync(IRandomAccessStream filestream, int puzzlesize)
        {
            List<WriteableBitmap> images = new List<WriteableBitmap>();
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(filestream);
            uint x = 0;
            uint y = 0;
            for (int i = 0; i < (puzzlesize * puzzlesize); i++)
            {
                InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
                BitmapEncoder enc = await BitmapEncoder.CreateForTranscodingAsync(ras, decoder);
                enc.BitmapTransform.ScaledHeight = 501;
                enc.BitmapTransform.ScaledWidth = 501;
                BitmapBounds bounds = new BitmapBounds();
                uint size = Convert.ToUInt32(500 / puzzlesize);
                bounds.Height = size;
                bounds.Width = size;
                if (x > 490)
                {
                    x = 0;
                    y = y + size;
                }
                bounds.X = x;
                bounds.Y = y;
                enc.BitmapTransform.Bounds = bounds;
                await enc.FlushAsync();
                var wb = new WriteableBitmap(Convert.ToInt32(size), Convert.ToInt32(size));
                await wb.SetSourceAsync(ras);
                images.Add(wb);
                x = x + size;
            }
            return images;
        }
        public static async Task SaveBitmapToFileAsync(WriteableBitmap image, string imagename, int id, int size)
        {
            StorageFolder pictureFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("SlidingPuzzles", CreationCollisionOption.OpenIfExists);//await StorageFolder.GetFolderFromPathAsync(@"C:\test");//await ApplicationData.Current.LocalFolder.CreateFolderAsync(imagename, CreationCollisionOption.OpenIfExists);
            StorageFolder pictureFolder2 = await pictureFolder.CreateFolderAsync(imagename, CreationCollisionOption.OpenIfExists);
            StorageFolder pictureFolder3 = await pictureFolder2.CreateFolderAsync(size.ToString(), CreationCollisionOption.OpenIfExists);
            var file = await pictureFolder3.CreateFileAsync(id.ToString() + ".png", CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream.AsRandomAccessStream());
                var pixelStream = image.PixelBuffer.AsStream();
                byte[] pixels = new byte[image.PixelBuffer.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)image.PixelWidth, (uint)image.PixelHeight, 96, 96, pixels);
                await encoder.FlushAsync();
            }
        }
        public static async Task SaveIRandomAccessStreamToFileAsync(IRandomAccessStream filestream, string imagename)
        {
            BitmapDecoder decoder = await BitmapDecoder.CreateAsync(filestream);
            InMemoryRandomAccessStream ras = new InMemoryRandomAccessStream();
            BitmapEncoder enc = await BitmapEncoder.CreateForTranscodingAsync(ras, decoder);
            enc.BitmapTransform.ScaledHeight = 500;
            enc.BitmapTransform.ScaledWidth = 500;
            BitmapBounds bounds = new BitmapBounds();
            uint size = Convert.ToUInt32(500);
            bounds.Height = size;
            bounds.Width = size;
            bounds.X = 0;
            bounds.Y = 0;
            enc.BitmapTransform.Bounds = bounds;
            await enc.FlushAsync();
            var wb = new WriteableBitmap(500, 500);
            await wb.SetSourceAsync(ras);
            StorageFolder pictureFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("SlidingPuzzles", CreationCollisionOption.OpenIfExists);//await StorageFolder.GetFolderFromPathAsync(@"C:\test");//await ApplicationData.Current.LocalFolder.CreateFolderAsync(imagename, CreationCollisionOption.OpenIfExists);
            StorageFolder pictureFolder2 = await pictureFolder.CreateFolderAsync(imagename, CreationCollisionOption.OpenIfExists);
            var file = await pictureFolder2.CreateFileAsync(imagename + ".png", CreationCollisionOption.ReplaceExisting);
            using (var stream = await file.OpenStreamForWriteAsync())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream.AsRandomAccessStream());
                var pixelStream = wb.PixelBuffer.AsStream();
                byte[] pixels = new byte[wb.PixelBuffer.Length];
                await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)wb.PixelWidth, (uint)wb.PixelHeight, 96, 96, pixels);
                await encoder.FlushAsync();
            }
        }
        private async Task<bool> CheckIfItemExistsAsync(StorageFolder Folder, string itemName)
        {
            var item = await Folder.TryGetItemAsync(itemName);
            return item != null;
        }
    }
}
