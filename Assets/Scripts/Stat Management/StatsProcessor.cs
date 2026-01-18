using UnityEngine;

public class StatsProcessor : MonoBehaviour
{
    public float failChance;
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
    
    // Chance of Failure based on Clarity
    // Formula: 2% * (50 - currentClarity) if clarity < 50
    public bool RollForSuccess(int clarityValue)
    {
        if (clarityValue >= 50) return true; // Always success Study

        int displacement = 50 - clarityValue;
        failChance = displacement * 2f;
        
        float roll = Random.Range(0f, 100f);
        
        // E.g. If we roll a 10 and failChance is 30, it's a fail
        return roll > failChance; 
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