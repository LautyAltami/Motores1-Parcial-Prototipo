using UnityEngine;

public class DoorScript : MonoBehaviour, IInteractable
{
    [SerializeField] Transform pivot;   
    [SerializeField] float openAngle = 90f;
    [SerializeField] float openVelocity = 2f;

    bool isOpen;

    Quaternion closedRotation;
    Quaternion targetRotation;
   
    void Start()
    {
        closedRotation = pivot.rotation;
        targetRotation = closedRotation;
    }

    void Update()
    {
        pivot.rotation = Quaternion.Lerp(pivot.rotation,targetRotation,Time.deltaTime * openVelocity);
    }

    public void Interact()
    {
        isOpen = !isOpen;

        float direction = isOpen ? 1f : 0f;

        targetRotation = isOpen ? Quaternion.Euler(0, openAngle, 0) * closedRotation : closedRotation;
    }
}
