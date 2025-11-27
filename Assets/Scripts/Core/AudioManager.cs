using UnityEngine;

namespace TrashRunner.Core
{
    public class AudioManager : MonoBehaviour
    {
        // Audio Sources
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        // Audio Clips
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip slideClip;
        [SerializeField] private AudioClip coinClip;
        [SerializeField] private AudioClip trashClip;
        [SerializeField] private AudioClip hitObstacleClip;
        [SerializeField] private AudioClip buttonClickClip;
        [SerializeField] private AudioClip musicClip;

        // Toggles
        [SerializeField] private bool muteSFX = false;
        [SerializeField] private bool muteMusic = false;

        private void Start()
        {
            ValidateDependencies();
            SetupAudioSources();
        }

        private void ValidateDependencies()
        {
            if (musicSource == null)
                Debug.LogWarning($"{nameof(AudioManager)}: musicSource not assigned!");
            if (sfxSource == null)
                Debug.LogWarning($"{nameof(AudioManager)}: sfxSource not assigned!");
        }

        private void SetupAudioSources()
        {
            if (musicSource != null)
            {
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                if (musicClip != null)
                {
                    musicSource.clip = musicClip;
                }
            }

            if (sfxSource != null)
            {
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
        }

        public void PlayJump()
        {
            PlaySFX(jumpClip);
        }

        public void PlaySlide()
        {
            PlaySFX(slideClip);
        }

        public void PlayPickupCoin()
        {
            PlaySFX(coinClip);
        }

        public void PlayPickupTrash()
        {
            PlaySFX(trashClip);
        }

        public void PlayHitObstacle()
        {
            PlaySFX(hitObstacleClip);
        }

        public void PlayButtonClick()
        {
            PlaySFX(buttonClickClip);
        }

        private void PlaySFX(AudioClip clip)
        {
            if (muteSFX || sfxSource == null || clip == null)
                return;

            sfxSource.PlayOneShot(clip);
        }

        public void StartMusic()
        {
            if (muteMusic || musicSource == null)
                return;

            if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }

        public void StopMusic()
        {
            if (musicSource == null)
                return;

            if (musicSource.isPlaying)
            {
                musicSource.Stop();
            }
        }

        public void SetMuteSFX(bool mute)
        {
            muteSFX = mute;
        }

        public void SetMuteMusic(bool mute)
        {
            muteMusic = mute;
            
            if (muteMusic)
            {
                StopMusic();
            }
            else
            {
                StartMusic();
            }
        }
    }
}
