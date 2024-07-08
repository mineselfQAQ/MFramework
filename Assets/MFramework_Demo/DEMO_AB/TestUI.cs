using MFramework;
using UnityEngine;
using UnityEngine.UI;

public class TestUI : MonoBehaviour
{
    private string[] m_Backgrounds = new string[]
    {
        "Assets/AssetBundle/Background/1.png",
        "Assets/AssetBundle/Background/2.png",
        "Assets/AssetBundle/Background/3.png",
        "Assets/AssetBundle/Background/4.png",
        "Assets/AssetBundle/Background/5.png",
        "Assets/AssetBundle/Background/6.png",
        "Assets/AssetBundle/Background/7.png",
    };


    private string[] m_Roles = new string[]
    {
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_000.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_001.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_002.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_003.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_004.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_005.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_006.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_007.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_008.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_009.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_010.png",
        "Assets/AssetBundle/Atlas/Role/Hog_Attack_011.png",
    };

    private string[] m_Icons = new string[]
    {
        "Assets/AssetBundle/Icon/1.png",
        "Assets/AssetBundle/Icon/2.png",
        "Assets/AssetBundle/Icon/3.png",
        "Assets/AssetBundle/Icon/4.png",
        "Assets/AssetBundle/Icon/5.png",
        "Assets/AssetBundle/Icon/6.png",
        "Assets/AssetBundle/Icon/7.png",
        "Assets/AssetBundle/Icon/8.png",
        "Assets/AssetBundle/Icon/9.png",
        "Assets/AssetBundle/Icon/10.png",
        "Assets/AssetBundle/Icon/11.png",
        "Assets/AssetBundle/Icon/12.png",
        "Assets/AssetBundle/Icon/13.png",
        "Assets/AssetBundle/Icon/14.png",
        "Assets/AssetBundle/Icon/15.png",
        "Assets/AssetBundle/Icon/16.png",
        "Assets/AssetBundle/Icon/17.png",
        "Assets/AssetBundle/Icon/18.png",
        "Assets/AssetBundle/Icon/19.png",
    };

    private string m_ModelUrl = "Assets/AssetBundle/Model/Ji.prefab";
    [SerializeField]
    private Transform m_ModelRoot;
    private GameObject m_ModelGO;
    private IResource m_ModelResource;

    [SerializeField]
    private RawImage m_RawImage_Background = null;
    [SerializeField]
    private Image m_Image_Bear = null;
    [SerializeField]
    private RawImage m_RawImage_Icon = null;

    private int m_BackgourndIndex = -1;
    private int m_BearIndex = -1;
    private int m_IconIndex = -1;


    // Use this for initialization
    void Start()
    {
        m_BackgourndIndex = -1;
        m_BearIndex = -1;
        m_IconIndex = -1;
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 切换背景的sprite
    /// </summary>
    public void OnChangeBackground()
    {
        if (m_Backgrounds.Length == 0)
            return;

        m_BackgourndIndex = ++m_BackgourndIndex % m_Backgrounds.Length;

        string backgroundUrl = m_Backgrounds[m_BackgourndIndex];

        //同步加载背景的sprite
        IResource resource = ResourceManager.Instance.Load(backgroundUrl, false);
        m_RawImage_Background.texture = resource.GetAsset() as Texture;
    }

    /// <summary>
    /// 切换人物的sprite
    /// </summary>
    public void OnChangeBear()
    {
        if (m_Roles.Length == 0)
            return;

        m_BearIndex = ++m_BearIndex % m_Roles.Length;

        string bearUrl = m_Roles[m_BearIndex];

        //同步加载人物的sprite
        IResource resource = ResourceManager.Instance.Load(bearUrl, false);
        m_Image_Bear.sprite = resource.GetAsset<Sprite>();
    }

    /// <summary>
    /// 切换道具图标
    /// </summary>
    public void OnChangeIcon()
    {
        if (m_Icons.Length == 0)
            return;

        m_IconIndex = ++m_IconIndex % m_Roles.Length;
        string iconUrl = m_Icons[m_IconIndex];

        //同步加载icon
        IResource resource = ResourceManager.Instance.Load(iconUrl, false);
        m_RawImage_Icon.texture = resource.GetAsset<Texture>();
    }

    /// <summary>
    /// 加载模型
    /// </summary>
    public void OnLoadModel()
    {
        if (m_ModelResource != null)
            return;

        //同步加载
        m_ModelResource = ResourceManager.Instance.Load(m_ModelUrl, false);
        m_ModelGO = m_ModelResource.Instantiate(m_ModelRoot, false);
        m_ModelGO.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    /// <summary>
    /// 卸载模型
    /// </summary>
    public void OnUnloadModel()
    {
        if (m_ModelResource == null)
            return;

        ResourceManager.Instance.Unload(m_ModelResource);
        m_ModelResource = null;
        if (m_ModelGO)
        {
            Destroy(m_ModelGO);
            m_ModelGO = null;
        }
    }
}
