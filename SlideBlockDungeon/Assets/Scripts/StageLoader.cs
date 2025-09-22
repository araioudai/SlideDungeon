using UnityEngine;
using UnityEngine.Tilemaps;

public class StageLoader : MonoBehaviour
{
    #region private変数
    [SerializeField] private Tilemap floorTilemap;   //地面用Tilemap
    [SerializeField] private Tilemap wallTilemap;    //壁用Tilemap
    [SerializeField] private TileBase floorBackTile; //木床タイル
    [SerializeField] private TileBase wallRockTile;  //石床タイル
    [SerializeField] private GameObject goal;        //ゴール生成用
    [SerializeField] private GameObject hole;

    [Header("視野移動用スライドをセット")]
    [SerializeField] private GameObject slider;             //ステージが視野移動する必要ない時用

    private int stageIndex;
    #endregion

    #region public変数
    public string csvFileName;                       //csvファイル名
    #endregion

    #region Unityイベント関数
    // Start is called before the first frame update
    void Start()
    {
        Init();
        //Debug.Log(stageIndex);
    }

    // Update is called once per frame
    void Update()
    {

    }
    #endregion

    #region Start呼び出し関数

    #region 初期化
    void Init()
    {
        stageIndex = StageIndex.Instance.GetIndex();
        csvFileName = "Stage" + stageIndex;
        LoadMapFromCSV(csvFileName);
    }
    #endregion

    #region ステージ読み込み

    void LoadMapFromCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError("CSVファイルが見つかりません: " + fileName);
            return;
        }

        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();

        string[] lines = csvFile.text.Trim().Split('\n');

        Camera cam = Camera.main;
        if (cam == null || !cam.orthographic) return;

        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;
        Vector3 topLeftWorld = cam.transform.position;
        topLeftWorld.x -= camHalfWidth;
        topLeftWorld.y += camHalfHeight;

        Vector3Int topLeftCell = floorTilemap.WorldToCell(topLeftWorld);
        // 上にずれてる分を補正
        topLeftCell.y -= 1;

        for (int y = 0; y < lines.Length; y++)
        {
            string[] values = lines[y].Trim().Split(',');

            for (int x = 0; x < values.Length; x++)
            {
                if (int.TryParse(values[x], out int value))
                {
                    Vector3Int cellPos = new Vector3Int(topLeftCell.x + x, topLeftCell.y - y, 0);
                    Vector3 goalPos = new Vector3(topLeftCell.x + x + 0.5f, topLeftCell.y - y + 0.5f, 0);

                    switch (value)
                    {
                        //0は何も置かない
                        //1～9はステージ用タイルマップ
                        //10～19は敵用

                        case 0:
                            floorTilemap.SetTile(cellPos, floorBackTile);
                            break;
                        case 1:
                            wallTilemap.SetTile(cellPos, wallRockTile);
                            break;
                        case 2:
                            floorTilemap.SetTile(cellPos, floorBackTile);
                            Instantiate(goal, goalPos, Quaternion.identity);
                            break;
                        case 3:
                            floorTilemap.SetTile(cellPos, floorBackTile);
                            Instantiate(hole, goalPos, Quaternion.identity);
                            break;
                    }
                }
            }
        }
    }
    #endregion

    #endregion
}
