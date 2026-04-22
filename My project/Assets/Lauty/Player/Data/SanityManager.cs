using UnityEngine;

public class SanityManager : MonoBehaviour
{
    [Header("Estado de Sanidad")]
    public float currentSanity = 100f;
    public bool canDieFromSanity = true;

    [Header("Variables del Entorno")]
    public bool isHidden = false;
    public bool hasLight = false;

    [Header("Tasas de Cambio")]
    public float darknessDrainRate = 3f;
    public float lockerRecoveryRate = 15f;
    public float monsterMultiplier = 5f;

    private Transform monster;

    void Start()
    {
        FindMonster();
    }

    void Update()
    {
        if (monster == null) FindMonster();

        // 1. Lógica si estás escondido
        if (isHidden)
        {
            currentSanity += lockerRecoveryRate * Time.deltaTime;
        }
        // 2. Lógica si estás expuesto
        else
        {
            float currentDrain = 0f;

            if (!hasLight) currentDrain += darknessDrainRate;

            if (monster != null)
            {
                float distance = Vector3.Distance(transform.position, monster.position);
                if (distance < 10f)
                {
                    currentDrain += monsterMultiplier * (10f / Mathf.Max(distance, 1f));
                }
            }

            currentSanity -= currentDrain * Time.deltaTime;
        }

        currentSanity = Mathf.Clamp(currentSanity, 0f, 100f);

        // 3. Condición de Muerte
        if (currentSanity <= 0 && canDieFromSanity)
        {
            DieFromInsanity();
        }
    }

    void FindMonster()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("Enemy");
        if (obj != null) monster = obj.transform;
    }

    void DieFromInsanity()
    {
        Debug.Log("¡GAME OVER! La locura te consumió antes de llegar al locker.");
    }
}