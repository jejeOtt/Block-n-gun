using System;
using UnityEngine;

namespace BlockAndGun.Player.Weapon
{
    [CreateAssetMenu(fileName = "WeaponSO", menuName = "Scriptable Objects/WeaponSO")]
    public class WeaponSO : ScriptableObject
    {
        public int weaponID;

        public WeaponTypeSO weaponType;

        public GameObject weaponPrefab;
        public GameObject HitVFXPrefab;
        public GameObject weaponProjectilePrefab;
        public AudioClip shootSound;

        public bool IsAutomatic = false;
        public bool CanZoom = false;
        public bool CanReload = false;

        public int Damage = 1;
        public float FireRate = .5f;
        public float ZoomAmount = 10f;
        public float ZoomRotationSpeed = .3f;
        public float projectileSpeed = 1f;

        public int MagazineSize = 12;
        public int AmmoAmount = 48;
        public int CurrentMagazineSize = 0;
        public int CurrentAmmoAmount = 0;

        public float ReloadTime = 2.0f;
    }

    public enum WeaponTypeSO
    {
        Primary = 1,
        Secondary = 2,
        Melee = 3,
        Equipment = 4
    }
}