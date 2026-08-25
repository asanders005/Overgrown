using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIFollowWorldObject : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 offset;

    private RectTransform rectTransform;
    private Camera mainCamera;
    private Canvas canvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = FindAnyObjectByType<Canvas>();
        rectTransform.SetParent(canvas.transform, false);

        mainCamera = Camera.main;

        if (canvas == null)
        {
            Debug.LogError("Canvas is not assigned in UIFollowWorldObject script on " + gameObject.name);
        }
    }

    private void LateUpdate()
    {
        if (targetTransform == null || canvas == null || mainCamera == null)
        {
            return;
        }

        Vector3 screenPos = mainCamera.WorldToScreenPoint(targetTransform.position + offset);

        if (screenPos.z < 0)
        {
            // If the target is behind the camera, hide the UI element
            rectTransform.gameObject.SetActive(false);
        }
        else
        {
            // If the target is in front of the camera, show the UI element and update its position
            rectTransform.gameObject.SetActive(true);
            rectTransform.position = screenPos;
        }

    }
}
