using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMode : MonoBehaviour
{
    #region �V���O���g��
    public static DebugMode Instance { get; private set; }
    #endregion

    #region private�ϐ�

    [Header("�I�����C�������L���O�Ŏg�����̃Z�b�g")]
    [SerializeField] private GameObject[] onlineRank;
    [Header("�I�t���C�������L���O�Ŏg�����̃Z�b�g")]
    [SerializeField] private GameObject[] offlineRank;
    [Header("ture: �I�����C�������L���O�^false: �I�t���C�������L���O")]
    [SerializeField] private bool debugMode;

    #endregion

    #region Get�֐�

    public bool GetDebugMode() {  return debugMode; }

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
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(debugMode);
    }

    #endregion

    #region Start�Ăяo���֐�

    void Init()
    {
        if (debugMode)
        {
            //�I�t���C�������L���O�Ŏg�����̂��\��
            for (int i = 0; i < offlineRank.Length; i++)
            {
                offlineRank[i].SetActive(false);
            }
            for(int i = 0; i < onlineRank.Length; i++)
            {
                onlineRank[i].SetActive(true);
            }
        }
        else
        {
            //�I�����C�������L���O�Ŏg�����̂��\��
            for (int i = 0; i < onlineRank.Length; i++)
            {
                onlineRank[i].SetActive(false);
            }
            for (int i = 0; i < offlineRank.Length; i++)
            {
                offlineRank[i].SetActive(true);
            }
        }
    }

    #endregion
}
