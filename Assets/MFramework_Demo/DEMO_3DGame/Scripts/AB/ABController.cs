using MFramework;

public class ABController : ComponentSingleton<ABController>
{
    protected override void Awake()
    {
        base.Awake();
        ResourceManager.Instance.Initialize(MABUtility.GetPlatform(), AB.GetFileUrl, 0);
    }

    protected virtual void Update()
    {
        ResourceManager.Instance.Update();
    }

    protected virtual void LateUpdate()
    {
        ResourceManager.Instance.LateUpdate();
    }
}
