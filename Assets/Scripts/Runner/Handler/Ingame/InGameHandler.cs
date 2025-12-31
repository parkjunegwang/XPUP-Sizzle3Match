using Assets.Scripts.FrameWork.Job;
using System.Linq;
using Unity.VectorGraphics.Editor;
using Unity.VisualScripting;
using UnityEngine;

public class InGameHandler : MonoBehaviour
{
    public enum BgFitMode
    {
        Cover,   // 화면 꽉 채움 (잘림 가능)
        Contain, // 전체 보임 (여백 가능)
        Stretch  // 왜곡 허용 (완전 꽉 채움)
    }

    public static InGameHandler I; //빠른개발과 편의를 위해 인스턴스하자 시간이없다 크크
    private SpriteRenderer sr;

    public SpriteRenderer GridManager;

    public GameObject Bunners;

    private GameObject prefabGrill;

    private Grill[] ObjBunners;

    private BgFitMode mode = BgFitMode.Stretch;

    private StageData Data;

    private Vector2 _cell; // 타일 한 칸 크기(월드)


    void Awake()
    {
        I = this;

        //  m_fsmLobby = new LobbyFSM(new());

        sr = GetComponent<SpriteRenderer>();
        // m_Player = GameObject.Find("Player");
     
        Resize();
    }
    public void Start()
    {
        Data = JobMaker.GlobalDataBox.GetData<StageData>();

        Data.SaveData = Resources.Load<SaveData>("StageData/Stage_1");

        prefabGrill = Resources.Load<GameObject>("Prefabs/Bunner");

        _cell = MeasureTileSize(prefabGrill); // 위 함수 사용

        

        var GrillPosData = Data.SaveData.GetCheckedPositions();

       for (int y = 0; y < Data.SaveData.height; y++)
       {
           for (int x = 0; x < Data.SaveData.width; x++)
           {
                if (GrillPosData.Contains(new Vector2Int(x, y)) == false) continue;

               float px = (x - (Data.SaveData.width - 1) / 2f) * (_cell.x + 0.05f);
               float py = (y - (Data.SaveData.height - 1) / 2f) * _cell.y;

               Vector2 pos = new Vector2(px, py);
              var grill = Instantiate(prefabGrill, pos, Quaternion.identity, Bunners.transform);

               var itemBunner = grill.GetComponent<Grill>();

               itemBunner.SetType((eTrayType)Data.SaveData.GetType(x, y));
           }
       }
        ObjBunners = new Grill[Bunners.transform.childCount];

        Data.insertStageItem();

        for (int i = 0; i < Bunners.transform.childCount; ++i)
        {
            ObjBunners[i] = Bunners.transform.GetChild(i).GetComponent<Grill>();

            ObjBunners[i].InitializeSlot();
        }

        //총 그릇 갯수를 구한다..
         var remainTrayCount = Data.GetRemainTrayCount();
         var sprayCount = remainTrayCount / Bunners.transform.childCount;
         var sprayRemainCount = remainTrayCount % Bunners.transform.childCount;

        for (int i = 0; i < Bunners.transform.childCount; ++i)
        {
            Grill ObjBunner = Bunners.transform.GetChild(i).GetComponent<Grill>();

            int GetAdd = 0;
            if (sprayRemainCount < 0)
            {
                GetAdd = 0;
            }
            else
            {
                sprayRemainCount -= 1;
                GetAdd = 1;
            }
            ObjBunner.SetRemaintrayCount(sprayCount + GetAdd);

            sprayRemainCount -= 1;
            //충전 나머지 그릇 갯수
            //ObjBunner.InitializeSlot();
        }
        InGameUIHandler.I.Initialize();
    }
    public void UnLockGrill(IngredientType type)
    {
        if (type == IngredientType.None) return;

        for (int i = 0; i < ObjBunners.Length; ++i)
        {
            if (ObjBunners[i].isLock)
            {
                ObjBunners[i].UnLockGrill(type);
            }
           // Bunners.transform.GetChild(i).GetComponent<Grill>().UnLockGrill(type);
        }


    }
    private void Resize()
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null) return;

        var cam = Camera.main;
        if (cam == null) return;

        // 디버그(원인 찾기용)
        // Debug.Log($"aspect={cam.aspect}, res={Screen.width}x{Screen.height}, ortho={cam.orthographicSize}");

        float worldH = cam.orthographicSize * 2f;
        float worldW = worldH * cam.aspect;

        Vector2 spriteSize = sr.sprite.bounds.size; // (월드 단위 기준 원본 크기)

        float sx = worldW / spriteSize.x;
        float sy = worldH / spriteSize.y;

        switch (mode)
        {
            case BgFitMode.Cover:
                // 비율 유지 + 꽉 채움 (잘림 가능)
                float sCover = Mathf.Max(sx, sy);
                transform.localScale = new Vector3(sCover, sCover, 1f);
                break;

            case BgFitMode.Contain:
                // 비율 유지 + 전체 보이기 (여백 가능)
                float sContain = Mathf.Min(sx, sy);
                transform.localScale = new Vector3(sContain, sContain, 1f);
                break;

            case BgFitMode.Stretch:
                // 왜곡 허용 + 완전 꽉 채움
                transform.localScale = new Vector3(sx, sy, 1f);
                GridManager.transform.localScale = new Vector3(sx, sx, 1f);
                break;
        }
    }

    private Vector2 MeasureTileSize(GameObject prefab)
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
