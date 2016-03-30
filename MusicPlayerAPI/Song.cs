using System.Collections.Generic;

namespace MusicPlayerAPI
{
    public class Song : IEqualityComparer<Song>
    {
        public string ID { get; }
        public string Title { get; }
        public string Artist { get; }        
        public string Duration { get; }
        public string Lyrics { get; }
        public string Path { get; }
        public long CreationTime { get; }

        public Song() { }

        public Song(string id, string title, string artist, string duration, string lyrics, string path, long creationTime)
        {
            ID = id;
            Title = title;
            Artist = artist;
            Duration = duration;
            Lyrics = lyrics;
            Path = path;
            CreationTime = creationTime;
        }

        public bool Equals(Song x, Song y)
        {
            return x.ID == y.ID;
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
