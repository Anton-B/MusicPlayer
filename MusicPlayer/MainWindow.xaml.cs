using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Media;
using System.Reflection;
using System.Xml;
using System.Configuration;
using MusicPlayerAPI;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using FMUtils.KeyboardHook;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SongsManager songsManager = new SongsManager();
        private DispatcherTimer timer = new DispatcherTimer();
        private PluginsManager pluginsManager = new PluginsManager();
        private DirectoryInfo pluginsDirectory = new DirectoryInfo(@"Plugins\");
        private Image contentButtonImage;
        private bool isPlaying;
        private DataGrid busyDataGrid
        {
            get
            {
                if (playlist1DataGrid.SelectedIndex == -1 && playlist2DataGrid.SelectedIndex == -1)
                    return visibDataGrid;
                return (playlist1DataGrid.SelectedIndex != -1) ? playlist1DataGrid : playlist2DataGrid;
            }
        }
        private DataGrid freeDataGrid { get { return (playlist1DataGrid.SelectedIndex == -1) ? playlist1DataGrid : playlist2DataGrid; } }
        private DataGrid visibDataGrid { get { return (playlist1DataGrid.Visibility == Visibility.Visible) ? playlist1DataGrid : playlist2DataGrid; } }
        private int busyDGSortIndex;
        private bool playlist1SongsMixed;
        private bool playlist2SongsMixed;
        private bool visibSongsMixed
        {
            get { return visibDataGrid == playlist1DataGrid ? playlist1SongsMixed : playlist2SongsMixed; }
            set
            {
                if (visibDataGrid == playlist1DataGrid)
                    playlist1SongsMixed = value;
                else
                    playlist2SongsMixed = value;
            }
        }
        private RepeatType repeatType;
        private ListView currentListView { get { return (navigTabControl.SelectedIndex == 0) ? navigListView : favoritesListView; } }
        private const int numOfSongsToAddWhenScrolling = 5;
        private Song[] playlist1Songs;
        private Song[] playlist2Songs;
        private Song[] visibleSongs
        {
            get { return visibDataGrid == playlist1DataGrid ? playlist1Songs : playlist2Songs; }
            set
            {
                if (visibDataGrid == playlist1DataGrid)
                    playlist1Songs = value;
                else
                    playlist2Songs = value;
            }

        }
        private Song[] busySongs
        {
            get { return busyDataGrid == playlist1DataGrid ? playlist1Songs : playlist2Songs; }
            set
            {
                if (busyDataGrid == playlist1DataGrid)
                    playlist1Songs = value;
                else
                    playlist2Songs = value;
            }
        }
        private bool songsListBlocked = false;
        private bool dragStarted = false;
        private Hook keyboardHook = new Hook("Media Keys Hook");
        private bool useDarkTheme;

        #region WINDOW
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            keyboardHook.KeyDownEvent += MediaKeyDown;
            contentButtonImage = (Image)this.Resources["contentButtonImage"];
            playlist1DataGrid.Tag = playlist1Songs;
            playlist2DataGrid.Tag = playlist2Songs;
            pluginsManager.LoadPlugin(pluginsDirectory);
            SetModeComboBoxItems();
            try { LoadSettings(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Ошибка чтения конфигурационного файла", MessageBoxButton.OK, MessageBoxImage.Error); }
            if (modeComboBox.SelectedIndex == -1 && modeComboBox.Items.Count > 0)
                modeComboBox.SelectedIndex = 0;
            if (modeComboBox.Items.Count > 1)
                modeStackPanel.Visibility = Visibility.Visible;
        }

        private void themeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (themeComboBox.SelectedIndex == -1)
                return;
            useDarkTheme = (themeComboBox.SelectedIndex == 0) ? false : true;
            SetTheme();
        }

        private void SetTheme()
        {
            var bc = new BrushConverter();
            if (useDarkTheme == true)
            {
                this.Background = GetBrush(bc, "#FF1E1E1E");
                this.Resources["TextBrush"] = GetBrush(bc, "#FFE6E6E6");
                this.Resources["ElementsBaseBrush"] = GetBrush(bc, "#FF68768A");
                this.Resources["ActiveTextBrush"] = GetBrush(bc, "#FF9BA7B8");
                this.Resources["ActiveTextMouseOverBrush"] = GetBrush(bc, "#FF68768A");
                this.Resources["ActiveTextMousePressedBrush"] = GetBrush(bc, "#FF56657C");
                this.Resources["SongCellBorderBrush"] = GetBrush(bc, "#334D7FA4");
                this.Resources["SongCellBackgroundMouseOverBrush"] = GetBrush(bc, "#334D7FA4");
                this.Resources["SongCellForegroundBrush"] = GetBrush(bc, "#FFE6E6E6");
                this.Resources["SongCellForegroundSelectedBrush"] = GetBrush(bc, "#FFE6E6E6");
                this.Resources["SearchInactiveForegroundBrush"] = GetBrush(bc, "Gray");
                this.Resources["SearchBorderBrush"] = GetBrush(bc, "#FF9BA7B8");
                this.Resources["MenuBackgroundBrush"] = GetBrush(bc, "#FF1E1E1E");
                this.Resources["MenuForegroundBrush"] = GetBrush(bc, "#FFE6E6E6");
                this.Resources["MenuForegroundMouseOverBrush"] = GetBrush(bc, "#FFE6E6E6");
                this.Resources["FavoritesButtonBackgroundBrush"] = GetBrush(bc, "#FF1E1E1E");
                this.Resources["FavoritesButtonBorderBrush"] = GetBrush(bc, "#FF1E1E1E");
                this.Resources["NavigItemForegroundBrush"] = GetBrush(bc, "#FF97A0AC");
                this.Resources["NavigItemForegroundMouseOverBrush"] = GetBrush(bc, "#FFE6E6E6");
                this.Resources["TabItemForegroundSelectedBrush"] = GetBrush(bc, "#FFE6E6E6");
                this.Resources["ScrollBarBackgroundBrush"] = GetBrush(bc, "#FF3E3E42");
                this.Resources["ScrollBarRepeatMouseOverBrush"] = GetBrush(bc, "#FFA6A6A6");
                this.Resources["ScrollBarRepeatMousePressedBrush"] = GetBrush(bc, "#FFEFEBEF");
                this.Resources["ScrollBarRepeatPathBrush"] = GetBrush(bc, "#FFA6A6A6");
                this.Resources["ScrollBarRepeatPathMouseOverBrush"] = GetBrush(bc, "Black");
                this.Resources["ScrollBarThumbBrush"] = GetBrush(bc, "#FF686868");
                this.Resources["ScrollBarThumbMouseOverBrush"] = GetBrush(bc, "#FF9E9E9E");

                this.Resources["PlayerButtonMouseOverBrush"] = GetBrush(bc, "#FF52729C");
                this.Resources["PlayerButtonMouseDownBrush"] = GetBrush(bc, "#FF425C81");
                this.Resources["PlayerBackgroundBrush"] = GetBrush(bc, "#FF50687A");
                this.Resources["PlayerSliderBackgroundBrush"] = GetBrush(bc, "LightGray");
                this.Resources["PlayerSliderBackgroundFilledBrush"] = GetBrush(bc, "White");
                this.Resources["PlayerSliderThumbStrokeBrush"] = GetBrush(bc, "White");
                this.Resources["PlayerSongInfoForegroundBrush"] = GetBrush(bc, "White");
            }
            else
            {
                this.Background = GetBrush(bc, "White");
                this.Resources["TextBrush"] = GetBrush(bc, "#FF787878");
                this.Resources["ElementsBaseBrush"] = GetBrush(bc, "#FF4D7FA4");
                this.Resources["ActiveTextBrush"] = GetBrush(bc, "#FF2B587A");
                this.Resources["ActiveTextMouseOverBrush"] = GetBrush(bc, "#FF5585A8");
                this.Resources["ActiveTextMousePressedBrush"] = GetBrush(bc, "#FF76A2C1");
                this.Resources["SongCellBorderBrush"] = GetBrush(bc, "#334D7FA4");
                this.Resources["SongCellBackgroundMouseOverBrush"] = GetBrush(bc, "#334D7FA4");
                this.Resources["SongCellForegroundBrush"] = GetBrush(bc, "#FF2B587A");
                this.Resources["SongCellForegroundSelectedBrush"] = GetBrush(bc, "White");
                this.Resources["SearchInactiveForegroundBrush"] = GetBrush(bc, "Gray");
                this.Resources["SearchBorderBrush"] = GetBrush(bc, "#FF819FB4");
                this.Resources["MenuBackgroundBrush"] = GetBrush(bc, "White");
                this.Resources["MenuForegroundBrush"] = GetBrush(bc, "#FF2B587A");
                this.Resources["MenuForegroundMouseOverBrush"] = GetBrush(bc, "White");
                this.Resources["FavoritesButtonBackgroundBrush"] = GetBrush(bc, "White");
                this.Resources["FavoritesButtonBorderBrush"] = GetBrush(bc, "White");
                this.Resources["NavigItemForegroundBrush"] = GetBrush(bc, "#FF2B587A");
                this.Resources["NavigItemForegroundMouseOverBrush"] = GetBrush(bc, "White");
                this.Resources["TabItemForegroundSelectedBrush"] = GetBrush(bc, "White");
                this.Resources["ScrollBarBackgroundBrush"] = GetBrush(bc, "#FFF0F0F0");
                this.Resources["ScrollBarRepeatMouseOverBrush"] = GetBrush(bc, "#FFA6A6A6");
                this.Resources["ScrollBarRepeatMousePressedBrush"] = GetBrush(bc, "#FF606060");
                this.Resources["ScrollBarRepeatPathBrush"] = GetBrush(bc, "#FFA6A6A6");
                this.Resources["ScrollBarRepeatPathMouseOverBrush"] = GetBrush(bc, "Black");
                this.Resources["ScrollBarThumbBrush"] = GetBrush(bc, "#FFCDCDCD");
                this.Resources["ScrollBarThumbMouseOverBrush"] = GetBrush(bc, "#FF919191");

                this.Resources["PlayerButtonMouseOverBrush"] = GetBrush(bc, "#FF52729C");
                this.Resources["PlayerButtonMouseDownBrush"] = GetBrush(bc, "#FF425C81");
                this.Resources["PlayerBackgroundBrush"] = GetBrush(bc, "#FF50687A");
                this.Resources["PlayerSliderBackgroundBrush"] = GetBrush(bc, "LightGray");
                this.Resources["PlayerSliderBackgroundFilledBrush"] = GetBrush(bc, "White");
                this.Resources["PlayerSliderThumbStrokeBrush"] = GetBrush(bc, "White");
                this.Resources["PlayerSongInfoForegroundBrush"] = GetBrush(bc, "White");
            }
            pluginsManager.SetThemeSettings(useDarkTheme);
        }

        private void SetDarkTheme()
        {
            var bc = new BrushConverter();

        }

        private Brush GetBrush(BrushConverter converter, string brushStr)
        {
            return (Brush)converter.ConvertFromString(brushStr);
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }
        #endregion

        #region PLAYER CORE
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!dragStarted)
                musicTimelineSlider.Value = mediaElement.Position.TotalMilliseconds;
            currTimelinePosLabel.Content = TimeFormatter.Format((int)TimeSpan.FromMilliseconds(musicTimelineSlider.Value).TotalMinutes)
                + ":" + TimeFormatter.Format(TimeSpan.FromMilliseconds(musicTimelineSlider.Value).Seconds);
        }

        private void MediaKeyDown(KeyboardHookEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Forms.Keys.MediaPreviousTrack:
                    prevButton_Click(null, new EventArgs());
                    break;
                case System.Windows.Forms.Keys.MediaPlayPause:
                    playPauseButton_Click(null, new EventArgs());
                    break;
                case System.Windows.Forms.Keys.MediaNextTrack:
                    nextButton_Click(null, new EventArgs());
                    break;
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            try
            {
                musicTimelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
                Song currentSong = busyDataGrid.SelectedItem as Song;
                if (currentSong != null)
                {
                    artistLabel.Content = currentSong.Artist;
                    titleLabel.Content = "-  " + currentSong.Title;
                    maxTimelinePosLabel.Content = currentSong.Duration;
                }
            }
            catch { }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void playPauseButton_Click(object sender, EventArgs e)
        {
            if (isPlaying)
                Pause();
            else
                Play();
        }

        private void prevButton_Click(object sender, EventArgs e)
        {
            Prev();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            RepeatType temp = repeatType;
            repeatType = RepeatType.NoRepeat;
            Next();
            repeatType = temp;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement != null)
                mediaElement.Volume = volumeSlider.Value;
        }

        private void slider_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var slider = (Slider)sender;
                Point position = e.GetPosition(slider);
                double d = 1.0d / slider.ActualWidth * position.X;
                var p = slider.Maximum * d;
                slider.Value = p;
                if (slider.Name == "musicTimelineSlider")
                    dragStarted = true;
            }
            else if (dragStarted)
                UpdateMediaElement();
        }

        private void slider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dragStarted == true)
                UpdateMediaElement();
        }

        private void slider_MouseLeave(object sender, MouseEventArgs e)
        {
            if (dragStarted == true)
                UpdateMediaElement();
        }

        private void sliderThumb_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            UpdateMediaElement();
        }

        private void UpdateMediaElement()
        {
            dragStarted = false;
            mediaElement.Position = TimeSpan.FromMilliseconds(musicTimelineSlider.Value);
        }

        private void Rand_Click(object sender, RoutedEventArgs e)
        {
            visibSongsMixed = !visibSongsMixed;
            ((Image)randButton.Content).Source = visibSongsMixed ? new BitmapImage(new Uri(@"pack://application:,,,/Images/rand_active.png"))
                : new BitmapImage(new Uri(@"pack://application:,,,/Images/rand.png"));
            SetSongsList(visibleSongs, true, true, false);
            if (visibDataGrid == busyDataGrid)
                visibDataGrid.ScrollIntoView(visibDataGrid.SelectedItem);
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            switch (repeatType)
            {
                case RepeatType.NoRepeat:
                    repeatType = RepeatType.RepeatSong;
                    ((Image)repeatButton.Content).Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/repeat_one.png"));
                    break;
                case RepeatType.RepeatSong:
                    repeatType = RepeatType.RepeatList;
                    ((Image)repeatButton.Content).Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/repeat_all.png"));
                    break;
                case RepeatType.RepeatList:
                    repeatType = RepeatType.NoRepeat;
                    ((Image)repeatButton.Content).Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/repeat.png"));
                    break;
            }
        }

        private void currentListButton_Click(object sender, RoutedEventArgs e)
        {
            ShowBusyPlaylist();
        }

        private async void songMenuButton_Click(object sender, RoutedEventArgs e)
        {
            var buttonImg = (Image)songMenuButton.Content;
            var rt = new RotateTransform();
            rt.CenterX = buttonImg.ActualWidth / 2;
            rt.CenterY = buttonImg.ActualHeight / 2;
            buttonImg.RenderTransform = rt;
            var dblAnim = new DoubleAnimation(0, 360, new Duration(new TimeSpan(0, 0, 0, 2)), FillBehavior.HoldEnd);
            dblAnim.RepeatBehavior = RepeatBehavior.Forever;
            rt.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(0, 360, new Duration(new TimeSpan(0, 0, 0, 2)), FillBehavior.HoldEnd));
            var contextMenu = new ContextMenu();
            contextMenu.Style = (Style)this.Resources["contextMenuStyle"];
            contextMenu.PlacementTarget = songMenuButton;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
            if (busyDataGrid.SelectedIndex == -1)
                return;
            var menuItems = await pluginsManager.GetSongMenuItems((Song)busyDataGrid.SelectedItem);
            if (menuItems == null)
                return;
            foreach (var itemText in menuItems)
            {
                MenuItem item = new MenuItem();
                item.Style = (Style)this.Resources["contextMenuItemStyle"];
                item.InputGestureText = itemText;
                item.Height = 30;
                item.Click += ContextMenuItem_Click;
                contextMenu.Items.Add(item);
            }
            contextMenu.IsOpen = true;
            buttonImg.RenderTransform = null;
        }

        private async void ContextMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (busyDataGrid.SelectedIndex == -1)
                return;
            var currSong = (Song)busyDataGrid.SelectedItem;
            var updBehavior = await pluginsManager.HandleMenuItemClick(((MenuItem)sender).InputGestureText, currSong);
            switch (updBehavior)
            {
                case UpdateBehavior.NoUpdate:
                    return;
                case UpdateBehavior.UpdateNavigationList:
                    mediaElement.Source = null;
                    ShowItems(addressTextBox.Tag?.ToString(), false);
                    break;
                case UpdateBehavior.UpdateSongsList:
                    mediaElement.Source = null;
                    SetSongsList(visibleSongs, true, true, true);
                    break;
                case UpdateBehavior.UpdateAll:
                    mediaElement.Source = null;
                    ShowItems(addressTextBox.Tag?.ToString(), false);
                    SetSongsList(visibleSongs, true, true, true);
                    break;
                case UpdateBehavior.DeleteSongFromList:
                    DeleteSong(currSong);
                    break;
                case UpdateBehavior.DeleteSongAndUpdateAll:
                    DeleteSong(currSong);
                    ShowItems(addressTextBox.Tag?.ToString(), false);
                    break;
            }
        }

        private void DeleteSong(Song song)
        {
            mediaElement.Source = null;
            var dataGrid = busyDataGrid;
            busyDataGrid.SelectedIndex = -1;
            var songs = playlist1Songs.ToList();
            songs.Remove(song);
            playlist1Songs = songs.ToArray();
            songs = playlist2Songs.ToList();
            songs.Remove(song);
            playlist2Songs = songs.ToArray();
            if (dataGrid.Items.Count > 0)
                dataGrid.SelectedIndex = 0;
            if (visibDataGrid == busyDataGrid)
            {
                busyDataGrid.SelectedIndex = -1;
                SetSongsList(busySongs, true, true, true);
            }
            else
            {
                var vSongs = visibleSongs;
                SetSongsList(busySongs, true, true, true);
                SetSongsList(vSongs, true, false, false);
            }
        }

        private void volumeButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.IsMuted = !mediaElement.IsMuted;
            ((Image)volumeButton.Content).Source = (mediaElement.IsMuted) ? new BitmapImage(new Uri(@"pack://application:,,,/Images/volume_off.png"))
                : new BitmapImage(new Uri(@"pack://application:,,,/Images/volume_on.png"));
        }

        private void InitializeTimer()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Play()
        {
            mediaElement.Play();
            ((Image)playPauseButton.Content).Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/pause.png"));
            taskbarPlayPauseButton.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Images/pause_taskbar.png"));
            taskbarPlayPauseButton.Description = "Пауза";
            isPlaying = true;
        }

        private void Pause()
        {
            mediaElement.Pause();
            ((Image)playPauseButton.Content).Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/play.png"));
            taskbarPlayPauseButton.ImageSource = new BitmapImage(new Uri(@"pack://application:,,,/Images/play_taskbar.png"));
            taskbarPlayPauseButton.Description = "Воспроизвести";
            isPlaying = false;
        }

        private void Prev()
        {
            if (busyDataGrid.SelectedIndex > 0)
                busyDataGrid.SelectedIndex--;
            if (busyDataGrid.SelectedItem != null)
                busyDataGrid.ScrollIntoView(busyDataGrid.SelectedItem);
        }

        private void Next()
        {
            if (busyDataGrid.SelectedItem == null)
                return;
            if (repeatType == RepeatType.RepeatSong)
                mediaElement.Position = new TimeSpan(0, 0, 0, 0);
            else
            {
                if (busyDataGrid.SelectedIndex != busyDataGrid.Items.Count - 1)
                    busyDataGrid.SelectedIndex++;
                else if (repeatType == RepeatType.RepeatList)
                    busyDataGrid.SelectedIndex = 0;
            }
            Play();
            busyDataGrid.ScrollIntoView(busyDataGrid.SelectedItem);
        }
        #endregion

        #region WORKING WITH PLUGINS & NAVIGATION
        private void SetModeComboBoxItems()
        {
            foreach (var kvp in pluginsManager.PluginInstasnces)
            {
                TextBlock tb = new TextBlock();
                tb.Text = kvp.Value.Name;
                tb.Tag = kvp.Key;
                modeComboBox.Items.Add(tb);
            }
        }

        private async void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mediaElement.Close();
            mediaElement.Source = null;
            ClearControls();
            addressTextBox.Tag = null;
            addressTextBox.Text = string.Empty;
            pluginsManager.Key = ((TextBlock)modeComboBox.SelectedItem).Tag.ToString();
            for (int i = 0; i < navigTabControl.Items.Count; i++)
                ((TabItem)navigTabControl.Items[i]).Header = pluginsManager.GetHeader(i);
            songMenuButton.Visibility = (pluginsManager.SupportsSongMenuButton) ? Visibility.Visible : Visibility.Hidden;
            navigTabControl.SelectedIndex = pluginsManager.OpenedTabIndex;
            navigProgressBar.IsIndeterminate = true;
            await ShowItems(addressTextBox.Tag?.ToString(), true);
            navigProgressBar.IsIndeterminate = false;
            songsProgressBar.IsIndeterminate = true;
            SetSongsList(await pluginsManager.GetDefaultSongsList(), true, true, true);
            songsProgressBar.IsIndeterminate = false;
            pluginsManager.SetThemeSettings(useDarkTheme);
        }

        private async void NavigTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pluginsManager.OpenedTabIndex != navigTabControl.SelectedIndex)
            {
                pluginsManager.OpenedTabIndex = (pluginsManager.OpenedTabIndex == 0) ? 1 : 0;
                if (pluginsManager.OpenedTabIndex == 0)
                    await ShowNavigationItems(addressTextBox.Tag?.ToString(), false);
                else
                    ShowFavoriteItems();
            }
        }

        private void NavigFavListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && pluginsManager.DoubleClickToOpenItem)
                currentListView.SelectedIndex = -1;
        }

        private void NavigListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && pluginsManager.DoubleClickToOpenItem)
                OpenItem(((ListView)sender).SelectedItem as NavigationItem);
        }

        private void NavigListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && !pluginsManager.DoubleClickToOpenItem)
                OpenItem((sender as Border)?.DataContext as NavigationItem);
        }

        private async void OpenItem(NavigationItem item)
        {
            if (item == null)
                return;
            if (item.UseAreYouSureMessageBox)
            {
                var answer = MessageBox.Show(this, item.AreYouSureMessageBoxMessage, item.Name, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (answer == MessageBoxResult.No)
                {
                    ((ListView)item.Parent).SelectedIndex = -1;
                    return;
                }
            }
            if (item != null && item.CanBeOpened)
                await ShowItems(item.Path, true);
            else if (item != null && !pluginsManager.UpdatePlaylistWhenFavoritesChanges)
            {
                songsProgressBar.IsIndeterminate = true;
                SetSongsList(await pluginsManager.GetSongsList(item), true, false, (mediaElement.Source == null) ? true : false);
                songsProgressBar.IsIndeterminate = false;
            }
        }

        private async Task ShowItems(string path, bool isScrollToUp)
        {
            await ShowNavigationItems(path, isScrollToUp);
            ShowFavoriteItems();
        }

        private async Task ShowNavigationItems(string path, bool isScrollToUp)
        {
            navigProgressBar.IsIndeterminate = true;
            var count = navigListView.Items.Count;
            for (int i = 0; i < count; i++)
                navigListView.Items.RemoveAt(0);
            List<NavigationItem> navItems = await pluginsManager.GetNavigationItems(path);
            if (navItems == null)
                return;
            navigProgressBar.IsIndeterminate = false;
            addressTextBox.Tag = (navItems == null) ? addressTextBox.Tag?.ToString() : path;
            addressTextBox.Text = (addressTextBox.Tag == null) ? string.Empty : addressTextBox.Tag.ToString();
            if (navItems == null)
                navItems = await pluginsManager.GetNavigationItems(addressTextBox.Tag?.ToString());
            foreach (var ni in navItems)
                ShowNavigationItem(navigListView, ni);

            if (navigListView.Items.Count > 0 && isScrollToUp)
                navigListView.ScrollIntoView(navigListView.Items[0]);
        }

        private void ShowFavoriteItems()
        {
            var count = favoritesListView.Items.Count;
            for (int i = 0; i < count; i++)
                favoritesListView.Items.RemoveAt(0);
            List<NavigationItem> favItems = pluginsManager.GetFavoriteItems();
            if (navigTabControl.SelectedIndex == 1)
                addressTextBox.Text = pluginsManager.GetHeader(1);
            foreach (var ni in favItems)
                ShowNavigationItem(favoritesListView, ni);
            if (favoritesListView.Items.Count > 0)
                favoritesListView.ScrollIntoView(favoritesListView.Items[0]);
        }

        private void ShowNavigationItem(ListView listView, NavigationItem ni)
        {
            ni.Parent = listView;
            listView.Items.Add(ni);
        }

        private void NavigationDockPanel_MouseEnterLeave(object sender, MouseEventArgs e)
        {
            DockPanel outerDp = sender is Border ? (DockPanel)((Border)sender).Child : (DockPanel)sender;
            if (outerDp == null)
                return;
            if (outerDp.Children.Count == 1 && outerDp.Children[0] as Image == null)
                return;
            var button = (Button)outerDp.Children[0];
            if (e.RoutedEvent.Name == "MouseEnter")
                button.Visibility = Visibility.Visible;
            else if (e.RoutedEvent.Name == "MouseLeave")
                button.Visibility = Visibility.Hidden;
        }

        private async void ChangeFavorites(object sender, RoutedEventArgs e)
        {
            NavigationItem ni = ((Button)sender).DataContext as NavigationItem;
            if (pluginsManager.IsFavorite(ni))
            {
                pluginsManager.DeleteFromFavorites(ni);
                ((Image)((Button)sender).Content).Source = new BitmapImage(new Uri(string.Format(@"{0}\{1}", Environment.CurrentDirectory, pluginsManager.AddButtonImageSource), UriKind.RelativeOrAbsolute));
                await ShowItems(addressTextBox.Tag?.ToString(), false);
            }
            else
            {
                pluginsManager.AddToFavorites(ni);
                ((Image)((Button)sender).Content).Source = new BitmapImage(new Uri(string.Format(@"{0}\{1}", Environment.CurrentDirectory, pluginsManager.DeleteButtonImageSource), UriKind.RelativeOrAbsolute));
            }
            var btnImage = ((Button)sender).Content as Image;
            if (pluginsManager.UpdatePlaylistWhenFavoritesChanges)
            {
                mediaElement.Close();
                mediaElement.Source = null;
                songsProgressBar.IsIndeterminate = true;
                SetSongsList(await pluginsManager.GetDefaultSongsList(), true, false, (mediaElement.Source == null) ? true : false);
                songsProgressBar.IsIndeterminate = false;
                if (visibDataGrid.Items.Count == 0)
                    ClearControls();
            }
        }
        #endregion

        #region WORKING WITH SONGS
        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = searchTextBox.Text == "Поиск..." && searchTextBox.Foreground.ToString()
                == this.Resources["SearchInactiveForegroundBrush"].ToString() ? string.Empty : searchTextBox.Text;
            searchTextBox.Foreground = (Brush)this.Resources["ActiveTextBrush"];
        }

        private void searchTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            searchTextBox.Foreground = searchTextBox.Text == string.Empty ? (Brush)this.Resources["SearchInactiveForegroundBrush"] : (Brush)this.Resources["ActiveTextBrush"];
            searchTextBox.Text = searchTextBox.Text == string.Empty ? "Поиск..." : searchTextBox.Text;
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            visibSongsMixed = false;
            ((Image)randButton.Content).Source = new BitmapImage(new Uri(@"pack://application:,,,/Images/rand.png"));
            if (sortComboBox.SelectedIndex == -1)
                return;
            if (visibDataGrid == busyDataGrid)
                busyDGSortIndex = sortComboBox.SelectedIndex;
            SetSongsList(visibleSongs, true, true, false);
            if (visibDataGrid != busyDataGrid)
                visibDataGrid.SelectedIndex = -1;
        }

        private void playlistDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is DataGrid) || ((DataGrid)sender).SelectedIndex == -1)
                return;
            if ((DataGrid)sender == visibDataGrid)
            {
                if (busyDataGrid != visibDataGrid)
                {
                    busyDataGrid.SelectedIndex = -1;
                    busyDGSortIndex = sortComboBox.SelectedIndex;
                }
            }
            else
                visibDataGrid.SelectedIndex = -1;
            if (busyDataGrid.SelectedIndex == -1)
                return;
            SetMediaElementSource();
            Play();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            searchTextBox.Focus();
            var input = e as KeyEventArgs;
            if (input != null && input.Key != Key.Enter || searchTextBox.Text == string.Empty)
                return;
            List<Song> resultSongs = new List<Song>();
            if (!pluginsManager.UseDefaultSearch)
            {
                var response = await pluginsManager.GetSearchResponse(searchTextBox.Text);
                if (response == null)
                    return;
                resultSongs.AddRange(response);
            }
            else
            {
                var songs = await pluginsManager.GetMyMusicSongs();
                if (songs == null)
                    return;
                foreach (var currentSong in songs)
                    if (currentSong.Title.ToLower().Contains(searchTextBox.Text.ToLower())
                        || currentSong.Artist.ToLower().Contains(searchTextBox.Text.ToLower()))
                        resultSongs.Add(currentSong);
            }
            songsProgressBar.IsIndeterminate = true;
            SetSongsList(resultSongs.ToArray(), pluginsManager.SortSearchResults, false, (mediaElement.Source == null) ? true : false);
            songsProgressBar.IsIndeterminate = false;
            if (visibDataGrid.Items.Count > 0)
                visibDataGrid.ScrollIntoView(visibDataGrid.Items[0]);
        }

        private async void myMusicButton_Click(object sender, RoutedEventArgs e)
        {
            songsProgressBar.IsIndeterminate = true;
            SetSongsList(await pluginsManager.GetMyMusicSongs(), true, false, false);
            songsProgressBar.IsIndeterminate = false;
        }

        private void ShowBusyPlaylist()
        {
            visibDataGrid.Visibility = Visibility.Collapsed;
            busyDataGrid.Visibility = Visibility.Visible;
            if (!visibSongsMixed)
                sortComboBox.SelectedIndex = busyDGSortIndex;
            ((Image)randButton.Content).Source = visibSongsMixed ? new BitmapImage(new Uri(@"pack://application:,,,/Images/rand_active.png"))
                : new BitmapImage(new Uri(@"pack://application:,,,/Images/rand.png"));
            numOfAudioTextBlock.Content = string.Format("Количество песен в списке: {0}", visibleSongs.Length);
        }

        private void SetSongsList(Song[] list, bool sortList, bool changeCurrentPlaylist, bool moveToFirstSong)
        {
            if (list == null || songsListBlocked)
                return;
            songsListBlocked = true;
            if (!changeCurrentPlaylist)
            {
                if (songsManager.SortSongs(visibleSongs, 0)
                .SequenceEqual(songsManager.SortSongs(list, 0), new Song() as IEqualityComparer<Song>))
                {
                    songsListBlocked = false;
                    return;
                }
                Song[] busyDataGridSongs;
                if (busyDataGrid == playlist1DataGrid)
                    busyDataGridSongs = playlist1Songs;
                else
                    busyDataGridSongs = playlist2Songs;
                if (visibDataGrid != busyDataGrid && songsManager.SortSongs(busyDataGridSongs, 0)
                    .SequenceEqual(songsManager.SortSongs(list, 0), new Song() as IEqualityComparer<Song>))
                {
                    ShowBusyPlaylist();
                    songsListBlocked = false;
                    return;
                }
                busyDataGrid.Visibility = Visibility.Collapsed;
                freeDataGrid.Visibility = Visibility.Visible;
                busyDGSortIndex = sortComboBox.SelectedIndex;
                ((Image)randButton.Content).Source = visibSongsMixed ? new BitmapImage(new Uri(@"pack://application:,,,/Images/rand_active.png"))
                : new BitmapImage(new Uri(@"pack://application:,,,/Images/rand.png"));
            }
            if (list == null || list.Length == 0)
            {
                visibleSongs = new Song[0];
                visibDataGrid.Items.Clear();
            }
            else
            {
                Song currSong = new Song();
                if (visibDataGrid == busyDataGrid && visibDataGrid.SelectedIndex != -1
                    && (visibSongsMixed == true || sortList))
                {
                    currSong = (Song)visibDataGrid.SelectedItem;
                    int count = visibDataGrid.Items.Count;
                    int i = 0;
                    while (visibDataGrid.Items.Count != 1)
                    {
                        if (((Song)visibDataGrid.Items[i]).Path != currSong.Path)
                            visibDataGrid.Items.RemoveAt(i);
                        else
                            i++;
                    }
                    var songsList = list.ToList();
                    songsList.Remove(currSong);
                    list = songsList.ToArray();
                }
                else
                    visibDataGrid.Items.Clear();

                Song[] songs = (visibSongsMixed) ? songsManager.MixSongs(list) : (sortList) ?
                    songsManager.SortSongs(list, sortComboBox.SelectedIndex) : list;
                var newSongsList = new List<Song>();
                if (currSong.Path != null)
                    newSongsList.Add(currSong);
                newSongsList.AddRange(songs);
                visibleSongs = newSongsList.ToArray();
                for (int i = 0; i < songs.Length && i < (currSong.Path != null ? numOfSongsToAddWhenScrolling - 1 : numOfSongsToAddWhenScrolling); i++)
                    visibDataGrid.Items.Add(songs[i]);
                if (moveToFirstSong)
                    MoveToFirstSong();
            }
            numOfAudioTextBlock.Content = string.Format("Количество песен в списке: {0}", visibleSongs.Length);
            songsListBlocked = false;
        }

        private void ClearControls()
        {
            playlist1Songs = playlist2Songs = new Song[0];
            playlist1DataGrid.Items.Clear();
            playlist2DataGrid.Items.Clear();
            playlist1DataGrid.SelectedIndex = playlist2DataGrid.SelectedIndex = -1;
            numOfAudioTextBlock.Content = "Количество песен в списке: 0";
            sortComboBox.SelectedIndex = 0;
            searchTextBox.Text = "Поиск...";
            searchTextBox.Foreground = (Brush)this.Resources["SearchInactiveForegroundBrush"];
            musicTimelineSlider.Value = 0;
            currTimelinePosLabel.Content = maxTimelinePosLabel.Content = "00:00";
            artistLabel.Content = "";
            titleLabel.Content = "";
            isPlaying = false;
        }

        private void MoveToFirstSong()
        {
            if (visibDataGrid.Items.Count != 0)
            {
                visibDataGrid.SelectedIndex = 0;
                visibDataGrid.ScrollIntoView(visibDataGrid.SelectedItem);
            }
            SetMediaElementSource();
            Pause();
        }

        private void SetMediaElementSource()
        {
            this.Title = string.Empty;
            if (busyDataGrid.SelectedIndex == -1)
                return;
            var song = (Song)busyDataGrid.SelectedItem;
            mediaElement.Source = new Uri(song.Path);
            this.Title = string.Format("{0} - {1}", song.Artist, song.Title);
        }

        private void playlistsScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sw = e.OriginalSource as ScrollViewer;
            sw.UpdateLayout();
            if (sw.ExtentHeight != 0 && sw.ExtentHeight - sw.ViewportHeight - sw.VerticalOffset <= 250)
            {
                int maxLength = visibleSongs.Length;
                int curLength = visibDataGrid.Items.Count;
                int n = (maxLength - curLength >= numOfSongsToAddWhenScrolling) ? numOfSongsToAddWhenScrolling + curLength : maxLength;
                for (int i = curLength; i < n; i++)
                    visibDataGrid.Items.Add(visibleSongs[i]);
            }
        }
        #endregion

        #region CONFIG
        private void SaveSettings()
        {
            var doc = LoadConfigDocument();
            var settingsNode = doc.SelectSingleNode("//appSettings");
            if (settingsNode != null)
                doc.SelectSingleNode("//configuration").RemoveChild(settingsNode);
            settingsNode = doc.CreateNode(XmlNodeType.Element, "appSettings", "");
            doc.LastChild.AppendChild(settingsNode);
            List<string> keys = new List<string> {  "Window.Left",
                                                    "Window.Top",
                                                    "Window.Width",
                                                    "Window.Height",
                                                    "Window.WindowState",
                                                    "Player.UseDarkTheme",
                                                    "Plugins.Key" };
            List<string> values = new List<string> {this.Left.ToString(),
                                                    this.Top.ToString(),
                                                    this.Width.ToString(),
                                                    this.Height.ToString(),
                                                    this.WindowState.ToString(),
                                                    useDarkTheme.ToString(),
                                                    pluginsManager.Key };
            string pattern = "{0}.FavoriteItems.{1}.{2}";
            foreach (var pluginInstance in pluginsManager.PluginInstasnces)
            {
                keys.Add(string.Format("{0}.{1}", pluginInstance.Key, "OpenedTabIndex"));
                values.Add(pluginInstance.Value.OpenedTabIndex.ToString());
                for (int i = 0; i < pluginInstance.Value.FavoriteItems.Count; i++)
                {
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "Name"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "Path"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "Height"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "CanBeOpened"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "CanBeFavorite"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "AddRemoveFavoriteImageSource"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "FontSize"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "CursorType"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "ImageSource"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "UseAreYouSureMessageBox"));
                    keys.Add(string.Format(pattern, pluginInstance.Key, i, "AreYouSureMessageBoxMessage"));
                    values.Add(pluginInstance.Value.FavoriteItems[i].Name);
                    values.Add(pluginInstance.Value.FavoriteItems[i].Path);
                    values.Add(pluginInstance.Value.FavoriteItems[i].Height.ToString());
                    values.Add(pluginInstance.Value.FavoriteItems[i].CanBeOpened.ToString());
                    values.Add(pluginInstance.Value.FavoriteItems[i].CanBeFavorite.ToString());
                    values.Add(pluginInstance.Value.FavoriteItems[i].AddRemoveFavoriteImageSource);
                    values.Add(pluginInstance.Value.FavoriteItems[i].FontSize.ToString());
                    values.Add(new CursorConverter().ConvertToString(pluginInstance.Value.FavoriteItems[i].CursorType));
                    values.Add(pluginInstance.Value.FavoriteItems[i].ImageSource);
                    values.Add(pluginInstance.Value.FavoriteItems[i].UseAreYouSureMessageBox.ToString());
                    values.Add(pluginInstance.Value.FavoriteItems[i].AreYouSureMessageBoxMessage);
                }
            }
            for (int i = 0; i < keys.Count; i++)
            {
                var element = settingsNode.SelectSingleNode(string.Format("//add[@key='{0}']", keys[i])) as XmlElement;
                if (element != null)
                    element.SetAttribute("value", values[i]);
                else
                {
                    element = doc.CreateElement("add");
                    element.SetAttribute("key", keys[i]);
                    element.SetAttribute("value", values[i]);
                    settingsNode.AppendChild(element);
                }
            }
            doc.Save(Assembly.GetExecutingAssembly().Location + ".config");
        }

        private void LoadSettings()
        {
            var allAppSettings = ConfigurationManager.AppSettings;
            if (allAppSettings.Count < 1)
            {
                useDarkTheme = false;
                themeComboBox.SelectedIndex = 0;
                return;
            }
                
            foreach (var pluginInstance in pluginsManager.PluginInstasnces)
            {
                if (allAppSettings[string.Format("{0}.{1}", pluginInstance.Key, "OpenedTabIndex")] == null)
                    continue;
                pluginInstance.Value.OpenedTabIndex = Convert.ToInt32(allAppSettings[string.Format("{0}.{1}", pluginInstance.Key, "OpenedTabIndex")]);
                string pattern = "{0}.FavoriteItems.{1}.{2}";
                int i = 0;

                while (true)
                {
                    if (allAppSettings[string.Format(pattern, pluginInstance.Key, i, "Name")] != null)
                    {
                        pluginInstance.Value.AddToFavorites(new NavigationItem(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "Name")],
                                                                         allAppSettings[string.Format(pattern, pluginInstance.Key, i, "Path")],
                                                                         Convert.ToDouble(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "Height")]),
                                                                         Convert.ToBoolean(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "CanBeOpened")]),
                                                                         Convert.ToBoolean(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "CanBeFavorite")]),
                                                                         allAppSettings[string.Format(pattern, pluginInstance.Key, i, "AddRemoveFavoriteImageSource")],
                                                                         Convert.ToDouble(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "FontSize")]),
                                                                         new CursorConverter().ConvertFromString(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "CursorType")]) as Cursor,
                                                                         allAppSettings[string.Format(pattern, pluginInstance.Key, i, "ImageSource")],
                                                                         Convert.ToBoolean(allAppSettings[string.Format(pattern, pluginInstance.Key, i, "UseAreYouSureMessageBox")]),
                                                                         allAppSettings[string.Format(pattern, pluginInstance.Key, i, "AreYouSureMessageBoxMessage")]));
                    }
                    else
                        break;
                    i++;
                }
            }

            this.Left = Convert.ToDouble(allAppSettings["Window.Left"]);
            this.Top = Convert.ToDouble(allAppSettings["Window.Top"]);
            this.Width = Convert.ToDouble(allAppSettings["Window.Width"]);
            this.Height = Convert.ToDouble(allAppSettings["Window.Height"]);
            this.WindowState = (WindowState)WindowState.Parse(WindowState.GetType(), allAppSettings["Window.WindowState"]);
            useDarkTheme = Convert.ToBoolean(allAppSettings["Player.UseDarkTheme"]);
            themeComboBox.SelectedIndex = (useDarkTheme == true) ? 1 : 0;

            if (allAppSettings["Plugins.Key"] == string.Empty && modeComboBox.Items.Count > 0)
                allAppSettings["Plugins.Key"] = (modeComboBox.Items[0] as TextBlock).Tag.ToString();
            for (int i = 0; i < modeComboBox.Items.Count; i++)
                if ((modeComboBox.Items[i] as TextBlock).Tag.ToString() == allAppSettings["Plugins.Key"])
                {
                    modeComboBox.SelectedIndex = i;
                    break;
                }
        }

        private XmlDocument LoadConfigDocument()
        {
            XmlDocument doc;
            try
            {
                doc = new XmlDocument();
                doc.Load(Assembly.GetExecutingAssembly().Location + ".config");
                return doc;
            }
            catch (FileNotFoundException ex)
            {
                throw new Exception("Конфигурационный файл не найден.", ex);
            }
        }
        #endregion
    }
}
