using BlockAndGun.Player.Weapon;

namespace BlockAndGun.Services
{
    public interface IWeapon
    {
        void Attack(WeaponSO weaponSO);
        void Reload(WeaponSO weaponSO);
    }
}