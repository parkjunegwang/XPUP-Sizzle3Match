using UnityEngine;
using UnityEngine.Tilemaps;


public class EditController : MonoBehaviour
{
    //	, IPointerDownHandler

	private enum EBubbleEditMode
    {
        Add,
        Remove,
    }
    [SerializeField] private Tilemap grid; // Grid 오브젝트 연결

    [SerializeField]
    GameObject Prefab;

    [SerializeField] private Tilemap overlayTilemap; // 그리드 선용 타일맵
    [SerializeField] private GameObject gridTile;      // 테두리만 있는 타일 (스프라이트)

    void Start()
    {
        // baseTilemap의 cellBounds 기준으로 그리드 타일 깔기
       

        for (int x = 0; x < 25; x++)
        {
            for (int y = 0; y < 25; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                Vector3 worldPos = grid.GetCellCenterWorld(pos);
                // 실제로 쓰는 타일이 있는 칸에만 그리드 깔고 싶으면:
                var bubbleInstance = GameObject.Instantiate(gridTile);//, bubbleCenter, Quaternion.identity);
                bubbleInstance.transform.parent = transform;
                bubbleInstance.transform.localPosition = worldPos;
                // 맵 전체 네모로 그리드 깔고 싶으면 위 if문 없애면 됨.
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;

            Vector3Int cellPos = grid.WorldToCell(mouseWorldPos);
            Debug.Log($"그리드 셀 좌표: {cellPos} (x:{cellPos.x}, y:{cellPos.y})");
            // 3. 셀 좌표 → 월드 좌표(타일 중앙)
            Vector3 worldPos = grid.GetCellCenterWorld(cellPos);

            _OnSelectedCell(new Vector2(worldPos.x, worldPos.y));
        }
    }
    private void _OnSelectedCell(Vector2 _cell)
    {
        var bubbleInstance = GameObject.Instantiate(Prefab);//, bubbleCenter, Quaternion.identity);
        bubbleInstance.transform.parent = transform;
        bubbleInstance.transform.localPosition = _cell;
    }
 
}
