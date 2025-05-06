using UnityEngine;
using UnityEngine.InputSystem;

public class ManualGrabberXR : MonoBehaviour
{
    public InputActionProperty grabAction; // Reference to the XR grab input
    public string grabbableTag = "Grabbable";
    public Transform handSnapPoint;
    public float grabDistance = 0.3f;

    private GameObject currentlyHeld;
    private bool previousGrabState = false;

    void Update()
    {
        bool grabPressed = grabAction.action.ReadValue<float>() > 0.5f;

        // Detect *press down* of the grab
        if (grabPressed && !previousGrabState)
        {
            if (currentlyHeld == null)
            {
                TryGrab();
            }
            else
            {
                Release();
            }
        }

        previousGrabState = grabPressed;
    }

    void TryGrab()
    {
        Collider[] nearby = Physics.OverlapSphere(transform.position, grabDistance);

        foreach (var col in nearby)
        {
            if (col.CompareTag(grabbableTag))
            {
                currentlyHeld = col.gameObject;
                Rigidbody rb = currentlyHeld.GetComponent<Rigidbody>();
                if (rb != null) rb.isKinematic = true;

                currentlyHeld.transform.SetParent(handSnapPoint);
                currentlyHeld.transform.localPosition = Vector3.zero;
                currentlyHeld.transform.localRotation = Quaternion.identity;

                break;
            }
        }
    }

    void Release()
    {
        if (currentlyHeld != null)
        {
            Rigidbody rb = currentlyHeld.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = false;

            currentlyHeld.transform.SetParent(null);
            currentlyHeld = null;
        }
    }
}
