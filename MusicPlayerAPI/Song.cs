using System.Collections.Generic;

namespace MusicPlayerAPI
{
    public class Song : IEqualityComparer<Song>
    {
        public string Title { get; set; }
        public string Artist { get; set; }        
        public string Duration { get; set; }
        public string Path { get; set; }
        public long CreationTime { get; set; }

        public Song() { }

        public Song(string title, string artist, string duration, string path, long creationTime)
        {
            Title = title;
            Artist = artist;
            Duration = duration;
            Path = path;
            CreationTime = creationTime;
        }

        public bool Equals(Song x, Song y)
        {
            return x.Path == y.Path;
        }

        public int GetHashCode(Song obj)
        {
            int hashSongTitle = obj.Title == null ? 0 : obj.Title.GetHashCode();
            int hashSongArtist = obj.Artist == null ? 0 : obj.Artist.GetHashCode();            
            int hashSongDuration = obj.Duration == null ? 0 : obj.Duration.GetHashCode();
            return hashSongTitle ^ hashSongArtist ^ hashSongDuration;
        }
    }
}
