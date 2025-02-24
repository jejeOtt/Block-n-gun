using BlockAndGun.Misc;
using BlockAndGun.Player.Classes;
using TMPro;
using UnityEngine;

namespace BlockAndGun.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] Transform weaponCamera;

        [SerializeField] GameObject hudContainer;
        [SerializeField] TMP_Text healthText;

        public BasicPlayerClassSO defaultBasicPlayerClassSO;

        GameManager gameManager;

        int currentHealth;

        private void Awake()
        {
            gameManager = FindFirstObjectByType<GameManager>();

            currentHealth = defaultBasicPlayerClassSO.Health;
            healthText.text = currentHealth.ToString();
        }

        public void TakeDamage(int hitDamageAmount)
        {
            currentHealth -= hitDamageAmount;
            healthText.text = currentHealth.ToString();
            if (currentHealth <= 0)
            {
                PlayerGameOver();
            }
        }

        private void PlayerGameOver()
        {
            weaponCamera.parent = null;
            gameManager.PlayerLoseAudioClip();
            hudContainer.SetActive(false);

            //InputPlayerController inputPlayerController = FindFirstObjectByType<InputPlayerController>();
            //inputPlayerController.SetCursorState(false);

            Destroy(this.gameObject);
        }

    }
}