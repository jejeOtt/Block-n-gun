using BlockAndGun.Player.Weapon;
using System.Collections.Generic;
using UnityEngine;

namespace BlockAndGun.Player.Classes
{
    [CreateAssetMenu(fileName = "BasicPlayerClassSO", menuName = "Scriptable Objects/BasicPlayerClassSO")]
    public class PlayerClassSO : ScriptableObject
    {
        [Range(1, 300)]
        public int Health = 100;
        public float MoveSpeed = 1.0f;
        public float knockbackResistance = 10f;
        public Animator animator;

        public List<WeaponSO> weaponsSO;


    }
}