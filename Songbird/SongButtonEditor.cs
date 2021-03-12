using UnityEngine;
using UnityEngine.UI;

namespace MusicScripts
{
    public class SongButtonEditor : MonoBehaviour
    {
        [SerializeField]
        protected Text songText = null;
        [SerializeField]
        protected Button songButton = null;

        protected SongInfo songInformation = null;
        protected AudioSource audioPlayer = null;
        protected Image coverImage = null;
        protected SongManager songManager = null;

        public void SetUpSongButton(AudioSource musicPlayer, SongInfo musiInformation, Image coverArt, SongManager manager)
        {
            songInformation = musiInformation;
            coverImage = coverArt;
            audioPlayer = musicPlayer;
            songText.text = songInformation.songName;
            songManager = manager;
            songButton.onClick.AddListener(OnSongButtonClicked);
        }

        protected void OnSongButtonClicked ()
        {
            audioPlayer.Stop();
            coverImage.sprite = songInformation.coverImage;
            audioPlayer.clip = songInformation.songFile;
            audioPlayer.Play();
            songManager.OnSongPlayed(audioPlayer.clip);
        }
    }
}
