using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public TempleManager templeManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // 플레이어 태그 확인
        {
            templeManager.TriggerPaladinEvent();
            // 한 번만 실행되게 하려면 오브젝트를 파괴하거나 비활성화
            gameObject.SetActive(false); 
        }
    }
}
