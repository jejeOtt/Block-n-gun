using BlockAndGun.Interfaces;
using BlockAndGun.Player.Input;
using BlockAndGun.Services;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Netcode;
using UnityEngine;

namespace BlockAndGun.Player.Weapon
{
    public class ActiveWeapon : NetworkBehaviour
    {
        private LocatorService serviceLocator;

        private IAudioService audioService;
        private IUIManagerService uiManagerService;
        private IWeaponManagerService weaponManagerService;

        private Animator animator;

        private PlayerManager playerManager;
        private PlayerController playerController;

        private Camera weaponCamera;

        private BaseWeapon currentWeapon;

        private WeaponSO currentWeaponSO;
        private WeaponSO startingWeaponSO;

        private List<WeaponSO> playerWeaponsSO;

        private NetworkVariable<int> networkCurrentWeaponIndex = new NetworkVariable<int>(-1); // -1 signifie aucune arme

        float defaultCameraFOV;
        float defaultRotationSpeed;
        float timeSinceLastShot = 0f;
        float timeSinceLastReload = 0f;

        const string shootString = "Shoot";
        const string reloadString = "Reload";

        public void RefillsAmmo() => currentWeaponSO.CurrentAmmoAmount = currentWeaponSO.AmmoAmount;

        public void RefillsAllWeaponsAmmo()
        {
            playerWeaponsSO.ForEach(x =>
            {
                x.CurrentMagazineSize = x.MagazineSize;
                x.CurrentAmmoAmount = x.AmmoAmount;
            });
        }

        private void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            playerController = GetComponentInParent<PlayerController>();
            animator = GetComponent<Animator>();

            serviceLocator = LocatorService.Instance;

            audioService = serviceLocator.GetService<IAudioService>();
            uiManagerService = serviceLocator.GetService<IUIManagerService>();
            weaponManagerService = serviceLocator.GetService<IWeaponManagerService>();
        }

        public override void OnNetworkSpawn()
        {

            if (IsOwner)
            {
                Debug.Log("Client : " + OwnerClientId +
                    ", IsServer : " + IsServer +
                    ", IsHost : " + IsHost +
                    ", IsClient : " + IsClient);


                defaultCameraFOV = playerController.playerFollowCamera.m_Lens.FieldOfView;
                defaultRotationSpeed = playerController.RotationSpeed;

                playerWeaponsSO = playerManager.defaultBasicPlayerClassSO.weaponsSO;

                WeaponSO startingWeaponSO = playerWeaponsSO.FirstOrDefault(x => x.weaponType == WeaponTypeSO.Primary);

                RefillsAllWeaponsAmmo();
                InstantiateNewWeaponForPlayer(startingWeaponSO);

            }
        }

        private void Update()
        {
            if (!IsOwner) return;

            timeSinceLastShot += Time.deltaTime;
            timeSinceLastReload += Time.deltaTime;

            if (uiManagerService != null && currentWeaponSO != null)
            {
                uiManagerService.UpdateMagazineUI(currentWeaponSO.CurrentMagazineSize);
                uiManagerService.UpdateAmmoUI(currentWeaponSO.CurrentAmmoAmount);
            }
        }

        public void SwitchWeapon(Vector2 input)
        {
            if (!IsOwner) return;

            int currentIndex = playerWeaponsSO.IndexOf(currentWeaponSO);

            // on utilise le modulo afin de pouvoir boucler dans la loop d'armes qu'on a au lieu d'utiliser des if (merci chatGPT)
            int nextIndex = (currentIndex - (int)input.y + playerWeaponsSO.Count) % playerWeaponsSO.Count;

            var weapon = playerWeaponsSO[nextIndex];

            // Envoyer la requête au serveur pour changer l'arme
            InstantiateNewWeaponForPlayer(weapon);
        }

        private void InstantiateNewWeaponForPlayer(WeaponSO weaponSO)
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

            if (timeSinceLastShot >= currentWeaponSO.FireRate &&
                timeSinceLastReload >= currentWeaponSO.ReloadTime &&
                currentWeaponSO.CurrentMagazineSize > 0)
            {
                PlayerShoot();

                timeSinceLastShot = 0f;

                AdjustMagazineAmount(-1);
            }
        }
        private void PlayerShoot()
        {
            currentWeapon.Attack(currentWeaponSO);
            audioService?.PlaySound(currentWeaponSO.shootSound);
            animator?.Play(shootString, 0, 0f);
        }

        public void HandleReload()
        {
            if (!IsOwner) return;

            // Vérifier si le reload est possible
            if (timeSinceLastReload < currentWeaponSO.ReloadTime || currentWeaponSO.CurrentAmmoAmount <= 0
                || currentWeaponSO.CurrentMagazineSize == currentWeaponSO.MagazineSize) return;
            // Rien à faire si le reload est en cooldown ou si le chargeur est déjà plein

            StartCoroutine(ReloadTimeCoolDown());
        }

        private IEnumerator ReloadTimeCoolDown()
        {
            animator?.Play(reloadString, 0, 0f);
            timeSinceLastReload = 0f;

            yield return new WaitForSeconds(currentWeaponSO.ReloadTime);

            // Déterminer le nombre de balles à recharger
            int reloadAmmoAmount = Mathf.Min(currentWeaponSO.MagazineSize - currentWeaponSO.CurrentMagazineSize, currentWeaponSO.CurrentAmmoAmount);

            currentWeaponSO.CurrentAmmoAmount -= reloadAmmoAmount;
            AdjustMagazineAmount(reloadAmmoAmount);

        }

    }
}