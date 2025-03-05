using BlockAndGun.Player.Weapon;

namespace BlockAndGun.Services
{
    public interface IWeaponBase
    {
        void Attack(WeaponSO weaponSO);
        void Reload(WeaponSO weaponSO);
    }
}