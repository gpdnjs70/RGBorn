using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookSound : MonoBehaviour
{
    [Header("사운드 설정")]
    public AudioClip pageTurnClip; // 인스펙터에서 페이지 넘기는 소리 할당
    public AudioClip bookCloseClip; // 책 펼치는 소리 할당

    // 애니메이션 이벤트에서 호출할 함수
    public void PlayPageSound()
    {
        if (SoundManager.instance != null && pageTurnClip != null)
        {
            // 1회성 효과음 재생 (SFXPlay는 소리 이름을 임의로 지정)
            SoundManager.instance.SFXPlay("PageTurn", pageTurnClip);
        }
    }

    public void PlayCloseSound()
    {
        if (SoundManager.instance != null && bookCloseClip != null)
        {
            SoundManager.instance.SFXPlay("BookClose", bookCloseClip);
        }
    }
}
