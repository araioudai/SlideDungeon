using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

//�����L���O1�����i�v���C���[���ƃN���A�^�C���j
[System.Serializable]
public class ScoreEntry
{
    public string playerName; //�v���C���[��
    public float clearTime;   //�N���A�^�C���i�Z���قǗǂ��j
}

//�e�X�e�[�W�̃����L���O�i���9���܂ŕێ��j
[System.Serializable]
public class StageRanking
{
    public List<ScoreEntry> scores = new List<ScoreEntry>();
}

//�Q�[���S�̂̃����L���O�f�[�^
[System.Serializable]
public class RankingData
{
    public List<StageRanking> stageRankings = new List<StageRanking>();
}

public class OffLineRankingManager : MonoBehaviour
{
    #region �V���O���g��
    public static OffLineRankingManager Instance { get; private set; }
    #endregion

    #region private�ϐ�

    private string filePath;                             //JSON�ۑ���̃p�X
    private RankingData rankingData = new RankingData(); //�S�X�e�[�W���̃����L���O�f�[�^

    #endregion

    #region Unity�C�x���g�֐�
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        filePath = Path.Combine(Application.persistentDataPath, "offline_ranking.json");

        LoadRanking();
    }
    #endregion

    #region �����L���O�֌W

    #region �����L���O�ɓ���邩����
    public bool IsHightScore(int stageIndex, float time)
    {
        EnsureStageExists(stageIndex); //�X�e�[�W��������Ȃ���Ίg��

        var stageRanking = rankingData.stageRankings[stageIndex - 1];
        if (stageRanking.scores.Count < 9) return true; //9�ʖ����Ȃ�K�������

        //��Ԓx���^�C����葁����Γ����
        return time < stageRanking.scores.Last().clearTime;
    }
    #endregion

    #region �X�R�A�ǉ�

    public void AddScore(int stageIndex, string name, float time)
    {
        EnsureStageExists(stageIndex);

        var stageRanking = rankingData.stageRankings[stageIndex - 1];
        stageRanking.scores.Add(new ScoreEntry { playerName = name, clearTime = time });

        // �^�C�����Ƀ\�[�g���ď��9�������c��
        stageRanking.scores = stageRanking.scores
              .OrderBy(e => e.clearTime) //������
              .Take(9)                   //���9���̂ݕۑ�
              .ToList();

        SaveRanking();
    }

    #endregion

    #region �w��X�e�[�W�̃����L���O�擾

    public List<ScoreEntry> GetRanking(int stageIndex)
    {
        EnsureStageExists(stageIndex);

        var stageRanking = rankingData.stageRankings[stageIndex - 1];
        if (stageRanking == null || stageRanking.scores == null)
        {
            return new List<ScoreEntry>(); // ��̃��X�g��Ԃ�
        }

        return stageRanking.scores;
    }

    #endregion

    #region �����L���O��JSON�t�@�C���ɕۑ�

    private void SaveRanking()
    {
        string json = JsonUtility.ToJson(rankingData, true); //true�Ő��`�o��
        File.WriteAllText(filePath, json);
    }

    #endregion

    #region JSON�t�@�C���ǂݍ���

    private void LoadRanking()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            rankingData = JsonUtility.FromJson<RankingData>(json);
        }
        else
        {
            rankingData = new RankingData();
        }
    }

    #endregion

    #region �w��X�e�[�W���̃����L���O���X�g�����݂��邩�m�F�A�Ȃ���Βǉ�
    private void EnsureStageExists(int stageIndex)
    {
        while (rankingData.stageRankings.Count < stageIndex)
        {
            rankingData.stageRankings.Add(new StageRanking());
        }
    }
    #endregion

    #region �����L���O�f�[�^���Z�b�g

    public void ResetAllRanking()
    {
        rankingData = new RankingData(); // �S�X�e�[�W�̃����L���O�f�[�^����ɂ���
        SaveRanking();                   // ��f�[�^�ŏ㏑���ۑ�
        Debug.Log("�����L���O�����ׂă��Z�b�g���܂���");
    }

    #endregion

    #endregion
}
