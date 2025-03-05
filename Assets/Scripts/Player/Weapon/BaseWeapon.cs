using BlockAndGun.Services;
using UnityEngine;

namespace BlockAndGun.Player.Weapon
{
    public abstract class BaseWeapon : MonoBehaviour, IWeaponBase
    {

        public abstract void Attack(WeaponSO weaponSO);

        public virtual void Reload(WeaponSO weaponSO)
        {
            {
                Debug.Log($"{weaponSO.name} ne peut pas être rechargée.");
            }
        }

    }
}