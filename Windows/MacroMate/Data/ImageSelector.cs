using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MacroMate.Data
{
    public class ImageSelector
    {
        private string filesPath;
        private List<string> images;
        private INavigation navigation;

        public ImageSelector(string folderPath, INavigation navigation)
        {
            this.filesPath = folderPath;
            this.navigation = navigation;
            images = GetImagePaths(this.filesPath);
        }

        public async Task<string> ChooseImage()
        {
            TaskCompletionSource<string> imageSelectedTask = new TaskCompletionSource<string>();

            StackLayout stackLayout = new StackLayout
            {
                Padding = new Thickness(20),
                BackgroundColor = Color.FromArgb("F8F4E3")
            };

            for (int i = 0; i < images.Count; i += 4)
            {
                HorizontalStackLayout hsl = new HorizontalStackLayout
                {
                    HorizontalOptions = LayoutOptions.Center
                };

                for (int j = 0; j < 4; j++)
                {
                    int ci = i + j;
                    if (ci < images.Count)
                    {
                        Image image = new Image
                        {
                            Source = ImageSource.FromFile(images[ci]),
                            WidthRequest = 80,
                            HeightRequest = 80,
                            Margin = new Thickness(15)
                        };

                        image.GestureRecognizers.Add(new TapGestureRecognizer
                        {
                            Command = new Command(() =>
                            {
                                if (!imageSelectedTask.Task.IsCompleted)
                                {
                                    imageSelectedTask.TrySetResult(images[ci]);
                                }
                            }),
                            NumberOfTapsRequired = 1
                        });

                        hsl.Children.Add(image);
                    }
                }
                stackLayout.Children.Add(hsl);
            }

            ScrollView scrollView = new ScrollView { Content = stackLayout };

            ContentPage popupPage = new ContentPage
            {
                Content = scrollView,
                Title = "Select an Icon"
            };

            await navigation.PushModalAsync(popupPage);

            var selectedImagePath = await imageSelectedTask.Task;

            await navigation.PopModalAsync();

            return selectedImagePath;
        }

        private List<string> GetImagePaths(string folderPath)
        {
            var imagePaths = Directory.GetFiles(folderPath, "*.png")
                                   .OrderBy(path => path)
                                   .ToList();

            return imagePaths;
        }
    }
}
