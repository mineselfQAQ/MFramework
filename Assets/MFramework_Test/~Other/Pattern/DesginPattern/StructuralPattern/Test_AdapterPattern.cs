using MFramework;
using UnityEngine;

public class Test_AdapterPattern : MonoBehaviour
{
    private void Start()
    {
        AudioPlayer player = new AudioPlayer();

        player.Play("Mp3", "Song1");
        player.Play("Mp4", "Song2");
        player.Play("VLC", "Song3");
        player.Play("AVI", "Song4");
    }

    public interface ThirdPatryMediaPlayer
    {
        public void Play(string name);
    }
    public class VLCPlayer : ThirdPatryMediaPlayer
    {
        public void Play(string name)
        {
            MLog.Print($"VLC File Playing, Name: {name}");
        }
    }
    public class MP4Player : ThirdPatryMediaPlayer
    {
        public void Play(string name)
        {
            MLog.Print($"MP4 File Playing, Name: {name}");
        }
    }

    public interface MediaPlayer
    {
        public void Play(string type, string name);
    }
    public class MediaAdapter
    {
        private ThirdPatryMediaPlayer thirdPatryMediaPlayer;

        public MediaAdapter(string type)
        {
            if (type.ToLower() == "vlc")
            {
                thirdPatryMediaPlayer = new VLCPlayer();
            }
            else if (type.ToLower() == "mp4")
            {
                thirdPatryMediaPlayer = new MP4Player();
            }
        }

        public void Play(string name)
        {
            thirdPatryMediaPlayer.Play(name);
        }
    }
    public class AudioPlayer : MediaPlayer
    {
        private MediaAdapter mediaAdapter;

        public void Play(string type, string name)
        {
            if (type.ToLower() == "mp3")
            {
                MLog.Print($"MP3 File Playing, Name: {name}");
            }
            else if (type.ToLower() == "mp4" || type.ToLower() == "vlc")
            {
                mediaAdapter = new MediaAdapter(type);
                mediaAdapter.Play(name);
            }
            else
            {
                MLog.Print("Not Supported");
            }
        }
    }
}
