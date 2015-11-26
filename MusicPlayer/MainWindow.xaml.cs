using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections;
using MusicPlayerAPI;
using System.Windows.Input;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SongsManager songsManager = new SongsManager();
        private DispatcherTimer timer = new DispatcherTimer();
        private double currentSliderValue = -1;
        private bool isPlaying;
        private bool isMixed;
        private RepeatValue repeatValue;
        private IEnumerable tempItemSource;

        #region CONSTRUCTORS
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            songsDataGrid.ItemsSource = songsManager.GetList(@"D:\MUSIC\"); //temporary
            DisplaySongs();
        }
        #endregion

        #region EVENT HANDLERS
        private void MusicTimelineSlider_LostMouseCapture(object sender, MouseEventArgs e)
        {
            mediaElement.Position = TimeSpan.FromMilliseconds(currentSliderValue);
            currentSliderValue = -1;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            musicTimelineSlider.Value = (currentSliderValue == -1) ? mediaElement.Position.TotalMilliseconds : currentSliderValue;
            labelCurrTimelinePos.Content = TimeFormatter.Format((int)mediaElement.Position.TotalMinutes)
                + ":" + TimeFormatter.Format(mediaElement.Position.Seconds);
            songsDataGrid.Columns[0].Width = songsDataGrid.Columns[1].Width
                = (songsDataGrid.ActualWidth - songsDataGrid.Columns[2].Width.Value) / 2;
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            musicTimelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            labelMaxTimelinePos.Content = TimeFormatter.Format((int)mediaElement.NaturalDuration.TimeSpan.TotalMinutes)
                + ":" + TimeFormatter.Format(mediaElement.NaturalDuration.TimeSpan.Seconds);
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            Next();
        }

        private void BtPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
                Pause();
            else
                Play();
        }

        private void BtPrev_Click(object sender, RoutedEventArgs e)
        {
            Prev();
        }

        private void BtNext_Click(object sender, RoutedEventArgs e)
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

        private void BtRand_Click(object sender, RoutedEventArgs e)
        {
            isMixed = !isMixed;
            if (isMixed)
            {
                tempItemSource = (Song[])songsDataGrid.ItemsSource;
                songsDataGrid.ItemsSource = songsManager.CreateRandomList((Song[])songsDataGrid.ItemsSource);
                btRand.Content = "+Rand";
            }
            else
            {
                songsDataGrid.ItemsSource = tempItemSource;
                tempItemSource = null;
                btRand.Content = "-Rand";
            }
            DisplaySongs();
        }

        private void BtRepeat_Click(object sender, RoutedEventArgs e)
        {
            switch (repeatValue)
            {
                case RepeatValue.NoRepeat:
                    repeatValue = RepeatValue.RepeatSong;
                    btRepeat.Content = "+1Rep";
                    break;
                case RepeatValue.RepeatSong:
                    repeatValue = RepeatValue.RepeatList;
                    btRepeat.Content = "+Rep";
                    break;
                case RepeatValue.RepeatList:
                    repeatValue = RepeatValue.NoRepeat;
                    btRepeat.Content = "-Rep";
                    break;
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

        private void Play()
        {
            mediaElement.Play();
            btPlayPause.Content = "Pause";
            isPlaying = true;
        }

        private void Pause()
        {
            mediaElement.Pause();
            btPlayPause.Content = "Play";
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
            if (songsDataGrid.Items.Count == 0)
            {
                labelTitle.Content = labelArtist.Content = "[Музыки не найдено]";
                songsDataGrid.IsEnabled = false;
            }
            else
                MoveToFirstSong();
        }

        private void MoveToFirstSong()
        {
            songsDataGrid.SelectedIndex = 0;
            SetSong();
            Pause();
        }

        private void SetSong()
        {
            Song currentSong = (Song)songsDataGrid.SelectedItem;
            mediaElement.Source = new Uri(currentSong.Path);
            labelTitle.Content = "-  " + currentSong.Title;
            labelArtist.Content = currentSong.Artist;
        }
        #endregion
    }
}
