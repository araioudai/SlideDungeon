using UnityEngine;

public class RankingReset : MonoBehaviour
{
    #region ������o�R���ČĂԂ��Ƃŏ�ɐ������C���X�^���X���Ăׂ�悤�ɂ���
    public void OnClickReset()
    {
        if (OffLineRankingManager.Instance != null)
        {
            OffLineRankingManager.Instance.ResetAllRanking();
        }
        else
        {
            Debug.LogWarning("OffLineRankingManager.Instance �����݂��܂���");
        }
    }
    #endregion
}
