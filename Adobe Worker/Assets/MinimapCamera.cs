using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 cameraOffset;

    [Tooltip("0~1 ������ ��")]
    [SerializeField] private float smoothSpeed = 0.125f;

    private void Start()
    {
        GameObject playerGameObject = AdobePlayerReference.playerInstance;
        if (player == null && playerGameObject != null)
        {
            player = playerGameObject.transform;
        }
    }

    void Update()
    {
        Vector3 desiredPosition = player.position + cameraOffset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
