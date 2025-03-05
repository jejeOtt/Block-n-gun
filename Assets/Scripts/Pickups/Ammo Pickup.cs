using BlockAndGun.Player.Weapon;

namespace BlockAndGun.Pickups
{
    public class AmmoPickup : Pickup
    {

        //Attention j'ai mis une logique différente que celle dans le tuto mais faudra voir dans ass of shart 
        //je mettrai différent types de "packs" : small, medium, large
        protected override void OnPickup(ActiveWeapon activeWeapon)
        {
            activeWeapon.RefillsAmmo();
        }
    }
}

