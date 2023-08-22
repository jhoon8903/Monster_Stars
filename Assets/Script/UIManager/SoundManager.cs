using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Script.UIManager
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] internal AudioClip bgmClip;
        [SerializeField] internal AudioClip reward;
        [SerializeField] internal AudioClip unitSelect;
        [SerializeField] internal AudioClip popupOpen;
        [SerializeField] internal AudioClip popupClose;
        public static SoundManager Instance;
        private AudioSource Bgm { get; set; }
        private bool _music;
        private const string MusicKey = "Music";
        public bool IsMusicEnabled
        {
            get => _music;
            set
            {
                _music = value;
                Bgm.mute = !_music;
            }
        }

        private AudioSource Effect { get; set; }
        private bool _sound;
        private const string SoundKey = "Sound";

        public bool IsSoundEnabled
        {
            get => _sound;
            set
            {
                _sound = value;
                Effect.mute = !_sound;
            }
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            BGM();
            Effect = gameObject.AddComponent<AudioSource>(); // 여기를 수정합니다.
            Effect.clip = popupOpen;
            DontDestroyOnLoad(gameObject);
            _music = PlayerPrefs.GetInt(MusicKey) == 1;
            IsMusicEnabled = _music;
            _sound = PlayerPrefs.GetInt(SoundKey) == 1;
            IsSoundEnabled = _sound;
        }
        private void BGM()
        {
            Bgm = gameObject.AddComponent<AudioSource>();
            Bgm.clip = bgmClip;
            Bgm.loop = true;
            Bgm.Play(); 
        }

        public void PlaySound(AudioClip clip)
        {
            Effect.clip = clip;
            Effect.Play();
        }
    }
}