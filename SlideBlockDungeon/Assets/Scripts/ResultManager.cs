using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    #region

    private const int NEXT = 0;
    private const int ONCEAGAIN = 1;
    private const int TITLE = 2;

    #endregion

    #region private�ϐ�

    [Header("�X�R�A�e�L�X�g�Z�b�g")]
    [SerializeField] private Text scoreText;
    [Header("�����N�e�L�X�g�Z�b�g")]
    [SerializeField] private Text rankText;
    [Header("�����N���[�h�e�L�X�g�Z�b�g")]
    [SerializeField] private Text wordsText;
    [Header("�X�R�A�\���p�p�l���Z�b�g")]
    [SerializeField] private GameObject scorePanel; //�X�R�A�\���p
    [Header("���O���͗p�p�l���Z�b�g")]
    [SerializeField] private GameObject inputPanel; //�����L���O�ɓo�^���閼�O����
    [Header("�Q�[���N���A���\���{�^���Z�b�g")]
    [SerializeField] private GameObject gameClear;
    [Header("�Q�[���I�[�o�[���\���{�^���Z�b�g")]
    [SerializeField] private GameObject gameOver;
    [Header("SE�p�I�[�f�B�I�\�[�X�^�{�̂��Z�b�g")]
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioClip seDecision;
    [Header("�X�e�[�W���Ƃ̃m���}�f�[�^�Z�b�g")]
    [SerializeField] private StageData[] stageDatas;

    private float clearTime;
    private int slideCount;
    private float rank;
    private int stageNumber;
    private float baseTime;
    private float baseSlide;
    private int transition;

    #endregion
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
        clearTime = 0;
        transition = 0;
        clearTime = GameManager.Instance.GetClearTimer();
        slideCount = GameManager.Instance.GetSlideCount();
        scoreText.text = "�N���A�@: " + clearTime.ToString("F2") + "\n\n�X���C�v: " + slideCount;
        RankMeasurement();
        if (GameManager.Instance.GetGameClear()) { gameOver.SetActive(false); gameClear.SetActive(true); }
        else { gameClear.SetActive(false); gameOver.SetActive(true); rankText.text = ""; wordsText.text = ""; }
        int stage = StageIndex.Instance.GetIndex();
        if (OffLineRankingManager.Instance.IsHightScore(stage, clearTime))
        {
            inputPanel.SetActive(true); //���O���̓p�l���\��
            Time.timeScale = 0f;        //���O���͒��͎~�߂�
        }
        else
        {
            scorePanel.SetActive(true);
        }
    }

    void RankMeasurement()
    {
        //���݂̃X�e�[�W�ԍ����擾
        stageNumber = StageIndex.Instance.GetIndex();
        //�Y���X�e�[�W�̊�f�[�^���擾
        StageData data = stageDatas[stageNumber - 1];
        //S�����N��^�C��
        baseTime = data.baseTime;
        //S�����N��X���C�h��
        baseSlide = data.baseSlide;

        //�X�R�A�v�Z
        //�e���ڂ���l�Ő��K������F1.0����B���A1.0�����Ȃ����ǂ�
        float timeNorm = clearTime / baseTime;               //�^�C���̊�B���x
        float slideNorm = (float)slideCount / baseSlide;     //�X���C�h���̊�B���x
        float score = timeNorm + slideNorm;                  //���v�X�R�A�i�������قǍ����сj2.0���s�b�^��

        //�����N����ifloat��臒l�𒲐��j
        //�e�����N��臒l�𒴂��Ȃ����Ŕ���B�l���������قǗǂ������N�ɂȂ�
        if (score <= 2.0f)
        {
            //S�����N�F�^�C���E�X���C�h�Ƃ��Ɋ�l�s�b�^�����A����ȉ��ŃN���A
            rankText.color = new Color32(255, 196, 0, 255);
            wordsText.color = new Color32(255, 196, 0, 255);
            rankText.text = "S";  
            wordsText.text = "�_�b���X���C��";
        }
        else if (score <= 2.2f)
        {
            //A�����N�F��l��菭�������ꍇ
            rankText.color = new Color32(255, 57, 67, 255);
            wordsText.color = new Color32(255, 57, 67, 255);
            rankText.text = "A";   
            wordsText.text = "�p�Y���X���C��";
        }
        else if (score <= 2.4f)
        {
            //B�����N : A��菭�������ꍇ
            rankText.color = new Color32(0, 72, 255, 255);
            wordsText.color = new Color32(0, 72, 255, 255);
            rankText.text = "B";  
            wordsText.text = "�n���X���C��";
        }
        else if (score <= 2.6f)
        {
            //C�����N : B��菭�������ꍇ
            rankText.color = new Color32(0, 255, 40, 255);
            wordsText.color = new Color32(0, 255, 40, 255);
            rankText.text = "C";  
            wordsText.text = "�V�ăX���C��";
        }
        else
        {
            //D�����N�FC��肳��ɃX�R�A���傫���ꍇ
            rankText.color = new Color32(203, 0, 255, 255);
            wordsText.color = new Color32(203, 0, 255, 255);
            rankText.text = "D";  
            wordsText.text = "�X���C���̗�";
        }
    }

    #region ���̃X�e�[�W��
    public void PushNext()
    {
        seSource.PlayOneShot(seDecision);
        StageIndex.Instance.SetNextIndex(1);
        SceneManager.LoadScene("GameScene");
    }
    #endregion

    #region �Q�[�����X�^�[�g

    public void GameReStart()
    {
        seSource.PlayOneShot(seDecision);
        StartCoroutine(GameSceneLoad());
    }

    #endregion

    #region �^�C�g���ւ������ꂽ

    public void PushTitle()
    {
        seSource.PlayOneShot(seDecision);
        StageIndex.Instance.SetIndex(0);
        StartCoroutine(TitleSceneLoad());
    }

    #endregion

    #region ���O�����肳�ꂽ��

    public void NameEnter()
    {
        //�����L���O�o�^����
        int stage = StageIndex.Instance.GetIndex();
        OffLineRankingManager.Instance.AddScore(stage, InputManager.playerName, clearTime);

        //���̓p�l������Ď��Ԃ��ĊJ
        inputPanel.SetActive(false);
        Time.timeScale = 1f;

        seSource.PlayOneShot(seDecision);
        scorePanel.SetActive(true);
    }

    #endregion

    #region �Q�[���V�[�����[�h�x���p
    IEnumerator GameSceneLoad()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameScene");
    }
    #endregion

    #region �^�C�g���V�[�����[�h�x���p
    IEnumerator TitleSceneLoad()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("TitleScene");
    }
    #endregion
}
