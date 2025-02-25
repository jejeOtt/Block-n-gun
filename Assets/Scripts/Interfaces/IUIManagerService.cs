namespace BlockAndGun.Interfaces
{
    public interface IUIManagerService
    {
        void UpdateMagazineUI(int magazineSize);
        void UpdateAmmoUI(int ammoAmount);
        void UpdateHealthUI(int health);
        void SetHudContainer(bool active);
    }
}
