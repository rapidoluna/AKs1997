using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public static bool IsDead { get; private set; }

    [SerializeField] private CharacterData characterData;
    private float currentHealth;

    [Header("Disable when player's dead")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => characterData != null ? characterData.MaxHealth : 100f;

    void Awake()
    {
        IsDead = false;
        if (characterData != null) currentHealth = MaxHealth;
        if (characterController == null) characterController = GetComponent<CharacterController>();
    }

    public void SetData(CharacterData data)
    {
        characterData = data;
        currentHealth = MaxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        currentHealth -= damage;

        if (PlayerDamageEffect.Instance != null)
            PlayerDamageEffect.Instance.OnHit();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        foreach (var script in scriptsToDisable)
        {
            if (script != null) script.enabled = false;
        }

        if (characterController != null)
        {
            characterController.enabled = false;
        }

        if (PlayerDeathCamera.Instance != null)
        {
            PlayerDeathCamera.Instance.PlayDeathAnimation();
        }

        if (CashRushHUD.Instance != null)
            CashRushHUD.Instance.ShowNotification("CRITICAL ERROR: SYSTEM OFFLINE");
    }
}