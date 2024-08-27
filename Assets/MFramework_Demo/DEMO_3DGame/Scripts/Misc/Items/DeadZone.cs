using UnityEngine;

public class DeadZone : MonoBehaviour
{
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(GameTags.Player))
        {
            if (other.TryGetComponent<Player>(out var player))
            {
                LevelRespawner.Instance.Respawn(true);
            }
        }
    }
}
