using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private bool isUnlocked = false;

    [SerializeField] private PlayerData playerData;

    public void UnlockPortal() => isUnlocked = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isUnlocked) return;
        if (!other.CompareTag("Player")) return;

        if (nextSceneName == "Stage2")
        {
            ApplyStage2Bonus();
        }

        // 페이드 매니저 호출
        FadeTransition.Instance.FadeToScene(nextSceneName);
    }

    private void ApplyStage2Bonus()
    {
        if (playerData.stage2Applied) return;

        playerData.stage2Applied = true;

        // 최대 체력 증가
        playerData.baseHP += 50f;

        // 공격 배율 증가
        playerData.damageMultiplier *= 1.2f;
    }

}