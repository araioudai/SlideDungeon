using UnityEngine;
using UnityEngine.Tilemaps;

public class StageLoader : MonoBehaviour
{
    #region private�ϐ�
    [SerializeField] private Tilemap floorTilemap;   //�n�ʗpTilemap
    [SerializeField] private Tilemap wallTilemap;    //�ǗpTilemap
    [SerializeField] private TileBase floorBackTile; //�؏��^�C��
    [SerializeField] private TileBase wallRockTile;  //�Ώ��^�C��
    [SerializeField] private GameObject goal;        //�S�[�������p
    [SerializeField] private GameObject hole;

    [Header("����ړ��p�X���C�h���Z�b�g")]
    [SerializeField] private GameObject slider;             //�X�e�[�W������ړ�����K�v�Ȃ����p

    private int stageIndex;
    #endregion

    #region public�ϐ�
    public string csvFileName;                       //csv�t�@�C����
    #endregion

    #region Unity�C�x���g�֐�
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

    #region Start�Ăяo���֐�

    #region ������
    void Init()
    {
        stageIndex = StageIndex.Instance.GetIndex();
        csvFileName = "Stage" + stageIndex;
        LoadMapFromCSV(csvFileName);
    }
    #endregion

    #region �X�e�[�W�ǂݍ���

    void LoadMapFromCSV(string fileName)
    {
        TextAsset csvFile = Resources.Load<TextAsset>(fileName);
        if (csvFile == null)
        {
            Debug.LogError("CSV�t�@�C����������܂���: " + fileName);
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
        // ��ɂ���Ă镪��␳
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
                        //0�͉����u���Ȃ�
                        //1�`9�̓X�e�[�W�p�^�C���}�b�v
                        //10�`19�͓G�p

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
