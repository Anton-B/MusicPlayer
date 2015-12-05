namespace MusicPlayerAPI
{
    public class NavigationItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public double Height { get; set; }
        public bool CanBeOpened { get; set; }
        public bool CanBeFavourite { get; set; }
        public string ImageSource { get; set; }

        public NavigationItem() { }

        public NavigationItem(string name, string path, double height, bool canBeOpened, bool canBeFavorite, string imageSource)
        {
            Name = name;
            Path = path;
            Height = height;
            CanBeOpened = canBeOpened;
            CanBeFavourite = canBeFavorite;
            ImageSource = imageSource;
        }
    }
}
