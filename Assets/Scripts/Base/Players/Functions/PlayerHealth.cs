using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public static PlayerHealth Instance { get; private set; }
    public static bool IsDead { get; private set; }

    [SerializeField] private CharacterData characterData;

    [SerializeField] private CharacterController characterController;
    [SerializeField] private MonoBehaviour[] scriptsToDisable;

    private float _currentHealth;
    private float _bonusMaxHealth;
    private float _currentBonusHealth;

    public float CurrentHealth => _currentHealth;
    public float BaseMaxHealth => characterData != null ? characterData.MaxHealth : 100f;

    private void Awake()
    {
        Instance = this;
        IsDead = false;

        if (characterController == null)
            characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        if (characterData != null) InitHealth();
    }

    public void SetData(CharacterData data)
    {
        characterData = data;
        InitHealth();
    }

    private void InitHealth()
    {
        _currentHealth = BaseMaxHealth;
        _bonusMaxHealth = 0f;
        _currentBonusHealth = 0f;
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;

        if (GameSessionManager.Instance != null)
            GameSessionManager.Instance.damageTaken += damage;

        float remaining = damage;
        if (_currentBonusHealth > 0)
        {
            if (_currentBonusHealth >= remaining)
            {
                _currentBonusHealth -= remaining;
                remaining = 0;
            }
            else
            {
                remaining -= _currentBonusHealth;
                _currentBonusHealth = 0;
            }
        }

        _currentHealth -= remaining;

        if (PlayerDamageEffect.Instance != null)
            PlayerDamageEffect.Instance.OnHit();

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        _currentHealth = Mathf.Min(_currentHealth + amount, BaseMaxHealth);
    }

    public void ApplyHealthBuff(float bonus)
    {
        _bonusMaxHealth = bonus;
        _currentBonusHealth = bonus;
    }

    private void Die()
    {
        if (IsDead) return;
        IsDead = true;

        if (GameSessionManager.Instance != null)
            GameSessionManager.Instance.isExtracted = false;

        foreach (var script in scriptsToDisable)
            if (script != null) script.enabled = false;

        if (characterController != null)
            characterController.enabled = false;

        if (PlayerDeathCamera.Instance != null)
            PlayerDeathCamera.Instance.PlayDeathAnimation();

        Invoke(nameof(LoadResultScene), 3f);
    }

    private void LoadResultScene()
    {
        SceneManager.LoadScene("ResultScene");
    }
}