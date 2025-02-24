using UnityEngine;

namespace BlockAndGun.Player.Weapon
{
    public abstract class BaseWeapon : MonoBehaviour, IWeapon
    {

        public WeaponSO weaponData;  // Référence aux stats de l'arme

        protected virtual void Start()
        {
            if (weaponData == null)
                Debug.LogError($"WeaponData is missing on {gameObject.name}");
        }
        public abstract void Attack();

        public virtual void Reload()
        {
            Debug.Log($"{weaponData.name} ne peut pas être rechargée.");
        }

    }
}