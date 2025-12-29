using UnityEngine;
using static UnityEngine.UI.Image;

public class EditSceneHandler : MonoBehaviour
{
    public static EditSceneHandler I;

    public int X = 2;
    public int Y = 2;

    public int StageLevel = 1;

    public int StageClearTime = 600;

    public int CompleteEXP = 100;

    public GameObject prefab_Bunner;
    public Transform tr_Bunners;

    private GameObject[] ObjBunners;

    private Vector2 _cell; // 타일 한 칸 크기(월드)

    public void Awake()
    {
        I = this;

        _cell = MeasureTileSize(prefab_Bunner); // 위 함수 사용
      //  SpawnGrid();
    }

    public void Start()
    {
        ObjBunners = new GameObject[X * Y];

        for (int y = 0; y < Y; y++)
        {
            for (int x = 0; x < X; x++)
            {
                float px = (x - (X - 1) / 2f) * (_cell.x + 0.05f);
                float py = (y - (Y - 1) / 2f) * _cell.y;

                Vector2 pos = new Vector2(px, py);
                Instantiate(prefab_Bunner, pos, Quaternion.identity, tr_Bunners);
            }
        }
    }

    private  Vector2 MeasureTileSize(GameObject prefab)
    {
        var go = Instantiate(prefab);
        go.SetActive(true);

        var srs = go.GetComponentsInChildren<SpriteRenderer>(true);
        Bounds b = default;
        bool has = false;

        foreach (var sr in srs)
        {
            if (!has) { b = sr.bounds; has = true; }
            else b.Encapsulate(sr.bounds);
        }

        Destroy(go);
        return has ? (Vector2)b.size : Vector2.one;
    }
}
