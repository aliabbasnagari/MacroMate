namespace MacroMate.View;

public partial class ChooseImage
{
	public ChooseImage()
	{
		InitializeComponent();
        LoadImages();
	}

    private void btnClose_Clicked(object sender, EventArgs e)
    {
        Close();
    }

    private void LoadImages()
    {
        string path = "C:\\Users\\Ali Abbas\\Documents\\MacroMate\\Icons";
        var images = GetImagePaths(path);
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
                        WidthRequest = 40,
                        HeightRequest = 40,
                        Margin = new Thickness(15)
                    };

                    image.GestureRecognizers.Add(new TapGestureRecognizer
                    {
                        Command = new Command(() =>
                        {
                            Close(images[ci]);
                        })
                    });

                    hsl.Children.Add(image);
                }
            }
            vSL.Children.Add(hsl);
        }
    }

    private List<string> GetImagePaths(string folderPath)
    {
        var imagePaths = Directory.GetFiles(folderPath, "*.png").OrderBy(path => path).ToList();
        return imagePaths;
    }
}