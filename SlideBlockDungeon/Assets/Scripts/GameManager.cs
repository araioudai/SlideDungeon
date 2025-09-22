using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    #region シングルトン
    public static GameManager Instance { get; private set; } //他のスクリプトからInstanceでアクセスできるようにする
    #endregion

    #region SEの定数

    private const int DECISION = 1;
    private const int CANCEL = 2;
    private const int GAMECLEAR = 3;
    private const int GAMEOVER = 4;

    #endregion

    #region private変数

    //[Header("プレイヤーをセット")]
    //[SerializeField] private Transform player;              //プレイヤー位置情報取得用
    //[Header("カメラをセット")]
    //[SerializeField] private GameObject mainCamera;         //カメラオブジェクト取得用
    [Header("スワイプ数表示用テキストセット")]
    [SerializeField] private Text swipeText;                //スワイプ数表示用
    [Header("時間表示用テキストをセット")]
    [SerializeField] private Text timeText;                 //時間表示用
    [Header("ゲームパネルをセット")]
    [SerializeField] private GameObject gamePanel;          //ゲームパネル表示・非表示用
    [Header("ポーズパネルをセット")]
    [SerializeField] private GameObject pausePanel;         //ポーズパネル表示・非表示用
    [Header("ゲームクリアパネルをセット")]
    [SerializeField] private GameObject gameClearPanel;     //ゲームクリアパネル表示・非表示用
    [Header("ゲームオーバーパネルをセット")]
    [SerializeField] private GameObject gameOverPanel;      //ゲームオーバーパネル表示・非表示用
    [Header("視野移動用スライダーセット")]
    [SerializeField] private GameObject slider;             //スライダーが必要じゃないステージでは非表示

    private bool hitCheckFirst;                             //ポータル1とプレイヤーの当たり判定
    private bool hitCheckSecond;                            //ポータル2とプレイヤーの当たり判定
    //private bool playerMove;                                //プレイヤーが動いているか
    private bool playerSwiped;                              //プレイヤーが新しくスワイプしたか
    private bool gameClear;                                 //ゲームクリアしたかどうか
    private bool gameOver;                                  //ゲームオーバーしたかどうか
    private bool isPause;                                   //現在ポーズ中かどうか
    private bool sePlay;                                    //seの再生一度だけ
    private bool tutorial;                                  //初回且、ステージが4以上だったらチュートリアル実行
    private bool isFollow;                                  //プレイヤーを追従する必要があるかどうか
    private int slideCount;                                 //スワイプ数をカウント用
    private float timer;                                    //クリア時間計測用
    private float clearTimeResult;                          //クリア時のタイムを保存

    #endregion

    #region Set関数
    //ポータル（どちらか）とプレイヤーが当ったフラグセット関数
    public void SetHitCheck(bool hitFirst, bool hitSecond) { hitCheckFirst = hitFirst; hitCheckSecond = hitSecond; }
    
    //プレイヤーが移動しているかのフラグセット関数
    //public void SetPlayerMove(bool move) { playerMove = move; }
    
    //プレイヤーがワープした後にスワイプしたかどうかのセット関数
    public void SetPlayerSwiped(bool swiped) { playerSwiped = swiped; }
    
    //ゲームクリアした時、ゲームオーバーした時フラグセット用
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

    #region Get関数
    //ポータルとプレイヤーどっちと当たったかフラグゲット関数
    public bool GetHitCheckFirst() {  return hitCheckFirst; }
    public bool GetHitCheckSecond() {  return hitCheckSecond; }
    
    //プレイヤーが移動しているかのフラグゲット関数
    //public bool GetPlayerMove() {  return playerMove; }
    
    //プレイヤーがワープした後にスワイプしたかどうかのゲット関数
    public bool GetPlayerSwiped() { return playerSwiped; }
    
    //ゲームクリアしているかのフラグゲット関数
    public bool GetGameClear() { return gameClear; }
    
    //現在ポーズ中かどうかのフラグゲット関数
    public bool GetPause() { return isPause; }
    
    //追従必要があるかどうかのフラグゲット関数
    public bool GetIsFollow() { return isFollow; }

    //クリア時のタイム取得
    public float GetClearTimer() { if (gameClear) { return clearTimeResult; } return 0; }

    //クリア時のスライド数取得
    public int GetSlideCount() { if (gameClear) { return slideCount; } return 0; }

    #endregion

    #region Unityイベント関数

    void Awake()
    {
        //シングルトン管理
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); //既にInstanceがあれば自分を破棄
            return;
        }
        Instance = this;
        //DontDestroyOnLoad(gameObject);  //シーン切り替えで破棄しない

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

    #region Start呼び出し関数

    void Init()
    {
        //Time.timeScale = 0f; //ゲーム開始時に時間を一時停止
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

        //ゲーム開始時に一度だけRendererを全取得してリスト化
        /*        renderers.AddRange(FindObjectsOfType<Renderer>());
                foreach (var r in renderers)
                {
                    //各Rendererの元のsortingOrderを保存しておく
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

    #region Update呼び出し関数

/*    #region 誤入力回避用
    void IncorrectInput()
    {
        timeElapsed += Time.unscaledDeltaTime; //Time.unscaledDeltaTimeを使用、これだとtimeScaleが0でもうまくいく

        if (timeElapsed >= delayBeforePlay)
        {
            Time.timeScale = 1f; //指定した時間が経過したら時間を元に戻す
        }
    }

    #endregion*/

    #region スワイプ数カウント

    public void SlideCount(int value)
    {
        slideCount += value;
    }

    #endregion

    #region スワイプ数表示

    void DrawSwipe()
    {
        swipeText.text = "スワイプ:" + slideCount.ToString("D2");
    }

    #endregion

    #region クリア時間カウント

    void ClearTimeCount()
    {
        timer += Time.deltaTime;
    }

    #endregion

    #region スコア用の時間表示

    void DrawTimer()
    {
        timeText.text = "時間: " + timer.ToString("F2");
    }

    #endregion

    #region チュートリアル用

    void FirstTutorial()
    {
        if (!tutorial) { return; }
        if (!StageIndex.Instance.GetFirst())
        {
            
        }
    }

    #endregion

    #region ポーズパネルの表示・非表示

    public void DrawPause()
    {
        //動作停止処理
        Time.timeScale = 0f;
        isPause = true;
        gameClearPanel.SetActive(false);
        pausePanel.SetActive(true);
        SoundManager.Instance.SePlay(DECISION);
    }

    public void ClosePause()
    {
        //動作開始処理
        Time.timeScale = 1f;
        isPause = false;
        pausePanel.SetActive(false);
        SoundManager.Instance.SePlay(CANCEL);
    }

    #endregion

    #region クリア／オーバーパネルの表示

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

    #region ゲームクリア／オーバー時の処理

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
                    //リザルトシーンへ遷移
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
            //リザルトシーンへ遷移
            StartCoroutine(ResultLoad());
        }
    }

    #endregion

    #region ゲームリスタート

    public void GameReStart()
    {
        SoundManager.Instance.SePlay(DECISION);
        if (Time.timeScale == 0f) { Time.timeScale = 1f; }
        StartCoroutine(GameSceneLoad());
    }

    #endregion

    #region 少ししたらリザルトシーンへ

    IEnumerator ResultLoad()
    {
        if (Time.timeScale == 0f) { Time.timeScale = 1f; }
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene("ResultScene");
    }

    #endregion

    #region ポーズ画面でゲーム終了
    public void GameEnd()
    {
        SceneManager.LoadScene("TitleScene");
    }

    #endregion

    #region ゲームシーンロード遅延用
    IEnumerator GameSceneLoad()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("GameScene");
    }
    #endregion

    #region タイトルシーンロード遅延用
    IEnumerator ResultSceneLoad()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("ResultScene");
    }
    #endregion

    #endregion
}
