using System.Windows.Media;
using System.Windows.Input;

namespace MusicPlayerAPI
{
    public class NavigationItem
    {
        public object Parent { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public double Height { get; set; } = 24;
        public bool CanBeOpened { get; set; }
        public bool CanBeFavorite { get; set; }
        public double FontSize { get; set; } = 12;
        public Brush Foreground { get; set; } = Brushes.Black;
        public Cursor CursorType { get; set; } = Cursors.Arrow;
        public string ImageSource { get; set; }
        public string AddRemoveFavoriteImageSource { get; set; }
        public bool UseAreYouSureMessageBox { get; set; }
        public string AreYouSureMessageBoxMessage { get; set; }

        public NavigationItem() { }

        public NavigationItem(string name, string path, double height, bool canBeOpened, bool canBeFavorite, string addRemoveFavoriteImageSource,
            double fontSize, Brush foreground, Cursor cursorType)
        {
            Name = name;
            Path = path;
            Height = height;
            CanBeOpened = canBeOpened;
            CanBeFavorite = canBeFavorite;
            AddRemoveFavoriteImageSource = addRemoveFavoriteImageSource;
            FontSize = fontSize;
            Foreground = foreground;
            CursorType = cursorType;
        }

        public NavigationItem(string name, string path, double height, bool canBeOpened, bool canBeFavorite, string addRemoveFavoriteImageSource,
            double fontSize, Brush foreground, Cursor cursorType, string imageSource)
            : this(name, path, height, canBeOpened, canBeFavorite, addRemoveFavoriteImageSource, fontSize, foreground, cursorType)
        {
            ImageSource = imageSource;
        }

        public NavigationItem(string name, string path, double height, bool canBeOpened, bool canBeFavorite, string addRemoveFavoriteImageSource,
            double fontSize, Brush foreground, Cursor cursorType, bool useAreYouSureMessageBox, string areYouSureMessageBoxMessage)
            : this(name, path, height, canBeOpened, canBeFavorite, addRemoveFavoriteImageSource, fontSize, foreground, cursorType)
        {
            UseAreYouSureMessageBox = useAreYouSureMessageBox;
            AreYouSureMessageBoxMessage = areYouSureMessageBoxMessage;
        }

        public NavigationItem(string name, string path, double height, bool canBeOpened, bool canBeFavorite, string addRemoveFavoriteImageSource,
            double fontSize, Brush foreground, Cursor cursorType, string imageSource, bool useAreYouSureMessageBox, string areYouSureMessageBoxMessage)
            : this(name, path, height, canBeOpened, canBeFavorite, addRemoveFavoriteImageSource, fontSize, foreground, cursorType, imageSource)
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
