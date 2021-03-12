using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MusicScripts
{
    public class SongManager : MonoBehaviour
    {
        [SerializeField]
        protected AudioSource musicPlayer = null;
        [SerializeField]
        protected SongButtonEditor songPrefab = null;
        [SerializeField]
        protected Button pauseButton = null;
        [SerializeField]
        protected Transform songContainer = null;
        [SerializeField]
        protected Image coverArt = null;
        [SerializeField]
        protected Image playPauseImage = null;
        [SerializeField]
        protected Sprite pauseSprite = null;
        [SerializeField]
        protected Sprite playSprite = null;
        [SerializeField]
        protected Slider musicSlider = null;
        [SerializeField]
        protected DialogueTrigger barmanDialogue = null;

        [Header("Noel add new music here!")]
        [SerializeField]
        protected SongInfo[] songInfos = null;

        protected float currentLerpTime = 0f;
        protected bool firstSongPlayed = false;


        public void SongUiOpened ()
        {
            if (musicPlayer.isPlaying)
            {
                musicSlider.value = musicPlayer.time;
                StartCoroutine(SongPlaying(musicPlayer.clip.length));
            }
        }

        public void OnSongPlayed(AudioClip clip)
        {
            if (firstSongPlayed == false)
            {
                firstSongPlayed = true;
                barmanDialogue.QuestActionCompelete();
            }

            StopAllCoroutines();
            float songLength = clip.length;
            musicSlider.value = 0f;
            musicSlider.minValue = 0f;
            musicSlider.maxValue = songLength;
            musicPlayer.time = 0f;
            StartCoroutine(SongPlaying(songLength));
        }

        protected void OnPauseButtonClicked()
        {
            if (musicPlayer.isPlaying && musicPlayer.clip != null)
            {
                musicPlayer.Pause();
                playPauseImage.sprite = playSprite;
                StopAllCoroutines();
            }
            else if (!musicPlayer.isPlaying && musicPlayer.clip != null)
            {
                musicPlayer.UnPause();
                playPauseImage.sprite = pauseSprite;
                musicPlayer.time = musicSlider.value;
                StartCoroutine(SongPlaying(musicSlider.maxValue));
            }
        }

        protected IEnumerator SongPlaying(float songLength)
        {
            float currentTimeStamp = musicPlayer.time;

            while(currentTimeStamp < songLength)
            {
                currentTimeStamp += Time.deltaTime;
                currentLerpTime = currentTimeStamp;
                float lerpValue = currentTimeStamp / songLength;
                musicSlider.value = Mathf.Lerp(musicSlider.minValue, musicSlider.maxValue, lerpValue);
                yield return null;
            }
        }

        protected void Start()
        {
            for (int i = 0; i < songInfos.Length; i++)
            {
                SongButtonEditor newButton = Instantiate(songPrefab, songContainer);
                newButton.SetUpSongButton(musicPlayer, songInfos[i], coverArt, this);
            }
            musicSlider.value = 0;
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }
    }
}