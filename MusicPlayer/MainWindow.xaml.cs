using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.Generic;
using MusicPlayerAPI;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;
using System.Linq;

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
        private Song[] mainPlaylist = new Song[0];
        private double currentSliderValue = -1;
        private bool isPlaying;
        private bool isMixed;
        private bool isMainPlaylist = true;
        private RepeatValue repeatValue;
        private ListView currentListView { get { return (navigTabControl.SelectedIndex == 0) ? navigListView : favoritesListView; } }
        private int currentTabIndex = 0;

        #region CONSTRUCTORS
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            pluginsManager.LoadPlugin(pluginsDirectory);
            SetModeComboBoxItems();
            modeComboBox.SelectedIndex = 0;
        }
        #endregion

        #region PLAYER CORE
        private void Timer_Tick(object sender, EventArgs e)
        {
            musicTimelineSlider.Value = (currentSliderValue == -1) ? mediaElement.Position.TotalMilliseconds : currentSliderValue;
            currTimelinePosLabel.Content = TimeFormatter.Format((int)mediaElement.Position.TotalMinutes)
                + ":" + TimeFormatter.Format(mediaElement.Position.Seconds);
        }

        private void MusicTimelineSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            mediaElement.Position = TimeSpan.FromMilliseconds(currentSliderValue);
            currentSliderValue = -1;
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            musicTimelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            Song currentSong = mainPlaylistDataGrid.SelectedItem as Song;
            if (currentSong != null)
            {
                artistLabel.Content = currentSong.Artist;
                titleLabel.Content = "-  " + currentSong.Title;
                maxTimelinePosLabel.Content = currentSong.Duration;
            }
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
                Pause();
            else
                Play();
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            Prev();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            RepeatValue temp = repeatValue;
            repeatValue = RepeatValue.NoRepeat;
            Next();
            repeatValue = temp;
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement != null)
                mediaElement.Volume = volumeSlider.Value;
        }

        private void MusicTimelineSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (musicTimelineSlider.IsMouseCaptureWithin)
                currentSliderValue = musicTimelineSlider.Value;
            if (currentSliderValue == -1)
                mediaElement.Position = TimeSpan.FromMilliseconds(musicTimelineSlider.Value);
        }

        private void RandButton_Click(object sender, RoutedEventArgs e)
        {
            isMixed = !isMixed;
            randButton.Content = (isMixed) ? "+Rand" : "-Rand";
            SetSongsList((Song[])dependentPlaylistDataGrid.ItemsSource, true);
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            switch (repeatValue)
            {
                case RepeatValue.NoRepeat:
                    repeatValue = RepeatValue.RepeatSong;
                    repeatButton.Content = "+1Rep";
                    break;
                case RepeatValue.RepeatSong:
                    repeatValue = RepeatValue.RepeatList;
                    repeatButton.Content = "+Rep";
                    break;
                case RepeatValue.RepeatList:
                    repeatValue = RepeatValue.NoRepeat;
                    repeatButton.Content = "-Rep";
                    break;
            }
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
            playPauseButton.Content = "Pause";
            isPlaying = true;
        }

        private void Pause()
        {
            mediaElement.Pause();
            playPauseButton.Content = "Play";
            isPlaying = false;
        }

        private void Prev()
        {
            if (mainPlaylistDataGrid.SelectedIndex > 0)
                mainPlaylistDataGrid.SelectedIndex--;
        }

        private void Next()
        {
            if (repeatValue == RepeatValue.RepeatSong)
                mediaElement.Position = new TimeSpan(0, 0, 0, 0);
            else
            {
                if (mainPlaylistDataGrid.SelectedIndex != mainPlaylistDataGrid.Items.Count - 1)
                    mainPlaylistDataGrid.SelectedIndex++;
                else if (repeatValue == RepeatValue.RepeatList)
                    mainPlaylistDataGrid.SelectedIndex = 0;
            }
            Play();
            mainPlaylistDataGrid.ScrollIntoView(mainPlaylistDataGrid.SelectedItem);
        }
        #endregion

        #region WORKING WITH PLUGINS & NAVIGATION
        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mediaElement.Close();
            ClearControls();
            pluginsManager.Key = ((TextBlock)modeComboBox.SelectedItem).Tag.ToString();
            for (int i = 0; i < navigTabControl.Items.Count; i++)
                ((TabItem)navigTabControl.Items[i]).Header = pluginsManager.GetHeader(i);
            ShowItems(addressTextBox.Tag?.ToString(), true);
            sortComboBox.SelectedIndex = 0;
            mainPlaylist = pluginsManager.GetSongs();
            SetSongsList(mainPlaylist, true);
        }

        private void NavigTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (currentTabIndex != navigTabControl.SelectedIndex)
            {
                ShowItems(addressTextBox.Tag?.ToString(), false);
                currentTabIndex = (currentTabIndex == 0) ? 1 : 0;
            }
        }

        private void NavigFavListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            currentListView.SelectedIndex = -1;
        }

        private void NavigListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationItem item = (navigListView.SelectedIndex == -1) ? null : ((DockPanel)navigListView.SelectedItem).Tag as NavigationItem;
            if (item != null && item.CanBeOpened)
                ShowItems(item.Path, true);
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            ShowItems(addressTextBox.Tag?.ToString(), false);
        }

        private void ImgAddDel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            NavigationItem ni = (NavigationItem)((DockPanel)((Image)sender).Parent).Tag;
            if (pluginsManager.IsFavorite(ni))
                pluginsManager.DeleteFromFavorites(ni);
            else
                pluginsManager.AddToFavorites(ni);
            ShowItems(addressTextBox.Tag?.ToString(), false);
            mediaElement.Close();
            mainPlaylist = pluginsManager.GetSongs();
            SetSongsList(mainPlaylist, true);
            if (dependentPlaylistDataGrid.ItemsSource != null && mainPlaylistDataGrid.ItemsSource != null && ((Song[])dependentPlaylistDataGrid.ItemsSource).SequenceEqual((Song[])mainPlaylistDataGrid.ItemsSource))
                dependentPlaylistDataGrid.SelectedIndex = 0;
            if (mainPlaylist == null || mainPlaylist.Length == 0)
                ClearControls();
        }

        private void ShowItems(string path, bool isScrollToUp)
        {
            ShowNavigationItems(path, isScrollToUp);
            ShowFavoriteItems();
        }

        private void ShowNavigationItems(string path, bool isScrollToUp)
        {
            navigListView.Items.Clear();
            List<NavigationItem> navItems = pluginsManager.GetNavigationItems(path);
            addressTextBox.Tag = (navItems == null) ? addressTextBox.Tag?.ToString() : path;
            addressTextBox.Text = (addressTextBox.Tag == null) ? "Компьютер" : addressTextBox.Tag.ToString();
            if (navItems == null)
                navItems = pluginsManager.GetNavigationItems(addressTextBox.Tag?.ToString());
            foreach (NavigationItem ni in navItems)
                ShowNavigationItem(navigListView, ni);
            if (navigListView.Items.Count > 0 && isScrollToUp)
                navigListView.ScrollIntoView(navigListView.Items[0]);
        }

        void ShowFavoriteItems()
        {
            favoritesListView.Items.Clear();
            List<NavigationItem> favItems = pluginsManager.GetFavoriteItems();
            if (navigTabControl.SelectedIndex == 1)
                addressTextBox.Text = pluginsManager.GetHeader(1);
            foreach (NavigationItem ni in favItems)
                ShowNavigationItem(favoritesListView, ni);
            if (favoritesListView.Items.Count > 0)
                favoritesListView.ScrollIntoView(favoritesListView.Items[0]);
        }

        void ShowNavigationItem(ListView listView, NavigationItem ni)
        {
            DockPanel dp = new DockPanel();
            dp.Tag = ni;
            dp.LastChildFill = true;
            dp.Height = ni.Height;
            dp.Width = listView.MinWidth;
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/" + ni.ImageSource, UriKind.RelativeOrAbsolute));
            img.Height = ni.Height;
            DockPanel.SetDock(img, Dock.Left);
            dp.Children.Add(img);
            if (ni.CanBeFavorite)
            {
                Image imgAddDel = new Image();
                imgAddDel.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/" + pluginsManager.GetItemButtonImage(ni), UriKind.RelativeOrAbsolute));
                imgAddDel.ToolTip = "Добавить в избранное / удалить из избранного";
                imgAddDel.Height = ni.Height;
                imgAddDel.Cursor = Cursors.Hand;
                imgAddDel.MouseDown += ImgAddDel_MouseDown;
                DockPanel.SetDock(imgAddDel, Dock.Right);
                dp.Children.Add(imgAddDel);
            }
            TextBlock tb = new TextBlock();
            tb.Text = ni.Name;
            tb.Padding = new Thickness(5, 4, 0, 0);
            dp.Children.Add(tb);
            listView.Items.Add(dp);
        }

        private void SetModeComboBoxItems()
        {
            foreach (KeyValuePair<string, IPlugin> kvp in pluginsManager.PluginInstasnces)
            {
                TextBlock tb = new TextBlock();
                tb.Text = kvp.Value.Name;
                tb.Tag = kvp.Key;
                modeComboBox.Items.Add(tb);
            }
        }
        #endregion

        #region WORKING WITH SONGS
        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            randButton.Content = "-Rand";
            isMixed = false;
            SetSongsList((Song[])dependentPlaylistDataGrid.ItemsSource, false);
            dependentPlaylistDataGrid.SelectedIndex = -1;
        }

        private void MainPlaylistDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (mainPlaylistDataGrid?.ItemsSource == null)
                return;
            if ((isMainPlaylist && dependentPlaylistDataGrid.ItemsSource == null)
                || ((Song[])dependentPlaylistDataGrid.ItemsSource).SequenceEqual((Song[])mainPlaylistDataGrid.ItemsSource)
                && mainPlaylistDataGrid.SelectedIndex != -1)
            {
                dependentPlaylistDataGrid.ItemsSource = mainPlaylistDataGrid.ItemsSource;
                dependentPlaylistDataGrid.SelectedIndex = mainPlaylistDataGrid.SelectedIndex;
            }
            PlaySong();
            Play();
        }

        private void DependentPlaylistDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dependentPlaylistDataGrid.SelectedIndex != -1)
            {
                mainPlaylistDataGrid.ItemsSource = dependentPlaylistDataGrid.ItemsSource;
                mainPlaylistDataGrid.SelectedIndex = dependentPlaylistDataGrid.SelectedIndex;
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text != string.Empty)
            {
                List<Song> songs = new List<Song>();
                foreach (Song currentSong in mainPlaylist)
                    if (currentSong.Title.ToLower().Contains(searchTextBox.Text.ToLower())
                        || currentSong.Artist.ToLower().Contains(searchTextBox.Text.ToLower()))
                        songs.Add(currentSong);
                isMainPlaylist = false;
                SetSongsList(songs.ToArray(), false);
                SelectItem();
            }
        }

        private void MyMusicButton_Click(object sender, RoutedEventArgs e)
        {
            isMainPlaylist = true;
            dependentPlaylistDataGrid.ItemsSource = isMixed ? mainPlaylist : songsManager.SortSongs(mainPlaylist, sortComboBox.SelectedIndex);
            numOfAudioTextBlock.Content = string.Format("Песен: {0}", dependentPlaylistDataGrid.Items.Count);
            SelectItem();
        }

        private void SelectItem()
        {
            if (((Song[])dependentPlaylistDataGrid.ItemsSource).SequenceEqual((Song[])mainPlaylistDataGrid.ItemsSource))
                dependentPlaylistDataGrid.SelectedIndex = mainPlaylistDataGrid.SelectedIndex;
            else
                dependentPlaylistDataGrid.SelectedIndex = -1;
        }

        private void SetSongsList(Song[] list, bool moveToFirstSong)
        {
            if (list == null)
                dependentPlaylistDataGrid.ItemsSource = null;
            else
            {
                dependentPlaylistDataGrid.ItemsSource = isMixed ? songsManager.MixSongs(list)
                    : songsManager.SortSongs(list, sortComboBox.SelectedIndex);
                if (isMainPlaylist)
                    mainPlaylist = isMixed ? (Song[])dependentPlaylistDataGrid.ItemsSource : mainPlaylist;
                numOfAudioTextBlock.Content = string.Format("Песен: {0}", dependentPlaylistDataGrid.Items.Count);
                if (moveToFirstSong)
                    MoveToFirstSong();
            }
        }

        private void ClearControls()
        {
            mainPlaylistDataGrid.ItemsSource = dependentPlaylistDataGrid.ItemsSource = null;
            mainPlaylistDataGrid.SelectedIndex = dependentPlaylistDataGrid.SelectedIndex = 0;
            isMainPlaylist = true;
            numOfAudioTextBlock.Content = "Песен: 0";
            sortComboBox.SelectedIndex = 0;
            searchTextBox.Clear();
            musicTimelineSlider.Value = 0;
            currTimelinePosLabel.Content = maxTimelinePosLabel.Content = "00:00";
            artistLabel.Content = "Исполнитель";
            titleLabel.Content = "Название";
            playPauseButton.Content = "Play";
            isPlaying = false;
        }

        private void MoveToFirstSong()
        {
            dependentPlaylistDataGrid.SelectedIndex = 0;
            PlaySong();
            Pause();
        }

        private void PlaySong()
        {
            if (mainPlaylistDataGrid.SelectedIndex == -1)
                return;
            mediaElement.Source = new Uri(((Song)mainPlaylistDataGrid.SelectedItem).Path);
        }
        #endregion
    }
}
