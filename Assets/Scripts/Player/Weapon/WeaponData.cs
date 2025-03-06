using System;
using Unity.Netcode;
using UnityEngine;

namespace BlockAndGun.Player.Weapon
{
    [Serializable]
    public struct WeaponData : INetworkSerializable
    {
        public int weaponID;
        public WeaponTypeSO weaponType;
        public bool IsAutomatic;
        public bool CanZoom;
        public bool CanReload;
        public int Damage;
        public float FireRate;
        public float ZoomAmount;
        public float ZoomRotationSpeed;
        public float projectileSpeed;
        public int MagazineSize;
        public int AmmoAmount;
        public int CurrentMagazineSize;
        public int CurrentAmmoAmount;
        public float ReloadTime;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref weaponID);
            serializer.SerializeValue(ref weaponType);
            serializer.SerializeValue(ref IsAutomatic);
            serializer.SerializeValue(ref CanZoom);
            serializer.SerializeValue(ref CanReload);
            serializer.SerializeValue(ref Damage);
            serializer.SerializeValue(ref FireRate);
            serializer.SerializeValue(ref ZoomAmount);
            serializer.SerializeValue(ref ZoomRotationSpeed);
            serializer.SerializeValue(ref projectileSpeed);
            serializer.SerializeValue(ref MagazineSize);
            serializer.SerializeValue(ref AmmoAmount);
            serializer.SerializeValue(ref CurrentMagazineSize);
            serializer.SerializeValue(ref CurrentAmmoAmount);
            serializer.SerializeValue(ref ReloadTime);
        }
    }


}