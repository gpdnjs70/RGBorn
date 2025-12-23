using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public ColorMode currentMode { get; private set; } = ColorMode.None;
    // 기준색
    private ColorMode baseColor = ColorMode.None;
    private bool hasBaseColor = false;

    public System.Action<ColorMode> OnColorChanged;

    private void Awake()
    {
        // 버튼 눌렀을 때 실행될 함수 연결
        if (rButton != null) rButton.onClick.AddListener(OnClickR);
        if (gButton != null) gButton.onClick.AddListener(OnClickG);
        if (bButton != null) bButton.onClick.AddListener(OnClickB);

        ResetColorSelection();
    }

    // 버튼 리셋
    public void ResetColorSelection()
    {
        hasBaseColor = false;
        baseColor = ColorMode.None;
        currentMode = ColorMode.None;
        OnColorChanged?.Invoke(currentMode);

        if (rIcon != null) rIcon.sprite = rSprite;
        if (gIcon != null) gIcon.sprite = gSprite;
        if (bIcon != null) bIcon.sprite = bSprite;
    }

    private void OnClickR()
    {
        // 아직 기준색 없음 → R을 기준색으로 세팅
        if (!hasBaseColor)
        {
            baseColor = ColorMode.Red;
            currentMode = ColorMode.Red;
            OnColorChanged?.Invoke(currentMode);
            ApplyBaseColorR();
            hasBaseColor = true;

            return;
        }

        // 이미 기준색이 R인데 다시 R 클릭 → 전체 리셋
        if (baseColor == ColorMode.Red)
        {
            ResetColorSelection();
            return;
        }

        // 다른 기준색에서 R을 눌렀을 때는 섞기
        if (baseColor == ColorMode.Green)
        {
            currentMode = ColorMode.Yellow;
            OnColorChanged?.Invoke(currentMode);
        }
        else if (baseColor == ColorMode.Blue)
        {
            currentMode = ColorMode.Purple;
            OnColorChanged?.Invoke(currentMode);
        }
    }

    private void OnClickG()
    {
        // 아직 기준색 없음 → G를 기준색으로 세팅
        if (!hasBaseColor)
        {
            baseColor = ColorMode.Green;
            currentMode = ColorMode.Green;
            OnColorChanged?.Invoke(currentMode);
            ApplyBaseColorG();
            hasBaseColor = true;

            return;
        }

        // 이미 기준색이 G인데 다시 G 클릭 → 전체 리셋
        if (baseColor == ColorMode.Green)
        {
            ResetColorSelection();
            return;
        }

        // 다른 기준색에서 G를 눌렀을 때 섞기
        if (baseColor == ColorMode.Red)
        {
            currentMode = ColorMode.Yellow;
            OnColorChanged?.Invoke(currentMode);
        }
        else if (baseColor == ColorMode.Blue)
        {
            currentMode = ColorMode.Cyan;
            OnColorChanged?.Invoke(currentMode);
        }
    }

    private void OnClickB()
    {
        // 아직 기준색 없음 → B를 기준색으로 세팅
        if (!hasBaseColor)
        {
            baseColor = ColorMode.Blue;
            currentMode = ColorMode.Blue;
            OnColorChanged?.Invoke(currentMode);
            ApplyBaseColorB();
            hasBaseColor = true;

            return;
        }

        // 이미 기준색이 B인데 다시 B 클릭 → 전체 리셋
        if (baseColor == ColorMode.Blue)
        {
            ResetColorSelection();
            return;
        }

        // 다른 기준색에서 B를 눌렀을 때 섞기
        if (baseColor == ColorMode.Red)
        {
            currentMode = ColorMode.Purple;
            OnColorChanged?.Invoke(currentMode);
        }
        else if (baseColor == ColorMode.Green)
        {
            currentMode = ColorMode.Cyan;
            OnColorChanged?.Invoke(currentMode);
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
