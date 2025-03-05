using BlockAndGun.Interfaces;
using TMPro;
using UnityEngine;

namespace BlockAndGun.Services
{
    public class UIManagerService : MonoBehaviour, IUIManagerService
    {
        [SerializeField] private GameObject hudContainer;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text magazineText;
        [SerializeField] private TMP_Text ammoSizeText;
        [SerializeField] private GameObject zoomVignette;

        public void UpdateAmmoUI(int ammoAmount)
        {
            ammoSizeText.text = ammoAmount.ToString("D2");
        }

        public void UpdateMagazineUI(int magazineSize)
        {
            magazineText.text = magazineSize.ToString("D2");
        }

        public void UpdateHealthUI(int health)
        {
            healthText.text = health.ToString("D2");
        }

        public void SetHudContainer(bool active)
        {
            hudContainer.SetActive(active);
        }
    }
}
