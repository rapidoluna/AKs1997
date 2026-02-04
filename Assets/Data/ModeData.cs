using UnityEngine;

[CreateAssetMenu(fileName = "ModeData", menuName = "AKs97/Mode Data")]
public class ModeData : ScriptableObject
{
    public string mapName;
    public string gameModeName;
    [TextArea]
    public string description;
}