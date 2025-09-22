using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    #region �V���O���g��
    public static TitleManager Instance { get; private set; } //���̃X�N���v�g����Instance�ŃA�N�Z�X�ł���悤�ɂ���
    #endregion

    #region SE�̒萔

    private const int DECISION = 0;
    private const int CANCEL = 1;

    #endregion

    #region private�ϐ�

    [Header("�^�C�g���p�l���Z�b�g")]
    [SerializeField] private GameObject titlePanel;    //�^�C�g���p�l���\���E��\���p
    [Header("�X�e�[�W�Z���N�g�p�l���Z�b�g")]
    [SerializeField] private GameObject selectPanel;   //�X�e�[�W�Z���N�g�p�l���\���E��\���p
    [Header("�X�e�[�W�Z���N�g�̖��O���̓I�u�W�F�Z�b�g")]
    [SerializeField] private GameObject inputObject;   //�����L���O�ɓo�^���閼�O����
    [Header("�X�e�[�W�Z���N�g�̃I�u�W�F(�{�^���Ȃ�)�Z�b�g")]
    [SerializeField] private GameObject selectObject;  //�X�e�[�W�I���{�^���Ȃ�
    [Header("�����L���O�Z���N�g�p�l���Z�b�g")]
    [SerializeField] private GameObject rankSelect;    //�����L���O�Z���N�g�p�l���\���E��\���p
    [Header("�����L���O�p�l���Z�b�g")]
    [SerializeField] private GameObject rankPanel;     //�����L���O�p�l���\���E��\���p
    [Header("���݂̃����L���O��\���p�e�L�X�g�Z�b�g")]
    [SerializeField] private Text ranking;             //���݂̃����L���O�\��
    [Header("�����L���O�\���p�e�L�X�g�Z�b�g")]
    [SerializeField] private Text[] textRanking;       //�����L���O�\���p
    [Header("���x���Z���N�g��z��ɃZ�b�g")]
    [SerializeField] private GameObject[] levelSelect; //���x���I���̃p�l���i�Q�[���I�u�W�F�N�g�j�ǂ���\�����邩
    [Header("�X�e�[�W�Z���N�g��ʂ�O�փ{�^���Z�b�g")]
    [SerializeField] private GameObject beforeButton;  //�O�փ{�^�����ŏ���1�`9���x���ł͕`�悵�Ȃ��p
    [Header("�X�e�[�W�Z���N�g��ʂ����փ{�^���Z�b�g")]
    [SerializeField] private Button nextButton;
    [Header("SE�p�I�[�f�B�I�\�[�X�^�{�̂��Z�b�g")]
    [SerializeField] private AudioSource button;       //�{�^���炷�p
    [SerializeField] private AudioClip[] buttonClip;   //�{�^��SE�z��

    private GameObject objctName;                      //�I�u�W�F�N�g��
    private GameObject rankName;                       //�I�u�W�F�N�g��

    private int level;                                 //���x���̂ǂ���\�����邩
    private bool isButton;                             //���� or �O�փ{�^���������ꂽ�t���O

    #endregion

    #region Set�֐�

    private void SetRank(int rank) { ranking.text = "���x��" + rank; }

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
        //Debug.Log(level);
        //Debug.Log(levelSelect.Length);
        //Debug.Log(GetIndex());
        UpdateSelect();
    }

    #endregion

    #region Start�Ăяo���֐�

    void Init()
    {
        Application.targetFrameRate = 120;
        level = 0;
        isButton = false;
        if (Time.timeScale == 0f) { Time.timeScale = 1f; }
    }

    #endregion

    #region Update�Ăяo���֐�

    #region �`�悷��X�e�[�W���X�V

    void UpdateSelect()
    {
        //�{�^����������ĂȂ��Ƃ���return
        if (!isButton) { return; }

        //�����ꂽ�{�^���ɑΉ��������̃p�l����\��
        levelSelect[level].SetActive(true);
        
        //�{�^���������ꂽ�t���O���Z�b�g
        isButton = false;
    }

    #endregion

    #endregion

    #region �{�^���Ăяo���֐�

    #region �^�C�g���ŃX�e�[�W�Z���N�g�������ꂽ��

    public void GameSelect()
    {
        button.PlayOneShot(buttonClip[DECISION]);
        titlePanel.SetActive(false);
        rankPanel.SetActive(false);
        selectPanel.SetActive(true);
        inputObject.SetActive(false);
        selectObject.SetActive(true);
    }

    #endregion

    #region �^�C�g���Ń����L���O�������ꂽ��

    public void RankingSelect()
    {
        button.PlayOneShot(buttonClip[DECISION]);
        StageIndex.Instance.SetIndex(1);
        if (DebugMode.Instance.GetDebugMode()) 
        {
            RankingManager.Instance.SetStage(StageIndex.Instance.GetIndex()); //�����̓��x��1�̃����L���O�Z�b�g
            //Debug.Log($"RankingManager.Instance: {RankingManager.Instance}");
            RankingManager.Instance.SetRequest(true);
        }
        else
        {
            //�I�t���C�������L���O����
        }
        titlePanel.SetActive(false);
        selectPanel.SetActive(false);
        rankSelect.SetActive(true);
        rankPanel.SetActive(false);
    }

    #endregion

    #region �����������L���O���x���̉����������ꂽ��

    public void RankingLevelSelect()
    {
        objctName = EventSystem.current.currentSelectedGameObject;
        string name = objctName.name;
        button.PlayOneShot(buttonClip[DECISION]);

        // �X�e�[�W�ԍ��ɕϊ�
        if (name.StartsWith("Stage"))
        {
            string numberPart = name.Replace("Stage", "");

            if (int.TryParse(numberPart, out int number))
            {
                StageIndex.Instance.SetIndex(number); //�I�����ꂽ�X�e�[�W�ԍ���ۑ�
            }
        }
        int stageIndex = StageIndex.Instance.GetIndex();

        //�����L���O�f�[�^�擾
        var rankingList = OffLineRankingManager.Instance.GetRanking(stageIndex);

        //��U�S�N���A
        for (int i = 0; i < textRanking.Length; i++)
        {
            textRanking[i].text = "";
        }

        //���O�ƃX�R�A���Z�b�g�i���ʂ͂��Ȃ��j
        for (int i = 0; i < rankingList.Count && i < textRanking.Length; i++)
        {
            if (i < rankingList.Count)
            {
                var entry = rankingList[i];
                textRanking[i].text = $"{entry.playerName}�@{entry.clearTime:F2}�b"; //�S�p�X�y�[�X
            }
            else
            {
                textRanking[i].text = ""; //�܂��f�[�^���Ȃ��ꍇ�͋�
            }
        }

        SetRank(stageIndex);
        rankSelect.SetActive(false);
        rankPanel.SetActive(true);
    }

    #endregion

    #region �����L���O�p�l������I���p�l���֖߂鎞

    public void ExitRanking()
    {
        button.PlayOneShot(buttonClip[CANCEL]);
        rankPanel.SetActive(false);
        rankSelect.SetActive(true);
    }

    #endregion

    #region ���O�����肳�ꂽ��
    /*
        public void InputName()
        {
            button.PlayOneShot(buttonClip[DECISION]);
            inputObject.SetActive(false);
            selectObject.SetActive(true);
        }
    */
    #endregion

    #region �X�e�[�W�I����ʂŎ��̃X�e�[�W�{�^���������ꂽ��

    public void NextSelect()
    {
        button.PlayOneShot(buttonClip[DECISION]);
        //���x���I���̃}�b�N�X�������玟�̃��x���ւ̃{�^���������Ȃ��悤�ɂ���
        if (level >= levelSelect.Length - 2) { nextButton.interactable = false; }

        //�ŏ��̑I����ʂŎ��փ{�^���������ꂽ���A�O�փ{�^���\��
        if (level == 0) { beforeButton.SetActive(true); }

        //�{�^���������t���Otrue
        isButton = true;

        //�{�^���������ꂽ�����݂̑I����ʂ��\��
        levelSelect[level].SetActive(false);

        //�I����ʂ�1�i�߂�
        level += 1;
    }

    #endregion

    #region �X�e�[�W�I����ʂőO�̃X�e�[�W�{�^���������ꂽ��

    public void BeforeSelect()
    {
        button.PlayOneShot(buttonClip[DECISION]);
        // level ���z��͈͊O�Ȃ� return
        if (level < 0) { return; }

        //���̃��x���ւ̃{�^����������悤�ɂ���
        nextButton.interactable = true;

        //�ŏ��̉�ʂ̂ЂƂO�őO�փ{�^���������ꂽ���A�O�փ{�^����\��
        if (level == 1) { beforeButton.SetActive(false); }

        //�{�^���������t���Otrue
        isButton = true;

        //�{�^���������ꂽ�����݂̑I����ʂ��\��
        levelSelect[level].SetActive(false);
        if (level > 0)
        {
            level -= 1; //0�i�z��̈�ԏ������l�j�ȏゾ������}�C�i�X
        }
    }

    #endregion

    #region �^�C�g���ŃX�e�[�W�̉����������ꂽ��

    public void GameStart()
    {
        objctName = EventSystem.current.currentSelectedGameObject;
        string name = objctName.name;
        button.PlayOneShot(buttonClip[DECISION]);

        //�X�e�[�W�ԍ��ɕϊ�
        if (name.StartsWith("Stage"))
        {
            string numberPart = name.Replace("Stage", "");

            //TryParse�ň��S�ɐ����ɕϊ��i���s���Ă��N���b�V�����Ȃ��j
            if (int.TryParse(numberPart, out int number))
            {
                StageIndex.Instance.SetIndex(number); //�I�����ꂽ�X�e�[�W�ԍ���ۑ�
                if (DebugMode.Instance.GetDebugMode())
                {
                    //Minutes Games Contest�p�R�����g�A�E�g�i�I�����C�������L���O�j
                    RankingManager.Instance.SetStage(StageIndex.Instance.GetIndex()); //�����ŃX�e�[�W�ԍ��̃����L���O�ɐ؂�ւ���
                }
                StartCoroutine(StageLoad());
            }
            else
            {
                //Debug.LogWarning("�X�e�[�W���ɐ��l���܂܂�Ă��܂���: " + name);

                StartCoroutine(TextCountDown());
            }
        }
    }

    IEnumerator StageLoad()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameScene");
    }

    IEnumerator TextCountDown()
    {
        //TextRock.SetActive(true);

        yield return new WaitForSeconds(1.0f); //1�b�҂�

        //TextRock.SetActive(false);
    }
    #endregion

    #region �߂邪�����ꂽ��

    public void Exit()
    {
        button.PlayOneShot(buttonClip[CANCEL]);
        //��U�S����\���ɂ��Ă���
        levelSelect[level].SetActive(false);
        beforeButton.SetActive(false);
        rankSelect.SetActive(false);
        selectPanel.SetActive(false);
        rankPanel.SetActive(false);
        //���x�����������āA�ŏ��̃��x���{�^����\��
        level = 0;
        levelSelect[level].SetActive(true);
        //���̃��x���ւ̃{�^����������悤�ɂ��Ă���
        nextButton.interactable = true;

        titlePanel.SetActive(true);
    }

    #endregion

/*    #region �����L���O�p�l���őO�փ{�^���������ꂽ��

    public void BeforeRanking()
    {
        button.PlayOneShot(buttonClip[DECISION]);
        StageIndex.Instance.SetBeforeIndex(1);
        SetRank(StageIndex.Instance.GetIndex());
        RankingManager.Instance.SetStage(StageIndex.Instance.GetIndex()); //�����Ń����L���O����؂�ւ���
        RankingManager.Instance.SetRequest(true);
    }

    #endregion

    #region �����L���O�p�l���Ŏ��փ{�^���������ꂽ��

    public void NextRanking()
    {
        button.PlayOneShot(buttonClip[DECISION]);
        StageIndex.Instance.SetNextIndex(1);
        SetRank(StageIndex.Instance.GetIndex());
        RankingManager.Instance.SetStage(StageIndex.Instance.GetIndex()); //�����Ń����L���O����؂�ւ���
        RankingManager.Instance.SetRequest(true);
    }

    #endregion*/

    #endregion

}
