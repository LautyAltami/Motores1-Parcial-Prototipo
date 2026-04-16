using UnityEngine;

public class MedicineScipt : MonoBehaviour, IInteractable
{
    [SerializeField] float SanityGainPoints;
    [SerializeField] GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Interact()
    {
        if(Player.TryGetComponent<Player_script>(out var playerScript))
        {
            playerScript.SanityGain(SanityGainPoints);
        }
        else if(Player.TryGetComponent<PlayerControllerScript>(out var playerControllerScript))
        {
            playerControllerScript.SanityGain(SanityGainPoints);
        }

        Destroy(gameObject);
    }

}
