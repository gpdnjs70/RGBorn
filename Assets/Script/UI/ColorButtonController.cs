using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ColorMode
{
    Red,
    Green,
    Blue,
    Purple,
    Yellow,
    Cyan
}

public class ColorButtonController : MonoBehaviour
{
    [SerializeField] private Button rButton;
    [SerializeField] private Button gButton;
    [SerializeField] private Button bButton;

    [SerializeField] private Image rIcon;
    [SerializeField] private Image gIcon;
    [SerializeField] private Image bIcon;

    [SerializeField] private Sprite rSprite;
    [SerializeField] private Sprite gSprite;
    [SerializeField] private Sprite bSprite;
    [SerializeField] private Sprite ySprite;
    [SerializeField] private Sprite pSprite;
    [SerializeField] private Sprite cSprite;

    public ColorMode currentMode { get; private set; } = ColorMode.Red;
    // 기준색
    private ColorMode baseColor = ColorMode.Red;
    private bool hasBaseColor = false;

    private void Awake()
    {
        // 버튼 눌렀을 때 실행될 함수 연결
        if (rButton != null) rButton.onClick.AddListener(OnClickR);
        if (gButton != null) gButton.onClick.AddListener(OnClickG);
        if (bButton != null) bButton.onClick.AddListener(OnClickB);

        ResetColorSelection();
    }

    public void ResetColorSelection()
    {
        hasBaseColor = false;

        // 버튼 아이콘을 순수 RGB로 되돌리기
        if (rIcon != null) rIcon.sprite = rSprite;
        if (gIcon != null) gIcon.sprite = gSprite;
        if (bIcon != null) bIcon.sprite = bSprite;

        Debug.Log("Color selection reset (no base color)");
    }

    private void OnClickR()
    {
        // 아직 기준색 없음 → R을 기준색으로 세팅
        if (!hasBaseColor)
        {
            baseColor = ColorMode.Red;
            currentMode = ColorMode.Red;
            ApplyBaseColorR();
            hasBaseColor = true;

            Debug.Log("첫 기준색: Red");
            return;
        }

        // 이미 기준색이 R인데 다시 R 클릭 → 전체 리셋
        if (baseColor == ColorMode.Red)
        {
            ResetColorSelection();
            Debug.Log("BaseColor = Red 상태에서 R 재클릭 → Reset");
            return;
        }

        // 다른 기준색에서 R을 눌렀을 때는 섞기
        if (baseColor == ColorMode.Green)
        {
            currentMode = ColorMode.Yellow;
            Debug.Log($"BaseColor = Green, Red 버튼 → currentMode = Yellow");
        }
        else if (baseColor == ColorMode.Blue)
        {
            currentMode = ColorMode.Purple;
            Debug.Log($"BaseColor = Blue, Red 버튼 → currentMode = Purple");
        }
    }

    private void OnClickG()
    {
        // 아직 기준색 없음 → G를 기준색으로 세팅
        if (!hasBaseColor)
        {
            baseColor = ColorMode.Green;
            currentMode = ColorMode.Green;
            ApplyBaseColorG();
            hasBaseColor = true;

            Debug.Log("첫 기준색: Green");
            return;
        }

        // 이미 기준색이 G인데 다시 G 클릭 → 전체 리셋
        if (baseColor == ColorMode.Green)
        {
            ResetColorSelection();
            Debug.Log("BaseColor = Green 상태에서 G 재클릭 → Reset");
            return;
        }

        // 다른 기준색에서 G를 눌렀을 때 섞기
        if (baseColor == ColorMode.Red)
        {
            currentMode = ColorMode.Yellow;
            Debug.Log($"BaseColor = Red, Green 버튼 → currentMode = Yellow");
        }
        else if (baseColor == ColorMode.Blue)
        {
            currentMode = ColorMode.Cyan;
            Debug.Log($"BaseColor = Blue, Green 버튼 → currentMode = Cyan");
        }
    }

    private void OnClickB()
    {
        // 아직 기준색 없음 → B를 기준색으로 세팅
        if (!hasBaseColor)
        {
            baseColor = ColorMode.Blue;
            currentMode = ColorMode.Blue;
            ApplyBaseColorB();
            hasBaseColor = true;

            Debug.Log("첫 기준색: Blue");
            return;
        }

        // 이미 기준색이 B인데 다시 B 클릭 → 전체 리셋
        if (baseColor == ColorMode.Blue)
        {
            ResetColorSelection();
            Debug.Log("BaseColor = Blue 상태에서 B 재클릭 → Reset");
            return;
        }

        // 다른 기준색에서 B를 눌렀을 때 섞기
        if (baseColor == ColorMode.Red)
        {
            currentMode = ColorMode.Purple;
            Debug.Log($"BaseColor = Red, Blue 버튼 → currentMode = Purple");
        }
        else if (baseColor == ColorMode.Green)
        {
            currentMode = ColorMode.Cyan;
            Debug.Log($"BaseColor = Green, Blue 버튼 → currentMode = Cyan");
        }
    }

    private void ApplyBaseColorR()
    {
        if (rIcon != null) rIcon.sprite = rSprite;
        if (gIcon != null) gIcon.sprite = ySprite;
        if (bIcon != null) bIcon.sprite = pSprite;
    }

    private void ApplyBaseColorG()
    {
        if (rIcon != null) rIcon.sprite = ySprite;
        if (gIcon != null) gIcon.sprite = gSprite;
        if (bIcon != null) bIcon.sprite = cSprite;
    }

    private void ApplyBaseColorB()
    {
        if (rIcon != null) rIcon.sprite = pSprite;
        if (gIcon != null) gIcon.sprite = cSprite;
        if (bIcon != null) bIcon.sprite = bSprite;
    }
}
