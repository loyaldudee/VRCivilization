using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToggleRotationOnGrab : MonoBehaviour
{
    [Header("Rotation Settings")]
    public GameObject targetObject;                    // Rotates on grab
    public float angle = 20f;
    public float rotationSpeed = 5f;
    public InputActionProperty grabAction;

    [Header("Claw Extension Settings")]
    public GameObject clawExtensionObject;             // This object scales in Y
    public float clawExtendScaleY = 1.5f;              // How tall it stretches
    public float scaleSpeed = 0.01f;

    [Header("Object Names to Hide at Start (Partial Match Allowed)")]
    public string[] objectNamesToHide;

    private Quaternion initialRotation;
    private Quaternion targetRotation;
    private bool isRotated = false;
    private bool isTransitioning = false;

    private Vector3 initialClawScale;

    void Awake()
    {
        if (targetObject != null)
            initialRotation = targetObject.transform.localRotation;

        if (clawExtensionObject != null)
            initialClawScale = clawExtensionObject.transform.localScale;
    }

    IEnumerator Start()
    {
        yield return null;

        foreach (string name in objectNamesToHide)
        {
            GameObject obj = FindObjectByName(name);
            if (obj != null)
            {
                DisableAllMeshes(obj);
                Debug.Log($"[MeshHider] Meshes disabled for '{obj.name}'.");
            }
            else
            {
                Debug.LogWarning($"[MeshHider] GameObject '{name}' not found.");
            }
        }
    }

    void Update()
    {
        if (grabAction.action.WasPressedThisFrame() && !isTransitioning)
        {
            isRotated = !isRotated;
            targetRotation = isRotated
                ? initialRotation * Quaternion.Euler(0, 0, -angle)
                : initialRotation;

            StartCoroutine(RotateToTarget());
            StartCoroutine(ScaleClawTemporarily());
        }
    }

    IEnumerator RotateToTarget()
    {
        isTransitioning = true;

        while (Quaternion.Angle(targetObject.transform.localRotation, targetRotation) > 0.1f)
        {
            targetObject.transform.localRotation = Quaternion.Slerp(
                targetObject.transform.localRotation,
                targetRotation,
                Time.deltaTime * rotationSpeed
            );
            yield return null;
        }

        targetObject.transform.localRotation = targetRotation;
        isTransitioning = false;
    }

    IEnumerator ScaleClawTemporarily()
    {
        if (clawExtensionObject == null)
            yield break;

        Vector3 targetScale = new Vector3(
            initialClawScale.x,
            initialClawScale.y * clawExtendScaleY,
            initialClawScale.z
        );

        // Scale up
        while (Vector3.Distance(clawExtensionObject.transform.localScale, targetScale) > 0.01f)
        {
            clawExtensionObject.transform.localScale = Vector3.Lerp(
                clawExtensionObject.transform.localScale,
                targetScale,
                Time.deltaTime * scaleSpeed
            );
            yield return null;
        }

        clawExtensionObject.transform.localScale = targetScale;

        // Optional pause at full stretch
        yield return new WaitForSeconds(0.2f);

        // Scale back
        while (Vector3.Distance(clawExtensionObject.transform.localScale, initialClawScale) > 0.01f)
        {
            clawExtensionObject.transform.localScale = Vector3.Lerp(
                clawExtensionObject.transform.localScale,
                initialClawScale,
                Time.deltaTime * scaleSpeed
            );
            yield return null;
        }

        clawExtensionObject.transform.localScale = initialClawScale;
    }

    GameObject FindObjectByName(string partialName)
    {
        Transform[] allTransforms = GameObject.FindObjectsOfType<Transform>(true);
        foreach (Transform t in allTransforms)
        {
            if (t.name.Contains(partialName))
                return t.gameObject;
        }
        return null;
    }

    void DisableAllMeshes(GameObject obj)
    {
        foreach (var mesh in obj.GetComponentsInChildren<MeshRenderer>(true))
            mesh.enabled = false;

        foreach (var skinned in obj.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            skinned.enabled = false;
    }
}
