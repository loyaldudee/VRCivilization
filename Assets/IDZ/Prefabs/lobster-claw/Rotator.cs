using UnityEngine;

public class RotateOnKeyPress : MonoBehaviour
{
    public GameObject targetObject; // Drag your object here in Inspector

    private bool hasRotated = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && !hasRotated)
        {
            if (targetObject != null)
            {
                // Rotate to exactly -20 degrees on Z
                targetObject.transform.rotation = Quaternion.Euler(
                    targetObject.transform.rotation.eulerAngles.x,
                    targetObject.transform.rotation.eulerAngles.y,
                    -20f
                );

                hasRotated = true; // prevent multiple rotations
            }
            else
            {
                Debug.LogWarning("No target object assigned!");
            }
        }
    }
}
