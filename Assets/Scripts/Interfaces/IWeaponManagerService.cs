using BlockAndGun.Player.Weapon;

namespace BlockAndGun.Interfaces
{
    public interface IWeaponManagerService
    {
        WeaponSO GetWeaponPrefabID(int id);
    }
}
