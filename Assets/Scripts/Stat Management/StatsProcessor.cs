using UnityEngine;

public class StatsProcessor : MonoBehaviour
{
    // I see that Blay made this declaration:
    // public float failChance;
    // Which I suppose is to be able to see how the failChance is calculated real-time
    // For your sanity after my new implementation, please have this instead
    [SerializeField, Range(0,1)] private float failChance;
    public int GetClarityCost(StatType type)
    {
        return type switch
        {
            StatType.spd => 16,
            StatType.mem => 13,
            StatType.wit => 19,
            StatType.luk => 15,
            _ => 0
        };
    }

    // Standalone Failure calculation (return 0.0 to 1.0)
    public float GetFailureChance(int clarityValue)
    {
        if (clarityValue >= 50) return 0f;
        
        int displacement = 50 - clarityValue;
        float chance = (displacement * 2f) / 100f; // Normalize [0,1] range
        failChance = chance; // Visual debugging
        return chance;
    }
    
    // Updated Roll method
    public bool RollForSuccess(int clarityValue)
    {
        float chance = GetFailureChance(clarityValue);
        float roll = Random.value;
        
        // roll 0.1, chance 0.3 => fail
        return roll > chance; 
    }

    // Mood System
    public (int pGain, int sGain) CalculateFinalGain(int baseP, int baseS, int moodLevel)
    {
        float pBonus = 0f;
        float sBonus = 0f;

        switch (moodLevel)
        {
            case 0: // Depressed
                pBonus = -0.20f; sBonus = -0.10f; break;
            case 1: // Bad
                pBonus = -0.10f; sBonus = -0.05f; break;
            case 2: // Normal
                pBonus = 0f; sBonus = 0f; break;
            case 3: // Good
                pBonus = 0.10f; sBonus = 0.05f; break;
            case 4: // Umazing
                pBonus = 0.20f; sBonus = 0.10f; break;
        }

        int finalP = Mathf.RoundToInt(baseP * (1f + pBonus));
        int finalS = Mathf.RoundToInt(baseS * (1f + sBonus));

        return (finalP, finalS);
    }
}