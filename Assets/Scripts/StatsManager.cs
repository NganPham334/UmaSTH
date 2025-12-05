using UnityEngine;
using System;
using System.Collections.Generic;

public enum StatType
{
    SPD,
	WIT,
	MEM,
	LUK // NOTE: Value 0, 1, 2, 3 respectively
}

[Serializable]
public class CharacterStat
{
	public StatType Type;
	public int CurrentValue = 0;
	public const int MaxValue = 1000;
}

[Serializable]
public struct StatGain
{
	public StatType PrimaryStat; // Chosen stat
	public StatType SecondaryStat; // Collateral damage
	public int PrimaryGain;
	public int SecondaryGain;
}

public class StatsManager : MonoBehaviour
{
	public static StatsManager Instance { get; private set; }
	public CurrentRunData runData; // Reference to data storage ScriptableObject
	public void Awake()
	{
		// Singleton Pattern
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
	}
		
	// Define gain amount
	private const int PrimaryGainAmount = 10;
	private const int SecondaryGainAmount = 2; // const value for now
	// TODO: GainAmount change base on Study Level
		
	// Primary stat and Secondary stat relationship
	public List<StatGain> StudyGains = new List<StatGain>()
	{
		new StatGain {PrimaryStat = StatType.SPD, SecondaryStat = StatType.WIT, PrimaryGain = PrimaryGainAmount, SecondaryGain = SecondaryGainAmount},
		new StatGain {PrimaryStat = StatType.MEM, SecondaryStat = StatType.SPD, PrimaryGain = PrimaryGainAmount, SecondaryGain = SecondaryGainAmount},
		new StatGain {PrimaryStat = StatType.WIT, SecondaryStat = StatType.LUK, PrimaryGain = PrimaryGainAmount, SecondaryGain = SecondaryGainAmount},
		new StatGain {PrimaryStat = StatType.LUK, SecondaryStat = StatType.MEM, PrimaryGain = PrimaryGainAmount, SecondaryGain = SecondaryGainAmount}
	};
	
	// REMOVED: Start() and Dictionary initialization
	
	// CHANGE: Increase Primary Stat and return Secondary Stat that was also changed
	public StatType IncrementStat(StatType primaryStatType)
	{
		if (runData == null) return primaryStatType;
		// Find the relationship/rule
		StatGain gainRule = StudyGains.Find(g => g.PrimaryStat == primaryStatType);
		
		ApplyGain(gainRule.PrimaryStat, gainRule.PrimaryGain);
		ApplyGain(gainRule.SecondaryStat, gainRule.SecondaryGain);
		
		return gainRule.SecondaryStat;
	}
	
	private void ApplyGain(StatType statType, int amount)
	{
		int currentValue = runData.GetStatValue(statType);
		const int MaxValue = 1000;
		
		currentValue += amount;
		currentValue = Mathf.Min(currentValue, MaxValue);
		
		runData.SetStatValue(statType, currentValue);
		
		Debug.Log($"{statType} = {currentValue} now");
	}
	
	// Getter function for StudyScreenController
	public int GetStatValue(StatType type)
	{
		if (runData == null) return 0;
		return runData.GetStatValue(type); // Move reading value to CurrentRunData object
	}
	
}