using System.Windows.Input;

namespace MusicPlayerAPI
{
    public class NavigationItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public double Height { get; set; } = 24;
        public bool CanBeOpened { get; set; }
        public bool CanBeFavorite { get; set; }
        public double FontSize { get; set; } = 12;
        public Cursor CursorType { get; set; } = Cursors.Arrow;
        public string ImageSource { get; set; }
        public bool UseAreYouSureMessageBox { get; set; }
        public string AreYouSureMessageBoxMessage { get; set; }

        private NavigationItem() { }

        public NavigationItem(string name, string path, double height, bool canBeOpened, bool canBeFavorite,
            double fontSize, Cursor cursorType)
        {
            Name = name;
            Path = path;
            Height = height;
            CanBeOpened = canBeOpened;
            CanBeFavorite = canBeFavorite;
            FontSize = fontSize;
            CursorType = cursorType;
        }

        public NavigationItem(string name, string path, double height, bool canBeOpened, bool canBeFavorite,
            double fontSize, Cursor cursorType, string imageSource)
            : this(name, path, height, canBeOpened, canBeFavorite, fontSize, cursorType)
        {
            ImageSource = imageSource;
        }

        public NavigationItem(string name, string path, double height, bool canBeOpened, bool canBeFavorite,
            double fontSize, Cursor cursorType, bool useAreYouSureMessageBox, string areYouSureMessageBoxMessage)
            : this(name, path, height, canBeOpened, canBeFavorite, fontSize, cursorType)
        {
            UseAreYouSureMessageBox = useAreYouSureMessageBox;
            AreYouSureMessageBoxMessage = areYouSureMessageBoxMessage;
        }

        public NavigationItem(string name, string path, double height, bool canBeOpened, bool canBeFavorite,
            double fontSize, Cursor cursorType, string imageSource, bool useAreYouSureMessageBox, string areYouSureMessageBoxMessage)
            : this(name, path, height, canBeOpened, canBeFavorite, fontSize, cursorType, imageSource)
        {
            UseAreYouSureMessageBox = useAreYouSureMessageBox;
            AreYouSureMessageBoxMessage = areYouSureMessageBoxMessage;
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
