using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemInventoryUI : MonoBehaviour
{
    public static ItemInventoryUI Instance;

    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private Transform container;

    private List<GameObject> _spawnedIcons = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    public void AddItemIcon(Sprite iconSprite)
    {
        if (iconSprite == null || iconPrefab == null) return;

        GameObject newIcon = Instantiate(iconPrefab, container);
        Image img = newIcon.GetComponent<Image>();

        if (img != null)
        {
            img.sprite = iconSprite;
        }

        _spawnedIcons.Add(newIcon);
    }

    public void ClearIcons()
    {
        foreach (GameObject icon in _spawnedIcons)
        {
            if (icon != null) Destroy(icon);
        }
        _spawnedIcons.Clear();
    }
}