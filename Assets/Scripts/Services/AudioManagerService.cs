using BlockAndGun.Interfaces;
using UnityEngine;

namespace BlockAndGun.Services
{
    public class AudioManagerService : MonoBehaviour, IAudioService
    {
        [SerializeField] private AudioSource audioSource;
        public void PlaySound(AudioClip audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
