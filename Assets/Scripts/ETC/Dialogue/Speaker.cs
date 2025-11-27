using Unity.VisualScripting;
using UnityEngine;

public enum SpeakerPosition
{
    Left,
    Right,
    Center
}
[CreateAssetMenu(menuName = "Dialogue/New Speaker")]
public class Speaker : ScriptableObject
{
    public string speakerName;
    public Sprite characterImage;
    public SpeakerPosition position;

}
