using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockAndGun.Misc
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] TMP_Text enemiesLeftText;
        [SerializeField] GameObject youWinText;
        [SerializeField] GameObject hudContainer;
        [SerializeField] AudioClip winAudioClip;
        [SerializeField] AudioClip LoseAudioClip;

        AudioSource winOrLoseAudioSource;

        private void Start()
        {
            winOrLoseAudioSource = GetComponent<AudioSource>();

        }

        int enemiesLeft = 0;

        const string enemiesLeftString = "Il reste ça : ";

        public void AdjustEnemiesLeft(int amount)
        {
            enemiesLeft += amount;
            enemiesLeftText.text = enemiesLeftString + enemiesLeft.ToString();

            if (enemiesLeft <= 0)
            {
                hudContainer.SetActive(false);
                youWinText.SetActive(true);
                winOrLoseAudioSource.PlayOneShot(winAudioClip);
                StartCoroutine(WaitWhenWinning());
            }
        }

        public void PlayerLoseAudioClip()
        {
            winOrLoseAudioSource.PlayOneShot(LoseAudioClip);
        }

        IEnumerator WaitWhenWinning()
        {
            yield return new WaitForSeconds(2);
            RestartLevel();
        }

        public void RestartLevel()
        {
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene);
        }

        public void QuitButton()
        {
            Debug.LogWarning("Does not work in the Unity editor you fucvk");
            Application.Quit();
        }
    }

}

