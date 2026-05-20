using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earRing : MonoBehaviour
{
    [Header("사운드 설정")]
    public AudioClip BreakClip;

    // 애니메이션 이벤트에서 호출할 함수
    public void PlayBreakSound()
    {
        if (SoundManager.instance != null && BreakClip != null)
        {
            SoundManager.instance.SFXPlay("Break", BreakClip);
        }
    }
}
