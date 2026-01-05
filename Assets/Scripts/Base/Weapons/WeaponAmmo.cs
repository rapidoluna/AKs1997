using UnityEngine;

public class WeaponAmmo : MonoBehaviour
{
    public int CurrentMagAmmo { get; private set; }

    public void Initialize(int magSize)
    {
        CurrentMagAmmo = magSize;
    }

    public bool CanConsume(int amount)
    {
        return CurrentMagAmmo >= amount;
    }

    public void Consume(int amount)
    {
        CurrentMagAmmo -= amount;
    }

    public void Refill(int amount)
    {
        CurrentMagAmmo = amount;
    }
}