using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjData : MonoBehaviour
{
    public List<DialogueData> dialogueList = new List<DialogueData>();

    [HideInInspector] public DialogueData dialogue;
    public bool isNPC;

    public void SetDialogue(int index)
    {
        if (index >= 0 && index < dialogueList.Count)
        {
            dialogue = dialogueList[index];
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: 해당 인덱스의 대사가 리스트에 없습니다!");
        }
    }
}
