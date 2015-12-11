namespace MusicPlayerAPI
{
    public class NavigationItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public double Height { get; set; }
        public bool CanBeOpened { get; set; }
        public bool CanBeFavorite { get; set; }
        public string ImageSource { get; set; }

        public NavigationItem() { }

        public NavigationItem(string name, string path, double height, bool canBeOpened, bool canBeFavorite, string imageSource)
        {
            Name = name;
            Path = path;
            Height = height;
            CanBeOpened = canBeOpened;
            CanBeFavorite = canBeFavorite;
            ImageSource = imageSource;
        }

        public override bool Equals(object obj)
        {
            return ((NavigationItem)obj).Path == this.Path;
        }

        public override int GetHashCode()
        {
            int hashNavigationItemName = Name == null ? 0 : Name.GetHashCode();
            int hashNavigationItemPath = Path == null ? 0 : Path.GetHashCode();
            return hashNavigationItemName ^ hashNavigationItemPath;
        }
    }
}
