using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using MusicPlayerAPI;

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
        public bool IsPlaying { get; set; }
        private RepeatValue repeatValue;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            songsManager.MainWindow = this;
            songsManager.GetList(@"D:\MUSIC\"); //temporary
        }

        private void InitializeTimer()
        {
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!musicTimelineSlider.IsMouseCaptureWithin)
            {
                if (currentSliderValue != -1)
                {
                    mediaElement.Position = TimeSpan.FromMilliseconds(currentSliderValue);
                    currentSliderValue = -1;
                }
                else
                    musicTimelineSlider.Value = mediaElement.Position.TotalMilliseconds;
            }
            labelCurrTimelinePos.Content = TimeFormatter.Format((int)mediaElement.Position.TotalMinutes) + ":" + TimeFormatter.Format(mediaElement.Position.Seconds);
            songsDataGrid.Columns[0].Width = songsDataGrid.Columns[1].Width = (songsDataGrid.ActualWidth - songsDataGrid.Columns[2].Width.Value) / 2;
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
            if (IsPlaying)
                Pause();
            else
                Play();
        }

        public void Play()
        {
            mediaElement.Play();
            btPlayPause.Content = "Pause";
            IsPlaying = true;
        }

        public void Pause()
        {
            mediaElement.Pause();
            btPlayPause.Content = "Play";
            IsPlaying = false;
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

        public void Prev()
        {
            if (songsDataGrid.SelectedIndex > 0)
                songsDataGrid.SelectedIndex--;
        }

        public void Next()
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

        public void CreateMediaList()
        {
            if (songsManager.Songs.Length == 0)
                labelTitle.Content = labelArtist.Content = "[Музыки не найдено]";
            else
            {
                songsDataGrid.SelectedIndex = 0;
                SetSong();
                Pause();
            }
        }

        public void SetSong()
        {
            Song currentSong = songsManager.Songs[songsDataGrid.SelectedIndex];
            mediaElement.Source = new Uri(currentSong.Path);
            labelTitle.Content = "-  " + currentSong.Title;
            labelArtist.Content = currentSong.Artist;
        }

        private void songsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSong();
            Play();
        }

        private void btRand_Click(object sender, RoutedEventArgs e)
        {
            songsManager.MixSongs();
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
    }
}
