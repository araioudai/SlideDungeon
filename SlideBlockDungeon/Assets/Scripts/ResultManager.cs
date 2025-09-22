using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    #region private�ϐ�

    [Header("�X�R�A�e�L�X�g�Z�b�g")]
    [SerializeField] private Text scoreText;
    [Header("�����N�e�L�X�g�Z�b�g")]
    [SerializeField] private Text rankText;
    [Header("�����N���[�h�e�L�X�g�Z�b�g")]
    [SerializeField] private Text wordsText;
    [Header("�Q�[���N���A���\���{�^���Z�b�g")]
    [SerializeField] private GameObject GameClear;
    [Header("�Q�[���I�[�o�[���\���{�^���Z�b�g")]
    [SerializeField] private GameObject GameOver;
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
        clearTime = GameManager.Instance.GetClearTimer();
        slideCount = GameManager.Instance.GetSlideCount();
        scoreText.text = "�N���A�@: " + clearTime.ToString("F2") + "\n\n�X���C�v: " + slideCount;
        RankMeasurement();
        if (GameManager.Instance.GetGameClear()) { GameOver.SetActive(false); GameClear.SetActive(true); }
        else { GameClear.SetActive(false); GameOver.SetActive(true); rankText.text = ""; wordsText.text = ""; }
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

    #region �^�C�g����
    public void PushTitle()
    {
        seSource.PlayOneShot(seDecision);
        StageIndex.Instance.SetIndex(0);
        StartCoroutine(TitleSceneLoad());
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
