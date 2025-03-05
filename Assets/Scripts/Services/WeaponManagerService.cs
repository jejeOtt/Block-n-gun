using BlockAndGun.Interfaces;
using BlockAndGun.Player.Weapon;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace BlockAndGun.Services
{
    public class WeaponManagerService : NetworkBehaviour, IWeaponManagerService
    {
        private readonly List<WeaponSO> weaponList = new List<WeaponSO>();

        public override void OnNetworkSpawn()
        {
            if (!IsServer) return;

            var weaponSORessources = Resources.FindObjectsOfTypeAll<WeaponSO>();

            foreach (var weapon in weaponSORessources)
            {
                weaponList.Add(weapon);
            } 
        }

        public WeaponSO GetWeaponPrefabID(int id)
        {
            return weaponList.FirstOrDefault(x => x.weaponID == id);
        }
    }
}
