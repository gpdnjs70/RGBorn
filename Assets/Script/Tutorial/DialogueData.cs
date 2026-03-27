using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TalkLine // 대화 한 줄의 정보
{
    public string talkerName;   // 말하는 사람 이름 (선택사항)
    [TextArea(3, 10)]
    public string context;      // 대화 내용
    public Sprite portrait;     // 해당 대화 시 표시할 초상화
}

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Data")]
public class DialogueData : ScriptableObject
{
    public int id;              // 오브젝트 식별용 ID
    public TalkLine[] lines;    // 대화 목록
}