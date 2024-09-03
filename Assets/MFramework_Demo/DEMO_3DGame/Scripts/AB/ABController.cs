using MFramework;

public class ABController : ComponentSingleton<ABController>
{
    public bool enableAB = true;

    protected override void Awake()
    {
        base.Awake();
        ResourceManager.Instance.Initialize(MABUtility.GetPlatform(), AB.GetFileUrl, 0);
    }

    protected virtual void Update()
    {
        if (enableAB)
        {
            ResourceManager.Instance.Update();
        }
    }

    protected virtual void LateUpdate()
    {
        if (enableAB)
        {
            ResourceManager.Instance.LateUpdate();
        }
    }
}
