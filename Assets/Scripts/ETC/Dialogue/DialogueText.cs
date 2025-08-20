using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueText")]
public class DialogueText : ScriptableObject
{
    public List<Speaker> speakers = new List<Speaker>();
    public List<string> paragraphs = new List<string>();
}
