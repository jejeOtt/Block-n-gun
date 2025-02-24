using BlockAndGun.Interfaces;
using BlockAndGun.Player.Input;
using BlockAndGun.Services;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace BlockAndGun.Player.Weapon
{
    public class ActiveWeapon : NetworkBehaviour
    {
        [SerializeField] Camera weaponCamera;
        [SerializeField] GameObject zoomVignette;
        [SerializeField] TMP_Text magazineSizeText;
        [SerializeField] TMP_Text ammoSizeText;

        private ServiceLocator serviceLocator;
        private IAudioService audioService;

        private Animator animator;

        private PlayerManager playerManager;
        private InputManager inputManager;
        private CinemachineVirtualCamera playerFollowCamera;

        private RangeWeapon currentWeapon;
        private WeaponSO startingWeaponSO;
        private List<WeaponSO> cachedWeaponsSO;
        private WeaponSO currentWeaponSO;


        float defaultCameraFOV;
        float defaultRotationSpeed;
        float timeSinceLastShot = 0f;
        float timeSinceLastReload = 0f;

        const string shootString = "Shoot";

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            animator = GetComponent<Animator>();
            inputManager = GetComponentInParent<InputManager>();
            playerFollowCamera = FindFirstObjectByType<CinemachineVirtualCamera>();

            defaultCameraFOV = playerFollowCamera.m_Lens.FieldOfView;
            defaultRotationSpeed = inputManager.RotationSpeed;
        }

        private void Start()
        {
            serviceLocator = ServiceLocator.Instance;
            audioService = serviceLocator.GetService<IAudioService>();

            cachedWeaponsSO = playerManager.defaultBasicPlayerClassSO.weaponsSO;
            startingWeaponSO = cachedWeaponsSO.Where(x => x.weaponType == WeaponTypeSO.Primary).FirstOrDefault();

            RefillsAllWeaponsAmmo();
            CreateNewWeapon(startingWeaponSO);
        }

        private void Update()
        {
            timeSinceLastShot += Time.deltaTime;
            timeSinceLastReload += Time.deltaTime;

            UpdateMagazineDisplay();
            UpdateAmmoDisplay();

        }

        void UpdateMagazineDisplay() => magazineSizeText.text = currentWeaponSO.CurrentMagazineSize.ToString("D2");
        void UpdateAmmoDisplay() => ammoSizeText.text = currentWeaponSO.CurrentAmmoAmount.ToString("D2");
        public void RefillsAmmo() => currentWeaponSO.CurrentAmmoAmount = currentWeaponSO.AmmoAmount;

        public void RefillsAllWeaponsAmmo()
        {
            cachedWeaponsSO.ForEach(x =>
            {
                x.CurrentMagazineSize = x.MagazineSize;
                x.CurrentAmmoAmount = x.AmmoAmount;
            });
        }

        public void SwitchWeapon(Vector2 input)
        {
            int currentIndex = cachedWeaponsSO.IndexOf(currentWeaponSO);

            // on utilise le modulo afin de pouvoir boucler dans la loop d'armes qu'on a au lieu d'utiliser des if (merci chatGPT)
            int nextIndex = (currentIndex - (int)input.y + cachedWeaponsSO.Count) % cachedWeaponsSO.Count;

            CreateNewWeapon(cachedWeaponsSO[nextIndex]);
        }

        private void CreateNewWeapon(WeaponSO weaponSO)
        {
            if (weaponSO == null || weaponSO == currentWeaponSO) return; // Empêche de recréer la même arme inutilement

            if (currentWeapon)
            {
                Destroy(currentWeapon.gameObject);
            }

            currentWeapon = Instantiate(weaponSO.weaponPrefab, transform).GetComponent<RangeWeapon>();
            currentWeaponSO = weaponSO;
        }


        public void AdjustMagazineAmount(int amount)
        {
            //Au lieu de faire un if on utilise la fonction Math.Clamp() pour éviter de dépasser la limite max du Magazine
            currentWeaponSO.CurrentMagazineSize = Mathf.Clamp(currentWeaponSO.CurrentMagazineSize + amount, 0, currentWeaponSO.MagazineSize);
        }

        public void HandleShoot()
        {
            if (currentWeapon == null || currentWeaponSO == null) return; // Vérifie si l'arme existe

            if (timeSinceLastShot >= currentWeaponSO.FireRate && timeSinceLastReload >= currentWeaponSO.ReloadTime
                && currentWeaponSO.CurrentMagazineSize > 0)
            {
                currentWeapon.Shoot(currentWeaponSO);
                audioService?.PlaySound(currentWeaponSO.shootSound);
                animator?.Play(shootString, 0, 0f);

                timeSinceLastShot = 0f;

                AdjustMagazineAmount(-1);
            }
        }

        public void HandleReload()
        {
            // Vérifier si le reload est possible
            if (timeSinceLastReload < currentWeaponSO.ReloadTime || currentWeaponSO.CurrentAmmoAmount <= 0
                || currentWeaponSO.CurrentMagazineSize == currentWeaponSO.MagazineSize) return;
            // Rien à faire si le reload est en cooldown ou si le chargeur est déjà plein
            timeSinceLastReload = 0f;

            StartCoroutine(ReloadTimeCoolDown());

        }

        private IEnumerator ReloadTimeCoolDown()
        {
            animator.Play("Reload", 0, 0f);

            yield return new WaitForSeconds(currentWeaponSO.ReloadTime);

            // Déterminer le nombre de balles à recharger
            int reloadAmmoAmount = Mathf.Min(currentWeaponSO.MagazineSize - currentWeaponSO.CurrentMagazineSize, currentWeaponSO.CurrentAmmoAmount);

            currentWeaponSO.CurrentAmmoAmount -= reloadAmmoAmount;
            AdjustMagazineAmount(reloadAmmoAmount);

        }

        //private void HandleZoom()
        //{
        //    if (!currentWeaponSO.CanZoom)
        //    {
        //        inputPlayerController.ZoomInput(false);
        //        ZoomOut();
        //        return;
        //    }

        //    if (inputPlayerController.zoom)
        //    {
        //        ZoomIn();
        //    }
        //    else
        //    {
        //        ZoomOut();
        //    }
        //}

        //private void ZoomOut()
        //{
        //    playerFollowCamera.m_Lens.FieldOfView = defaultCameraFOV;
        //    weaponCamera.fieldOfView = defaultCameraFOV;

        //    zoomVignette.SetActive(false);
        //    firstPersonController.ChangeRotationSpeed(defaultRotationSpeed);
        //}

        //private void ZoomIn()
        //{
        //    playerFollowCamera.m_Lens.FieldOfView = currentWeaponSO.ZoomAmount;
        //    weaponCamera.fieldOfView = currentWeaponSO.ZoomAmount;

        //    zoomVignette.SetActive(true);
        //    firstPersonController.ChangeRotationSpeed(currentWeaponSO.ZoomRotationSpeed);
        //}
    }
}