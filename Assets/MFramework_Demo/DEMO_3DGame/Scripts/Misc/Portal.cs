using MFramework;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Portal : MonoBehaviour
{
    public bool useFlash = true;
    public Portal exit;
    public float exitOffset = 1f;
    public AudioClip teleportClip;

    protected Collider m_collider;
    protected AudioSource m_audio;

    protected PlayerCamera m_camera;

    protected virtual void Start()
    {
        m_collider = GetComponent<Collider>();
        m_collider.isTrigger = true;

        if (teleportClip) m_audio = gameObject.GetOrAddComponent<AudioSource>();
        m_camera = FindObjectOfType<PlayerCamera>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (exit && other.TryGetComponent(out Player player))
        {
            float yOffset = player.unsizedPosition.y - transform.position.y;

            player.transform.position = exit.transform.position + Vector3.up * yOffset;
            player.FaceDirection(exit.transform.forward);
            m_camera.ResetCamera();

            player.transform.position += player.transform.forward * exit.exitOffset;
            player.lateralVelocity = player.transform.forward * player.lateralVelocity.magnitude;

            if (useFlash)
            {
                UIController.Instance.TriggerFlash();
            }

            if(teleportClip) m_audio.PlayOneShot(teleportClip);
        }
    }
}
