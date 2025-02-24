using BlockAndGun.Player.Weapon;
using UnityEngine;

namespace BlockAndGun.Pickups
{
    public abstract class Pickup : MonoBehaviour
    {
        [SerializeField] float rotationSpeed = 100f;

        const string playerString = "Player";

        private void Update()
        {
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(playerString))
            {
                ActiveWeapon activeWeapon = other.GetComponentInChildren<ActiveWeapon>();
                OnPickup(activeWeapon);
                Destroy(this.gameObject);
            }
        }
        protected abstract void OnPickup(ActiveWeapon activeWeapon);
    }
}