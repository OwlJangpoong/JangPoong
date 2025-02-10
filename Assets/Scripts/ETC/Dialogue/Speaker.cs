using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/New Speaker")]
public class Speaker : ScriptableObject
{
    public string speakerName;
    public Sprite characterImage; 
}
