using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MusicPlayerAPI;
using System.Linq;
using System.Windows.Input;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SongsManager songsManager = new SongsManager();
        private double currentSliderValue = -1;
        private DispatcherTimer timer = new DispatcherTimer();
        private bool isPlaying { get; set; }
        private RepeatValue repeatValue;

        #region CONSTRUCTORS
        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            musicTimelineSlider.LostMouseCapture += MusicTimelineSlider_LostMouseCapture;
            songsManager.GetList(@"D:\MUSIC\"); //temporary
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

        private void btPlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
                Pause();
            else
                Play();
        }

        private void btPrev_Click(object sender, RoutedEventArgs e)
        {
            Prev();
        }

        private void btNext_Click(object sender, RoutedEventArgs e)
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

        private void SetMediaPosition(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (musicTimelineSlider.IsMouseCaptureWithin)
                currentSliderValue = musicTimelineSlider.Value;
            if (currentSliderValue == -1)
                mediaElement.Position = TimeSpan.FromMilliseconds(musicTimelineSlider.Value);
        }

        private void songsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSong();
            Play();
        }

        private void btRand_Click(object sender, RoutedEventArgs e)
        {
            songsManager.IsMixed = !songsManager.IsMixed;
            if (songsManager.IsMixed)
            {
                songsManager.CreateRandomList();
                btRand.Content = "+Rand";
            }
            else
                btRand.Content = "-Rand";
            DisplaySongs();
        }

        private void btRepeat_Click(object sender, RoutedEventArgs e)
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
            if (songsManager.Songs.Length == 0)
            {
                labelTitle.Content = labelArtist.Content = "[Музыки не найдено]";
                songsDataGrid.IsEnabled = false;
            }
            else
            {
                ExtractDataToShow();
                MoveToFirstSong();
            }
        }

        private void ExtractDataToShow()
        {
            var dataToShow = from s in songsManager.Songs
                             select new { s.Title, s.Artist, s.Duration };
            songsDataGrid.ItemsSource = dataToShow;
        }

        private void MoveToFirstSong()
        {
            songsDataGrid.SelectedIndex = 0;
            SetSong();
            Pause();
        }

        private void SetSong()
        {
            Song currentSong = songsManager.Songs[songsDataGrid.SelectedIndex];
            mediaElement.Source = new Uri(currentSong.Path);
            labelTitle.Content = "-  " + currentSong.Title;
            labelArtist.Content = currentSong.Artist;
        }
        #endregion
    }
}
