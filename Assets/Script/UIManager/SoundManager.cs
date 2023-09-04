using System.Collections;
using UnityEngine;

namespace Script.UIManager
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField] internal AudioClip bgmClip;
        [SerializeField] internal AudioClip reward;
        [SerializeField] internal AudioClip unitSelect;
        [SerializeField] internal AudioClip popupOpen;
        [SerializeField] internal AudioClip popupClose;
        [SerializeField] internal AudioClip kfeAttackMotion;
        [SerializeField] internal AudioClip bcjAttackMotion;
        [SerializeField] internal AudioClip dAttackMotion;
        [SerializeField] internal AudioClip iagAttackMotion;
        [SerializeField] internal AudioClip cAttackMotion;
        [SerializeField] internal AudioClip stageClearSound;
        [SerializeField] internal AudioClip stageFailSound;
        [SerializeField] internal AudioClip touchUnitSound;
        [SerializeField] internal AudioClip match3Sound;
        [SerializeField] internal AudioClip match4Sound;
        [SerializeField] internal AudioClip match5Sound;
        [SerializeField] internal AudioClip stageFdsSound;
        [SerializeField] internal AudioClip stageVwSound;
        [SerializeField] internal AudioClip bossWaveClip;
        [SerializeField] internal AudioClip levelUpBtnSound;
        public static SoundManager Instance;
        private AudioSource Bgm { get; set; }
        public bool music = true;
        protected internal const string MusicKey = "Music";

        public bool IsMusicEnabled
        {
            get => music;
            set
            {
                music = value;
                if (Bgm != null)
                {
                    Bgm.mute = !music;
                }
            }
        }

        private AudioSource Effect { get; set; }
        public bool sound = true;
        protected internal const string SoundKey = "Sound";

        private AudioSource ClearSound { get; set; }
        private AudioSource AttackSound { get; set; }

        public bool IsSoundEnabled
        {
            get => sound;
            set
            {
                sound = value;
                Effect.mute = !sound;
            }
        }

        private AudioSource _match;

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
            BGM(bgmClip);
            Effect = gameObject.AddComponent<AudioSource>();
            Effect.clip = popupOpen;
            ClearSound = gameObject.AddComponent<AudioSource>();
            ClearSound.clip = stageClearSound;
            GameObject o;
            AttackSound = (o = gameObject).AddComponent<AudioSource>();
            AttackSound.clip = cAttackMotion;
            DontDestroyOnLoad(o);
            music = bool.Parse(PlayerPrefs.GetString(MusicKey, "true"));
            IsMusicEnabled = music;
            sound = bool.Parse(PlayerPrefs.GetString(SoundKey, "true"));
            IsSoundEnabled = sound;
            _match = gameObject.AddComponent<AudioSource>();
        }
        public void BGM(AudioClip clip)
        {
            if (Bgm != null && Bgm.isPlaying)
            {
                Bgm.Stop();
            }

            if (Bgm == null)
            {
                Bgm = gameObject.AddComponent<AudioSource>();
            }

            Bgm.clip = clip;
            Bgm.loop = true;
            Bgm.volume = 0.5f;
            Bgm.mute = !IsMusicEnabled;
            Bgm.Play();
        }

        public void PlaySound(AudioClip clip)
        {        
            if (Effect != null && Effect.isPlaying)
            {
                Effect.Stop();
            }
            Effect.clip = clip;
            Effect.Play();
        }

        public IEnumerator BossWave(AudioClip audioClip)
        {
            var currentClip = Bgm.clip;
            Debug.Log(currentClip.name);
            Bgm.Stop();
            Debug.Log(Bgm.isPlaying);
            Bgm.clip = audioClip;
            Bgm.Play();
            yield return new WaitForSeconds(audioClip.length);
            Bgm.Play();
            yield return new WaitForSeconds(audioClip.length);
            Bgm.clip = currentClip;
            Bgm.Play();
        }

        public void StageSound(int stage, bool forceChange = false)
        {
            if (!IsMusicEnabled && !forceChange) return;
            switch (stage)
            {
                case 1 or 2 or 4 or 6 or 7 or 8 or 11 or 12 or 14 or 17 or 18 or 19:
                    BGM(stageFdsSound);
                    break;
                case 3 or 5 or 9 or 10 or 13 or 15or 16 or 20:
                    BGM(stageVwSound);
                    break;
            }
        }

        public void ClearSoundEffect(AudioClip clip)
        {
            if (Bgm != null && Bgm.isPlaying)
            {
                Bgm.Stop();
            }

            ClearSound.clip = clip;
            ClearSound.Play();
        }
        public void AttackSoundEffect(AudioClip clip)
        {
            AttackSound.clip = clip;
            AttackSound.Play();
        }
        public void MatchSound(int matchCount)
        {
            if (!IsSoundEnabled || matchCount == 0) return;
            _match.clip = matchCount switch
            {
                3 => match3Sound,
                4 => match4Sound,
                5 => match5Sound,
            };
            _match.Play();
        }                       
    }
}