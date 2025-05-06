using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapToCorrectHand : MonoBehaviour
{
    [Header("Snap Points")]
    public Transform leftHandSnapPoint;
    public Transform rightHandSnapPoint;

    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogError("[SnapToCorrectHand] No XRGrabInteractable found on this object.");
            return;
        }

        if (leftHandSnapPoint == null || rightHandSnapPoint == null)
        {
            Debug.LogError("[SnapToCorrectHand] Assign both left and right snap points!");
            return;
        }

        grabInteractable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject;

        if (interactor is XRDirectInteractor directInteractor)
        {
            if (directInteractor.name.ToLower().Contains("left"))
            {
                grabInteractable.attachTransform = leftHandSnapPoint;
                Debug.Log("[SnapToCorrectHand] Snapped to LEFT hand.");
            }
            else if (directInteractor.name.ToLower().Contains("right"))
            {
                grabInteractable.attachTransform = rightHandSnapPoint;
                Debug.Log("[SnapToCorrectHand] Snapped to RIGHT hand.");
            }
            else
            {
                Debug.LogWarning("[SnapToCorrectHand] Couldn't determine hand. Defaulting to LEFT.");
                grabInteractable.attachTransform = leftHandSnapPoint;
            }
        }
        else
        {
            Debug.LogWarning("[SnapToCorrectHand] Interactor isn't a direct interactor.");
        }
    }
}
