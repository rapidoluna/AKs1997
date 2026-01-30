using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [SerializeField] private Image specialityIcon;
    [SerializeField] private Image ultimateIcon;
    [SerializeField] private Color activeColor = Color.white;
    [SerializeField] private Color inactiveColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    private AbilityProcessor _processor;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _processor = player.GetComponentInChildren<AbilityProcessor>();
            PlayerInitializer initializer = player.GetComponent<PlayerInitializer>();
            if (initializer != null && initializer.CharacterData != null)
            {
                if (specialityIcon) specialityIcon.sprite = initializer.CharacterData.characterSpeciality.abilityIcon;
                if (ultimateIcon) ultimateIcon.sprite = initializer.CharacterData.characterUltimate.abilityIcon;
            }
        }
    }

    private void Update()
    {
        if (_processor == null) return;

        UpdateVisual(specialityIcon, AbilityType.Speciality);
        UpdateVisual(ultimateIcon, AbilityType.Ultimate);
    }

    private void UpdateVisual(Image icon, AbilityType type)
    {
        if (icon == null || _processor == null) return;

        bool cooldown = _processor.GetCooldownNormalized(type) > 0;
        bool executing = _processor.IsExecuting(type);

        icon.color = (cooldown || executing) ? inactiveColor : activeColor;
    }
}