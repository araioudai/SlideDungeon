using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    #region �V���O���g��
    public static GameManager Instance { get; private set; } //���̃X�N���v�g����Instance�ŃA�N�Z�X�ł���悤�ɂ���
    #endregion

    #region SE�̒萔

    private const int DECISION = 1;
    private const int CANCEL = 2;
    private const int GAMECLEAR = 3;
    private const int GAMEOVER = 4;

    #endregion

    #region private�ϐ�

    //[Header("�v���C���[���Z�b�g")]
    //[SerializeField] private Transform player;              //�v���C���[�ʒu���擾�p
    //[Header("�J�������Z�b�g")]
    //[SerializeField] private GameObject mainCamera;         //�J�����I�u�W�F�N�g�擾�p
    [Header("�X���C�v���\���p�e�L�X�g�Z�b�g")]
    [SerializeField] private Text swipeText;                //�X���C�v���\���p
    [Header("���ԕ\���p�e�L�X�g���Z�b�g")]
    [SerializeField] private Text timeText;                 //���ԕ\���p
    [Header("�Q�[���p�l�����Z�b�g")]
    [SerializeField] private GameObject gamePanel;          //�Q�[���p�l���\���E��\���p
    [Header("�|�[�Y�p�l�����Z�b�g")]
    [SerializeField] private GameObject pausePanel;         //�|�[�Y�p�l���\���E��\���p
    [Header("�Q�[���N���A�p�l�����Z�b�g")]
    [SerializeField] private GameObject gameClearPanel;     //�Q�[���N���A�p�l���\���E��\���p
    [Header("�Q�[���I�[�o�[�p�l�����Z�b�g")]
    [SerializeField] private GameObject gameOverPanel;      //�Q�[���I�[�o�[�p�l���\���E��\���p
    [Header("����ړ��p�X���C�_�[�Z�b�g")]
    [SerializeField] private GameObject slider;             //�X���C�_�[���K�v����Ȃ��X�e�[�W�ł͔�\��

    private bool hitCheckFirst;                             //�|�[�^��1�ƃv���C���[�̓����蔻��
    private bool hitCheckSecond;                            //�|�[�^��2�ƃv���C���[�̓����蔻��
    //private bool playerMove;                                //�v���C���[�������Ă��邩
    private bool playerSwiped;                              //�v���C���[���V�����X���C�v������
    private bool gameClear;                                 //�Q�[���N���A�������ǂ���
    private bool gameOver;                                  //�Q�[���I�[�o�[�������ǂ���
    private bool isPause;                                   //���݃|�[�Y�����ǂ���
    private bool sePlay;                                    //se�̍Đ���x����
    private bool tutorial;                                  //���񊎁A�X�e�[�W��4�ȏゾ������`���[�g���A�����s
    private bool isFollow;                                  //�v���C���[��Ǐ]����K�v�����邩�ǂ���
    private int slideCount;                                 //�X���C�v�����J�E���g�p
    private float timer;                                    //�N���A���Ԍv���p
    private float clearTimeResult;                          //�N���A���̃^�C����ۑ�

    #endregion

    #region Set�֐�
    //�|�[�^���i�ǂ��炩�j�ƃv���C���[���������t���O�Z�b�g�֐�
    public void SetHitCheck(bool hitFirst, bool hitSecond) { hitCheckFirst = hitFirst; hitCheckSecond = hitSecond; }
    
    //�v���C���[���ړ����Ă��邩�̃t���O�Z�b�g�֐�
    //public void SetPlayerMove(bool move) { playerMove = move; }
    
    //�v���C���[�����[�v������ɃX���C�v�������ǂ����̃Z�b�g�֐�
    public void SetPlayerSwiped(bool swiped) { playerSwiped = swiped; }
    
    //�Q�[���N���A�������A�Q�[���I�[�o�[�������t���O�Z�b�g�p
    public void SetGameClear(bool clear) { gameClear = clear; }
    public void SetGameOver(bool over) {  gameOver = over; }

