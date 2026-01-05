using UnityEngine;

public class WeaponEffects : MonoBehaviour
{
    private WeaponController controller;
    private AudioSource audioSource;

    private void Start()
    {
        controller = GetComponent<WeaponController>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayFireEffect()
    {
        WeaponData data = controller.GetWeaponData();
        if (data.fireSound != null) audioSource.PlayOneShot(data.fireSound);
    }

    public void PlayEmptySound()
    {
        WeaponData data = controller.GetWeaponData();
        if (data.emptySound != null) audioSource.PlayOneShot(data.emptySound);
    }

    public void PlayReloadSound()
    {
        WeaponData data = controller.GetWeaponData();
        if (data.reloadSound != null) audioSource.PlayOneShot(data.reloadSound);
    }
}