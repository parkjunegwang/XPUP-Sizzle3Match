using UnityEngine;
using UnityEngine.UIElements;

[ExecuteInEditMode]
public class SafeAreaBorder : MonoBehaviour
{
    [SerializeField] UIDocument m_document;
    [SerializeField] Color m_borderColor = Color.black;
    [SerializeField] string m_element;
    [Range(0, 1f)]
    [SerializeField] float m_multiplier = 1f;
    
    VisualElement m_root;

    float m_leftBorder;
    float m_rightBorder;
    float m_topBorder;
    float m_bottomBorder;



    public VisualElement RootElement => m_root;
    public float LeftBorder => m_leftBorder;
    public float RightBorder => m_rightBorder;
    public float TopBorder => m_topBorder;
    public float BottomBorder => m_bottomBorder;

    public float Multiplier { get => m_multiplier; set => m_multiplier = value; }




    void Awake() => Initialize();

    public void Initialize()
    {

        if (m_document == null || m_document.rootVisualElement == null)
            return;
        
        if (string.IsNullOrEmpty(m_element))
            m_root = m_document.rootVisualElement;
        else
            m_root = m_document.rootVisualElement.Q<VisualElement>(m_element);
        
        if (m_root == null)
            return;
        
        m_root.RegisterCallback<GeometryChangedEvent>(evt => OnGeometryChangedEvent());

        ApplySafeArea();
    }

    void OnGeometryChangedEvent() => ApplySafeArea();

    void OnValidate() => ApplySafeArea();
        
    void ApplySafeArea()
    {
        if (m_root == null)
            return;

        Rect safeArea = Screen.safeArea;

        m_leftBorder = safeArea.x;
        m_rightBorder = Screen.width - safeArea.xMax;
        m_topBorder = Screen.height - safeArea.yMax;
        m_bottomBorder = safeArea.y;
                        
        m_root.style.borderTopWidth = m_topBorder * m_multiplier;
        m_root.style.borderBottomWidth = m_bottomBorder * m_multiplier;
        m_root.style.borderLeftWidth = m_leftBorder * m_multiplier;
        m_root.style.borderRightWidth = m_rightBorder * m_multiplier;

        m_root.style.borderBottomColor = m_borderColor;
        m_root.style.borderTopColor = m_borderColor;
        m_root.style.borderLeftColor = m_borderColor;
        m_root.style.borderRightColor = m_borderColor;
    }
}