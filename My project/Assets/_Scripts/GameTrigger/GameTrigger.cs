using UnityEngine;

public class GameTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        Victory,
        Defeat
    }

    [SerializeField] private TriggerType type;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        switch (type)
        {
            case TriggerType.Victory:
                GameManager.DispararVictoria();
                break;

            case TriggerType.Defeat:
                GameManager.DispararDerrota();
                break;
        }

        Destroy(gameObject);
    }
}