using UnityEngine;
using UnityEngine.UI;

namespace MusicScripts
{
    [System.Serializable]
    public class SongInfo
    {
        [SerializeField]
        public string songName = "";
        [SerializeField]
        public AudioClip songFile = null;
        [SerializeField]
        public Sprite coverImage = null;
    }
}