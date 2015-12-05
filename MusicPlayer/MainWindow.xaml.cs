using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections;
using System.Collections.Generic;
using MusicPlayerAPI;
using System.Windows.Input;
using System.IO;
using System.Windows.Media.Imaging;

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
        private double currentSliderValue = -1;
        private bool isPlaying;
        private bool isMixed;
        private RepeatValue repeatValue;
        private IEnumerable lastSongsList;
        private string pluginKey { get { return ((TextBlock)modeComboBox.SelectedItem).Tag.ToString(); } }

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

        #region EVENT HANDLERS
        private void ModeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mediaElement.Close();
            ClearControls();
            if (modeComboBox.SelectedIndex == 0)
            {
                ShowNavigationItems(null);
                songsDataGrid.ItemsSource = pluginsManager.GetSongs(pluginKey);
                sortComboBox.SelectedIndex = 0;
                lastSongsList = null;
                DisplaySongs();
            }
        }

        private void NavigListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            navigListView.SelectedIndex = -1;
        }

        private void NavigListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            NavigationItem item = (navigListView.SelectedIndex == -1) ? null : ((StackPanel)navigListView.SelectedItem).Tag as NavigationItem;
            if (item != null && item.CanBeOpened)
                ShowNavigationItems(item.Path);
        }

        private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isMixed)
                CreateRandomList();
            else
                songsDataGrid.ItemsSource = songsManager.SortSongs((Song[])songsDataGrid.ItemsSource, sortComboBox.SelectedIndex);
            DisplaySongs();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            ShowNavigationItems(addressTextBox.Tag?.ToString());
        }

        private void MusicTimelineSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            mediaElement.Position = TimeSpan.FromMilliseconds(currentSliderValue);
            currentSliderValue = -1;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            musicTimelineSlider.Value = (currentSliderValue == -1) ? mediaElement.Position.TotalMilliseconds : currentSliderValue;
            currTimelinePosLabel.Content = TimeFormatter.Format((int)mediaElement.Position.TotalMinutes)
                + ":" + TimeFormatter.Format(mediaElement.Position.Seconds);
            songsDataGrid.Columns[0].Width = songsDataGrid.Columns[1].Width
                = (songsDataGrid.ActualWidth - songsDataGrid.Columns[2].Width.Value) / 2;
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            musicTimelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            Song currentSong = (Song)songsDataGrid.SelectedItem;
            artistLabel.Content = currentSong.Artist;
            titleLabel.Content = "-  " + currentSong.Title;
            maxTimelinePosLabel.Content = currentSong.Duration;
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

        private void SongsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSong();
            Play();
        }

        private void RandButton_Click(object sender, RoutedEventArgs e)
        {
            CreateRandomList();
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (lastSongsList == null)
                lastSongsList = songsDataGrid.ItemsSource;
            List<Song> songs = new List<Song>();
            if (searchTextBox.Text != string.Empty)
            {
                foreach (Song currentSong in lastSongsList)
                    if (currentSong.Title.ToLower().Contains(searchTextBox.Text.ToLower())
                        || currentSong.Artist.ToLower().Contains(searchTextBox.Text.ToLower()))
                        songs.Add(currentSong);
                songsDataGrid.ItemsSource = (isMixed) ? songsManager.CreateRandomList(songs.ToArray())
                    : songsManager.SortSongs(songs.ToArray(), sortComboBox.SelectedIndex);
                DisplaySongs();
            }
        }

        private void MyMusicButton_Click(object sender, RoutedEventArgs e)
        {
            if (lastSongsList != null)
            {
                songsDataGrid.ItemsSource = (isMixed) ? songsManager.CreateRandomList((Song[])lastSongsList)
                    : songsManager.SortSongs((Song[])lastSongsList, sortComboBox.SelectedIndex);
                lastSongsList = null;
                DisplaySongs();
            }
        }
        #endregion

        #region METHODS
        private void InitializeTimer()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void ShowNavigationItems(string path)
        {
            navigListView.Items.Clear();
            List<NavigationItem> navItems = pluginsManager.GetItems(pluginKey, path);
            addressTextBox.Tag = (navItems == null) ? addressTextBox.Tag?.ToString() : path;
            addressTextBox.Text = (addressTextBox.Tag == null) ? "Компьютер" : addressTextBox.Tag.ToString();
            if (navItems == null)
                navItems = pluginsManager.GetItems(pluginKey, addressTextBox.Tag?.ToString());
            
            foreach (NavigationItem ni in navItems)
            {
                StackPanel sp = new StackPanel();
                sp.Tag = ni;
                sp.Orientation = Orientation.Horizontal;
                Image img = new Image();
                img.Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/" + ni.ImageSource, UriKind.RelativeOrAbsolute));
                img.Height = ni.Height;
                TextBlock tb = new TextBlock();
                tb.Text = ni.Name;
                tb.Padding = new Thickness(5, 4, 0, 0);
                sp.Children.Add(img);
                sp.Children.Add(tb);
                navigListView.Items.Add(sp);
            }
            navigListView.ScrollIntoView(navigListView.Items[0]);            
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

        private void ClearControls()
        {
            songsDataGrid.ItemsSource = null;
            numOfAudioTextBlock.Content = "Песен: 0";
            sortComboBox.SelectedIndex = -1;
            searchTextBox.Clear();
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
            if (songsDataGrid.SelectedIndex > 0)
                songsDataGrid.SelectedIndex--;
        }

        private void Next()
        {
            if (repeatValue == RepeatValue.RepeatSong)
                mediaElement.Position = new TimeSpan(0, 0, 0, 0);
            else
            {
                if (songsDataGrid.SelectedIndex != songsDataGrid.Items.Count - 1)
                    songsDataGrid.SelectedIndex++;
                else if (repeatValue == RepeatValue.RepeatList)
                    songsDataGrid.SelectedIndex = 0;
            }
            Play();
            songsDataGrid.ScrollIntoView(songsDataGrid.SelectedItem);
        }

        private void DisplaySongs()
        {
            numOfAudioTextBlock.Content = string.Format("Песен: {0}", songsDataGrid.Items.Count);
            if (songsDataGrid.Items.Count == 0)
            {
                titleLabel.Content = artistLabel.Content = "[Музыки не найдено]";
                songsDataGrid.IsEnabled = false;
            }
            else
            {
                songsDataGrid.IsEnabled = true;
                MoveToFirstSong();
            }
        }

        private void MoveToFirstSong()
        {
            songsDataGrid.SelectedIndex = 0;
            SetSong();
            Pause();
        }

        private void SetSong()
        {
            mediaElement.Source = (songsDataGrid.SelectedIndex == -1) ? null : new Uri(((Song)songsDataGrid.SelectedItem).Path);
        }

        private void CreateRandomList()
        {
            isMixed = !isMixed;
            if (isMixed)
            {
                songsDataGrid.ItemsSource = songsManager.CreateRandomList((Song[])songsDataGrid.ItemsSource);
                DisplaySongs();
                randButton.Content = "+Rand";
            }
            else
            {
                songsDataGrid.ItemsSource = songsManager.SortSongs((Song[])songsDataGrid.ItemsSource, sortComboBox.SelectedIndex);
                randButton.Content = "-Rand";
            }
        }
        #endregion
    }
}
