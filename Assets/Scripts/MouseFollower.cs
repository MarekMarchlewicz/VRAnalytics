using UnityEngine;

/// <summary>
/// Based on GvrViewer from Google
/// </summary>
public class MouseFollower : MonoBehaviour
{
    [SerializeField, Range(0, 1)]
    private float neckScaleModel;

    private readonly Vector3 neckOffset = new Vector3(0, 0.075f, 0.08f);

    private Quaternion initialRotation = Quaternion.identity;

    private float mouseX = 0;
    private float mouseY = 0;
    private float mouseZ = 0;

    private void Start()
    {
        Cursor.visible = false;
    }

    private void Update()
    {
        UpdateRotation();
    }

    public void UpdateRotation()
    {
        Quaternion rot;

        bool rolled = false;

        if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
        {
            mouseX += Input.GetAxis("Mouse X") * 5;
            if (mouseX <= -180)
            {
                mouseX += 360;
            }
            else if (mouseX > 180)
            {
                mouseX -= 360;
            }
            mouseY -= Input.GetAxis("Mouse Y") * 2.4f;
            mouseY = Mathf.Clamp(mouseY, -85, 85);
        }
        else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            rolled = true;
            mouseZ += Input.GetAxis("Mouse X") * 5;
            mouseZ = Mathf.Clamp(mouseZ, -85, 85);
        }
        if (!rolled)
        {
            mouseZ = Mathf.Lerp(mouseZ, 0, Time.deltaTime / (Time.deltaTime + 0.1f));
        }
        rot = Quaternion.Euler(mouseY, mouseX, mouseZ);
        var neck = (rot * neckOffset - neckOffset.y * Vector3.up) * neckScaleModel;

        transform.localPosition = neck;
        transform.localRotation = rot;
    }
}
