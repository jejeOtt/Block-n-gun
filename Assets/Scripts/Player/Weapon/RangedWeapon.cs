using Cinemachine;
using UnityEngine;

namespace BlockAndGun.Player.Weapon
{
    public class RangedWeapon : BaseWeapon
    {
        [SerializeField] ParticleSystem muzzleFlash;
        [SerializeField] LayerMask interactionLayers;

        CinemachineImpulseSource impulseSource;

        private void Awake()
        {
            impulseSource = GetComponent<CinemachineImpulseSource>();
        }

        public override void Attack(WeaponSO weaponSO)
        {
            muzzleFlash.Play();
            impulseSource.GenerateImpulse();

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity,
                interactionLayers, QueryTriggerInteraction.Ignore))
            {
                Vector3 hitEnemyPosition = hit.point;
                Instantiate(weaponSO.HitVFXPrefab, hitEnemyPosition, Quaternion.identity);



                //EnemyHealth enemyHealth = hit.collider.GetComponentInParent<EnemyHealth>();
                //enemyHealth?.TakeDamage(weaponSO.Damage);

            }
        }
    }
}