using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public static bool IsDead { get; private set; }

    [SerializeField] private CharacterData characterData;
    private float currentHealth;
    private float _bonusMaxHealth;
    private float _currentBonusHealth;

    [SerializeField] private CharacterController characterController;
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    public float CurrentHealth => currentHealth;
    public float CurrentBonusHealth => _currentBonusHealth;
    public float BaseMaxHealth => characterData != null ? characterData.MaxHealth : 100f;
    public float BonusMaxHealth => _bonusMaxHealth;

    void Awake()
    {
        IsDead = false;
        if (characterData != null) currentHealth = BaseMaxHealth;
        if (characterController == null) characterController = GetComponent<CharacterController>();
    }

    public void SetData(CharacterData data)
    {
        characterData = data;
        currentHealth = BaseMaxHealth;
        _bonusMaxHealth = 0f;
        _currentBonusHealth = 0f;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        float remainingDamage = damage;

        if (_currentBonusHealth > 0)
        {
            if (_currentBonusHealth >= remainingDamage)
            {
                _currentBonusHealth -= remainingDamage;
                remainingDamage = 0;
            }
            else
            {
                remainingDamage -= _currentBonusHealth;
                _currentBonusHealth = 0;
            }
        }

        currentHealth -= remainingDamage;

        if (PlayerDamageEffect.Instance != null)
            PlayerDamageEffect.Instance.OnHit();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Min(currentHealth + amount, BaseMaxHealth);
    }

    public bool IsFullHealth()
    {
        return currentHealth >= BaseMaxHealth;
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;
        foreach (var script in scriptsToDisable) if (script != null) script.enabled = false;
        if (characterController != null) characterController.enabled = false;
        if (PlayerDeathCamera.Instance != null) PlayerDeathCamera.Instance.PlayDeathAnimation();
    }

    public void ApplyHealthBuff(float healthBonus)
    {
        _bonusMaxHealth = healthBonus;
        _currentBonusHealth = healthBonus;
    }

    public void ResetHealthBuff()
    {
        _bonusMaxHealth = 0f;
        _currentBonusHealth = 0f;
    }
}