using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class DialogueImporterEditor : EditorWindow
{
    private TextAsset inputTextAsset;
    private DialogueText targetDialogue;

    [MenuItem("Tools/Dialogue Importer")]
    public static void ShowWindow()
    {
        GetWindow<DialogueImporterEditor>("Dialogue Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("대사 자동 입력기", EditorStyles.boldLabel);
        inputTextAsset = (TextAsset)EditorGUILayout.ObjectField("대사 텍스트 파일", inputTextAsset, typeof(TextAsset), false);
        targetDialogue = (DialogueText)EditorGUILayout.ObjectField("적용할 DialogueText", targetDialogue, typeof(DialogueText), false);

        if (GUILayout.Button("Import Dialogue") && inputTextAsset != null && targetDialogue != null)
        {
            ImportDialogue();
        }
    }

    private void ImportDialogue()
    {
        string[] lines = inputTextAsset.text.Split('\n');
        targetDialogue.speakers.Clear();
        targetDialogue.paragraphs.Clear();

        // 모든 Speaker 자산을 미리 불러오기
        string[] guids = AssetDatabase.FindAssets("t:Speaker", new[] { "Assets/Scripts/ETC/Dialogue/Speakers" });
        List<Speaker> allSpeakers = new List<Speaker>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Speaker speakerAsset = AssetDatabase.LoadAssetAtPath<Speaker>(path);
            if (speakerAsset != null)
            {
                allSpeakers.Add(speakerAsset);
            }
        }
        
        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] parts = line.Split('\t');
            if (parts.Length < 2) continue;

            string speakerNameFromText = parts[0].Trim();
            string dialogue = parts[1].Trim();
          
            Speaker matchedSpeaker = null;

// "주인공"이거나 "플레이어"면 null 넣기 → 런타임에서 플레이어 이름 사용
            if (speakerNameFromText.Contains("플레이어") || speakerNameFromText.Contains("주인공"))
            {
                matchedSpeaker = null; // 런타임에서 플레이어 이름 넣는 처리 있음
            }
            else
            {
                matchedSpeaker = allSpeakers.Find(s => s.speakerName == speakerNameFromText);
                if (matchedSpeaker == null)
                {
                    Debug.LogWarning($"⚠️ Speaker not found: {speakerNameFromText}");
                    continue; // 건너뛰기
                }
            }

            targetDialogue.speakers.Add(matchedSpeaker);
            targetDialogue.paragraphs.Add(dialogue);
        } 
        

        EditorUtility.SetDirty(targetDialogue);
        AssetDatabase.SaveAssets();
        Debug.Log("✅ 대사 자동 입력 완료!");
    }
}
