using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using TagLib;
using MusicPlayerAPI;

namespace MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {        
        private double currentSliderValue = -1;
        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            InitializeMediaElement();
            Player.mainWindow = this;
            volumeSlider.ValueChanged += SetVolume;
            Player.GetList(@"D:\MUSIC\"); //temporary
        }

        private void InitializeMediaElement()
        {
            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.UnloadedBehavior = MediaState.Manual;
            mediaElement.Volume = 1;
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
            labelCurrTimelinePos.Content = FormatTime((int)mediaElement.Position.TotalMinutes) + ":" + FormatTime(mediaElement.Position.Seconds);
            if (mediaElement.NaturalDuration.HasTimeSpan && mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds == mediaElement.Position.TotalMilliseconds)
                Player.Next();
            songsDataGrid.Columns[0].Width = songsDataGrid.Columns[1].Width = (songsDataGrid.ActualWidth - songsDataGrid.Columns[2].Width.Value) / 2;
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            musicTimelineSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            labelMaxTimelinePos.Content = FormatTime((int)mediaElement.NaturalDuration.TimeSpan.TotalMinutes)
                + ":" + FormatTime(mediaElement.NaturalDuration.TimeSpan.Seconds);
        }

        public string FormatTime(int time)
        {
            return ((time < 10) ? "0" : "").ToString() + time.ToString();
        }

        private void btPlayPause_Click(object sender, RoutedEventArgs e)
        {            
            if (Player.IsPlaying)
                Player.Pause();
            else
                Player.Play();            
        }

        private void btPrev_Click(object sender, RoutedEventArgs e)
        {           
            Player.Prev();
        }

        private void btNext_Click(object sender, RoutedEventArgs e)
        {
            int temp = Player.repeatValue;
            Player.repeatValue = 0;
            Player.Next();
            Player.repeatValue = temp;
        }        

        private void SetVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {       
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
            if (Player.GetSongs().Length == 0)
                labelTitle.Content = labelArtist.Content = "[Музыки не найдено]";
            else
            {
                songsDataGrid.SelectedIndex = 0;
                SetSong();
                Player.Play();
                Player.Pause();
            }
        }

        public void SetSong()
        {
            Song currentSong = Player.GetSongs()[songsDataGrid.SelectedIndex];
            mediaElement.Source = new Uri(currentSong.Path);
            labelTitle.Content = "-  " + currentSong.Title;
            labelArtist.Content = currentSong.Artist;
        }

        private void songsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetSong();
            Player.Play();
        }

        private void btRand_Click(object sender, RoutedEventArgs e)
        {
            Player.MixSongs();
        }

        private void btRepeat_Click(object sender, RoutedEventArgs e)
        {
            switch (Player.repeatValue)
            {
                case 0:
                    Player.repeatValue = 1;
                    btRepeat.Content = "+1Rep";
                    break;
                case 1:
                    Player.repeatValue = 2;
                    btRepeat.Content = "+Rep";
                    break;
                case 2:
                    Player.repeatValue = 0;
                    btRepeat.Content = "-Rep";
                    break;
            }
        }
    }
}
