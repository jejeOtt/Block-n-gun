using BlockAndGun.Interfaces;
using BlockAndGun.Player.Input;
using BlockAndGun.Services;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BlockAndGun.Player.Weapon
{
    public class ActiveWeapon : MonoBehaviour
    {
        [SerializeField] Camera weaponCamera;

        private LocatorService serviceLocator;

        private IAudioService audioService;
        private IUIManagerService uiManagerService;

        private Animator animator;
        private Transform playerCameraRoot;

        private PlayerManager playerManager;
        private PlayerController playerController;
        private CinemachineVirtualCamera playerFollowCamera;

        private BaseWeapon currentWeapon;

        private WeaponSO startingWeaponSO;
        private WeaponSO currentWeaponSO;

        private List<WeaponSO> cachedWeaponsSO;

        float defaultCameraFOV;
        float defaultRotationSpeed;
        float timeSinceLastShot = 0f;
        float timeSinceLastReload = 0f;

        const string shootString = "Shoot";

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            animator = GetComponent<Animator>();
            playerController = GetComponentInParent<PlayerController>();
            playerFollowCamera = FindFirstObjectByType<CinemachineVirtualCamera>();

            var cameraData = Camera.main.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(weaponCamera);

            playerCameraRoot = transform.parent;
            playerFollowCamera.Follow = playerCameraRoot;

            defaultCameraFOV = playerFollowCamera.m_Lens.FieldOfView;
            defaultRotationSpeed = playerController.RotationSpeed;
        }

        private void Start()
        {
            serviceLocator = LocatorService.Instance;

            audioService = serviceLocator.GetService<IAudioService>();
            uiManagerService = serviceLocator.GetService<IUIManagerService>();

            cachedWeaponsSO = playerManager.defaultBasicPlayerClassSO.weaponsSO;
            startingWeaponSO = cachedWeaponsSO.Where(x => x.weaponType == WeaponTypeSO.Primary).FirstOrDefault();

            RefillsAllWeaponsAmmo();
            CreateNewWeapon(startingWeaponSO);
        }

        private void Update()
        {
            timeSinceLastShot += Time.deltaTime;
            timeSinceLastReload += Time.deltaTime;

            uiManagerService.UpdateMagazineUI(currentWeaponSO.CurrentMagazineSize);
            uiManagerService.UpdateAmmoUI(currentWeaponSO.CurrentAmmoAmount);

        }
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

            currentWeapon = Instantiate(weaponSO.weaponPrefab, transform).GetComponent<BaseWeapon>();
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
                currentWeapon.Attack(currentWeaponSO);
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