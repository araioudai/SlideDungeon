using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageIndex : MonoBehaviour
{
    #region �V���O���g��
    public static StageIndex Instance { get; private set; }
    #endregion

    #region private�ϐ�
    private int stageIndex; //�X�e�[�W�ԍ�
    private bool firstTime; //�ŏ��̃v���C��������`���[�g���A��
    #endregion

    #region Set�֐�
    //�X�e�[�W�ԍ��Z�b�g
    public void SetIndex(int index) { stageIndex = index; }

    //�X�e�[�W�ԍ������ցi�����L���O�p�l���Ȃǁj
    public void SetNextIndex(int index) { stageIndex += index; if (stageIndex > 14) stageIndex = 1; }

    //�X�e�[�W�ԍ���O�ցi�����L���O�p�l���Ȃǁj
    public void SetBeforeIndex(int index) { stageIndex -= index; if (stageIndex < 1) stageIndex = 14; }

    //�ŏ��̃v���C���ǂ����Z�b�g�p
    public void SetFirst(bool first) { firstTime = first; }
    #endregion

    #region Get�֐�
    //�X�e�[�W�ԍ�����p
    public int GetIndex() { return stageIndex; }

    //�ŏ��̃v���C���ǂ�������p
    public bool GetFirst() {  return firstTime; }

    #endregion

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Init()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene")
        {
            firstTime = false;
        }
    }
}
