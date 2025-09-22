using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region �V���O���g��
    public static SoundManager Instance { get; private set; }
    #endregion

    #region private�ϐ�

    #region BGM
    [Header("BGM�p�I�[�f�B�I�\�[�X�Z�b�g")]
    [SerializeField] private AudioSource bgmSource;
    [Header("BGM�p�I�[�f�B�I�i�{�́j�Z�b�g")]
    [SerializeField] private AudioClip[] bgmClip;
    #endregion

    #region SE
    [Header("SE�p�I�[�f�B�I�\�[�X�Z�b�g")]
    [SerializeField] private AudioSource seSource;
    [Header("SE�p�I�[�f�B�I�i�{�́j�Z�b�g")]
    [SerializeField] private AudioClip[] seClip;
    #endregion

    private int number;

    #endregion

    #region Unity�C�x���g�֐�
    void Awake()
    {
        //�V���O���g���Ǘ�
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //����Instance������Ύ�����j��
            return;
        }
        Instance = this;
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

    #endregion

    #region Start�Ăяo���֐�
    void Init()
    {
        //number = StageIndex.Instance.GetIndex();
        //minutes�p�f�[�^�ߖ�
        number = 1;
        BgmPlay(number);
    }
    #endregion

    void BgmPlay(int number)
    {
        bgmSource.PlayOneShot(bgmClip[number-1]);
/*        if (GameManager.Instance.GetGameClear())
        {
            bgmSource.Stop();
        }*/
    }

    public void BgmStop()
    {
        bgmSource.Stop();
    }

    public void SePlay(int number)
    {
        seSource.PlayOneShot(seClip[number]);
    }
}
