using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    #region シングルトン
    public static CameraManager Instance { get; private set; } //他のスクリプトからInstanceでアクセスできるようにする
    #endregion

    #region private変数

    [Header("スライダーをセット")]
    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform sliderRange;
    [Header("プレイヤーをセット")]
    [SerializeField] private Transform target;                         //追従対象（プレイヤーのTransform）
    [Header("デッドゾーンのX・Y範囲")]
    [SerializeField] private Vector2 deadZone = new Vector2(2f, 1.5f); //デッドゾーンのX・Y範囲（中心からの距離）
    [Header("追従速度をセット")]
    [SerializeField] private float followSpeed = 5f;                   //追従速度（Lerpの係数）

    private bool isSlider;

    #endregion
    
    #region Get関数

    public bool GetSlide() { return isSlider; }

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
    }

    // Start is called before the first frame update
    void Start()
    {
        //onValueChangedイベントにメソッドを登録する
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(isSlider);
        TapPosCheck();
        CameraPos();
    }

    void LateUpdate()
    {
        if (target == null) return;                          //追従対象がセットされていなければ処理終了
        if (!GameManager.Instance.GetIsFollow()) { return; } //追従する必要がないステージなら処理終了
        if (isSlider) { return; }

        Vector3 pos = transform.position;
        Vector3 tpos = target.position;

        // ------ X 軸の処理 ------
        float dx = tpos.x - pos.x;
        if (Mathf.Abs(dx) > deadZone.x)
        {
            // デッドゾーン外に出ている場合のみLerpで滑らかに追従
            pos.x = Mathf.Lerp(pos.x,
                               tpos.x - Mathf.Sign(dx) * deadZone.x,
                               followSpeed * Time.deltaTime);
        }

        // ------ Y 軸の処理（必要なら） ------
        /*float dy = tpos.y - pos.y;
        if (Mathf.Abs(dy) > deadZone.y)
        {
            pos.y = Mathf.Lerp(pos.y,
                               tpos.y - Mathf.Sign(dy) * deadZone.y,
                               followSpeed * Time.deltaTime);
        }*/
        // カメラ位置を更新（Z軸は維持）
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);
    }

    #endregion


    #region Start呼び出し関数


    #endregion

    #region Update呼び出し関数

    #region カメラ追従
    void CameraPos()
    {
        if (isSlider)
        {
            //スライダーの値(0～1)を 0～5 のワールド座標に変換
            float x = Mathf.Lerp(0f, 5f, slider.value);

            //カメラの位置を更新
            transform.position = new Vector3(x, 0, -10);
        }
        else
        {
            if (GameManager.Instance.GetIsFollow())
            {
                //transform.position = new Vector3(player.transform.position.x, transform.position.y, -10);
            }
        }
    }
    #endregion

    #endregion

    #region Slider値が変更されたときに呼び出されるメソッド

    //Sliderの値が変更されたときに呼び出されるメソッド
    public void OnSliderValueChanged(float value)
    {
        Debug.Log("Sliderの値が変更されました: " + value);

        isSlider = true;
    }

    #endregion

    #region タップ場所判定

    void TapPosCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //タップした位置
            Vector2 tapPos = Input.mousePosition;

            //スライダー範囲に含まれているか
            bool inSlider = RectTransformUtility.RectangleContainsScreenPoint(
                sliderRange, //対象となるUI要素
                tapPos,      //タップ/クリックされたスクリーン座標
                Camera.main  // UIを描画しているカメラ
            );

            if (!inSlider)
            {
                //スライダー以外の場所をタップ時にfalse
                isSlider = false;
            }
            else
            {
                //一応スライダー判定をこっちでもしておく
                isSlider = true;
            }
        }
    }

    #endregion
}
