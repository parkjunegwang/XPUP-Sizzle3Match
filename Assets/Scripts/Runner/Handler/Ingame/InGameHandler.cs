using Assets.Scripts.FrameWork.Job;
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

    private BgFitMode mode = BgFitMode.Stretch;

    private StageData Data;
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

        Data.Data = Resources.Load<SaveData>("StageData/Stage_1");

        Data.insertStageItem();

        for (int i = 0; i < Bunners.transform.childCount; ++i)
        {
            Grill ObjBunner = Bunners.transform.GetChild(i).GetComponent<Grill>();

            ObjBunner.InitializeSlot();
        }

        //총 그릇 갯수를 구한다..
         var remainTrayCount = Data.GetRemainTrayCount();
         var sprayCount = remainTrayCount / Bunners.transform.childCount;
         var sprayRemainCount = remainTrayCount % Bunners.transform.childCount;

        for (int i = 0; i < Bunners.transform.childCount; ++i)
        {
            Grill ObjBunner = Bunners.transform.GetChild(i).GetComponent<Grill>();

            if (sprayRemainCount < 0)
            {
                sprayRemainCount = 0;
            }
            ObjBunner.SetRemaintrayCount(sprayCount + sprayRemainCount);

            sprayRemainCount -= 1;
            //충전 나머지 그릇 갯수
            //ObjBunner.InitializeSlot();
        }
        //
    }
    public void UnLockGrill(IngredientType type)
    {
        if (type == IngredientType.None) return;

        for (int i = 0; i < Bunners.transform.childCount; ++i)
        {
            Bunners.transform.GetChild(i).GetComponent<Grill>().UnLockGrill(type);
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
}
