using UnityEngine;

public class WeaponShooting : MonoBehaviour
{
    private WeaponController controller;
    private float lastFireTime;

    private void Start()
    {
        controller = GetComponent<WeaponController>();
    }

    public void TryFire()
    {
        WeaponData data = controller.GetWeaponData();
        if (data == null) return;

        if (Time.time >= lastFireTime + (60f / 600f))
        {
            if (controller.Ammo.CanConsume((int)data.usingBullet))
            {
                Fire(data);
            }
            else
            {
                controller.Effects.PlayEmptySound();
            }
        }
    }

    private void Fire(WeaponData data)
    {
        lastFireTime = Time.time;
        controller.Ammo.Consume((int)data.usingBullet);

        for (int i = 0; i < data.firingBullets; i++)
        {
            PerformShoot(data);
        }

        controller.Effects.PlayFireEffect();
        controller.Recoil.ApplyRecoil();
    }

    private void PerformShoot(WeaponData data)
    {
        Vector3 fireDirection = transform.forward;
        if (Physics.Raycast(transform.position, fireDirection, out RaycastHit hit, data.effectiveRange))
        {
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(fireDirection * data.impactForce, ForceMode.Impulse);
            }
        }
    }
}