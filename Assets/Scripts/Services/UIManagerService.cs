using BlockAndGun.Interfaces;
using TMPro;
using UnityEngine;

namespace BlockAndGun.Services
{
    public class UIManagerService : MonoBehaviour, IUIManagerService
    {
        public GameObject hudContainer;
        public TMP_Text healthText;
        public TMP_Text magazineText;
        public TMP_Text ammoSizeText;
        public GameObject zoomVignette;

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
