using BlockAndGun.Interfaces;
using BlockAndGun.Misc;
using BlockAndGun.Player.Classes;
using BlockAndGun.Services;
using UnityEngine;

namespace BlockAndGun.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] Transform weaponCamera;

        private LocatorService serviceLocator;
        private IUIManagerService uIManagerService;

        public PlayerClassSO defaultBasicPlayerClassSO;

        GameManager gameManager;

        int currentHealth;

        private void Awake()
        {
            gameManager = FindFirstObjectByType<GameManager>();


        }
        private void Start()
        {
            serviceLocator = LocatorService.Instance;
            uIManagerService = serviceLocator.GetService<IUIManagerService>();

            currentHealth = defaultBasicPlayerClassSO.Health;
            uIManagerService.UpdateHealthUI(currentHealth);

        }

        public void TakeDamage(int hitDamageAmount)
        {
            currentHealth -= hitDamageAmount;
            uIManagerService.UpdateHealthUI(currentHealth);

            if (currentHealth <= 0)
            {
                PlayerGameOver();
            }
        }

        private void PlayerGameOver()
        {
            weaponCamera.parent = null;
            gameManager.PlayerLoseAudioClip();
            uIManagerService.SetHudContainer(false);

            //InputPlayerController inputPlayerController = FindFirstObjectByType<InputPlayerController>();
            //inputPlayerController.SetCursorState(false);

            Destroy(this.gameObject);
        }

    }
}