/*    public bool TryGetClearTime(out float clearTime)
    {
        if (gameClear)
        {
            clearTime = clearTimeResult;
            return true;
        }
        clearTime = 0f;
        return false;
    }*/

    #endregion

    #region Get�֐�
    //�|�[�^���ƃv���C���[�ǂ����Ɠ����������t���O�Q�b�g�֐�
    public bool GetHitCheckFirst() {  return hitCheckFirst; }
    public bool GetHitCheckSecond() {  return hitCheckSecond; }
    
    //�v���C���[���ړ����Ă��邩�̃t���O�Q�b�g�֐�
    //public bool GetPlayerMove() {  return playerMove; }
    
    //�v���C���[�����[�v������ɃX���C�v�������ǂ����̃Q�b�g�֐�
    public bool GetPlayerSwiped() { return playerSwiped; }
    
    //�Q�[���N���A���Ă��邩�̃t���O�Q�b�g�֐�
    public bool GetGameClear() { return gameClear; }
    
    //���݃|�[�Y�����ǂ����̃t���O�Q�b�g�֐�
    public bool GetPause() { return isPause; }
    
    //�Ǐ]�K�v�����邩�ǂ����̃t���O�Q�b�g�֐�
    public bool GetIsFollow() { return isFollow; }

    //�N���A���̃^�C���擾
    public float GetClearTimer() { if (gameClear) { return clearTimeResult; } return 0; }

    //�N���A���̃X���C�h���擾
    public int GetSlideCount() { if (gameClear) { return slideCount; } return 0; }

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
        //DontDestroyOnLoad(gameObject);  //�V�[���؂�ւ��Ŕj�����Ȃ�

    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    
    // Update is called once per frame
    void Update()
    {
        /*        Debug.Log(slideCount);
                Debug.Log(hitCheck);*/

        //CameraPos();
        DrawSwipe();
        ClearTimeCount();
        DrawTimer();
        //FirstTutorial();
        DrawClear();
        DrawOver();
        GameClear();
        GameOver();
    }
    #endregion

    #region Start�Ăяo���֐�

    void Init()
    {
        //Time.timeScale = 0f; //�Q�[���J�n���Ɏ��Ԃ��ꎞ��~
        Time.timeScale = 1f;
        isPause = false;
        gameClear = false;
        gameOver = false;
        sePlay = false;
        playerSwiped = false;
        slideCount = 0;
        timer = 0f;
        clearTimeResult = 0f;
        gamePanel.SetActive(true);
        gameClearPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);

        SliderOff();

        //�Q�[���J�n���Ɉ�x����Renderer��S�擾���ă��X�g��
        /*        renderers.AddRange(FindObjectsOfType<Renderer>());
                foreach (var r in renderers)
                {
                    //�eRenderer�̌���sortingOrder��ۑ����Ă���
                    defaultOrders[r] = r.sortingOrder;
                }*/
    }

    void SliderOff()
    {
        int index = StageIndex.Instance.GetIndex();
        switch (index){
            case 1: case 2: case 3:
                isFollow = false;
                slider.SetActive(false);
                break;
            default:
                tutorial = true;
                isFollow = true;
                slider.SetActive(true);
                break;
        }
    }

    #endregion

    #region Update�Ăяo���֐�

/*    #region ����͉��p
    void IncorrectInput()
    {
        timeElapsed += Time.unscaledDeltaTime; //Time.unscaledDeltaTime���g�p�A���ꂾ��timeScale��0�ł����܂�����

        if (timeElapsed >= delayBeforePlay)
        {
            Time.timeScale = 1f; //�w�肵�����Ԃ��o�߂����玞�Ԃ����ɖ߂�
        }
    }

    #endregion*/

    #region �X���C�v���J�E���g

    public void SlideCount(int value)
    {
        slideCount += value;
    }

    #endregion

    #region �X���C�v���\��

    void DrawSwipe()
    {
        swipeText.text = "�X���C�v:" + slideCount.ToString("D2");
    }

    #endregion

    #region �N���A���ԃJ�E���g

    void ClearTimeCount()
    {
        timer += Time.deltaTime;
    }

    #endregion

    #region �X�R�A�p�̎��ԕ\��

    void DrawTimer()
    {
        timeText.text = "����: " + timer.ToString("F2");
    }

    #endregion

    #region �`���[�g���A���p

    void FirstTutorial()
    {
        if (!tutorial) { return; }
        if (!StageIndex.Instance.GetFirst())
        {
            
        }
    }

    #endregion

    #region �|�[�Y�p�l���̕\���E��\��

    public void DrawPause()
    {
        //�����~����
        Time.timeScale = 0f;
        isPause = true;
        gameClearPanel.SetActive(false);
        pausePanel.SetActive(true);
        SoundManager.Instance.SePlay(DECISION);
    }

    public void ClosePause()
    {
        //����J�n����
        Time.timeScale = 1f;
        isPause = false;
        pausePanel.SetActive(false);
        SoundManager.Instance.SePlay(CANCEL);
    }

    #endregion

    #region �N���A�^�I�[�o�[�p�l���̕\��

    void DrawClear()
    {
        if (gameClear)
        {
            gamePanel.SetActive(false);
            gameOverPanel.SetActive(false);
            gameClearPanel.SetActive(true);
        }
    }

    void DrawOver()
    {
        if (gameOver)
        {
            gamePanel.SetActive(false);
            gameClearPanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }
    }

    #endregion

    #region �Q�[���N���A�^�I�[�o�[���̏���

    void GameClear()
    {
        if (gameClear)
        {
            SoundManager.Instance.BgmStop();
            if (!sePlay)
            {
                clearTimeResult = timer;
                if (DebugMode.Instance.GetDebugMode())
                {
                    RankingManager.Instance.SetTime(clearTimeResult);
                }
                else
                {
                    //���U���g�V�[���֑J��
                    StartCoroutine(ResultLoad());
                }
                SoundManager.Instance.SePlay(GAMECLEAR);
                sePlay = true;
            }
        }
    }

    void GameOver()
    {
        if (gameOver)
        {
            SoundManager.Instance.BgmStop();
            if (!sePlay)
            {
                SoundManager.Instance.SePlay(GAMEOVER);
                sePlay = true;
            }
            //���U���g�V�[���֑J��
            StartCoroutine(ResultLoad());
        }
    }

    #endregion

    #region �Q�[�����X�^�[�g

    public void GameReStart()
    {
        SoundManager.Instance.SePlay(DECISION);
        if (Time.timeScale == 0f) { Time.timeScale = 1f; }
        StartCoroutine(GameSceneLoad());
    }

    #endregion

    #region ���������烊�U���g�V�[����

    IEnumerator ResultLoad()
    {
        if (Time.timeScale == 0f) { Time.timeScale = 1f; }
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("ResultScene");
    }

    #endregion

    #region �|�[�Y��ʂŃQ�[���I��
    public void GameEnd()
    {
        SceneManager.LoadScene("TitleScene");
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
    IEnumerator ResultSceneLoad()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("ResultScene");
    }
    #endregion

    #endregion
}
