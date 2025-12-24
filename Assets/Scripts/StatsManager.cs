using UnityEngine;
using System;
using System.Collections.Generic;

public enum StatType
{
    spd,
	wit,
	mem,
	luk // NOTE: Value 0, 1, 2, 3 respectively
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
	
	[Header("Current Run Data")]
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
		
	// Dictionary for Study Gain based on Study Level
	private Dictionary<int, (int primary, int secondary)> levelRewards = new Dictionary<int, (int, int)>
	{
		{ 1, (10, 2) },
		{ 2, (12, 3) },
		{ 3, (18, 7) },
		{ 4, (24, 11) },
		{ 5, (30, 15) }
	};
		
	// Primary stat and Secondary stat relationship
	[Header("Study Gains Relationship")]
	public List<StatGain> StudyGains = new List<StatGain>()
	{
		new StatGain {PrimaryStat = StatType.spd, SecondaryStat = StatType.wit, PrimaryGain = PrimaryGainAmount, SecondaryGain = SecondaryGainAmount},
		new StatGain {PrimaryStat = StatType.mem, SecondaryStat = StatType.spd, PrimaryGain = PrimaryGainAmount, SecondaryGain = SecondaryGainAmount},
		new StatGain {PrimaryStat = StatType.wit, SecondaryStat = StatType.luk, PrimaryGain = PrimaryGainAmount, SecondaryGain = SecondaryGainAmount},
		new StatGain {PrimaryStat = StatType.luk, SecondaryStat = StatType.mem, PrimaryGain = PrimaryGainAmount, SecondaryGain = SecondaryGainAmount}
	};
	
	// REMOVED: Start() and Dictionary initialization
	
	// CHANGE: Increase Primary Stat and return Secondary Stat that was also changed
	public StatType IncrementStat(StatType primaryStatType)
	{
		if (runData == null) return primaryStatType;

		// When study, only Primary Stat gain Study Weight
		AddWeight(primaryStatType);

		// Primary and Secondary Stat Gain calculation based on Study Level
		int currentLevel = runData.GetLevel(primaryStatType);
    	var gains = levelRewards[currentLevel];

		// Find the relationship/rule
		StatGain gainRule = StudyGains.Find(g => g.PrimaryStat == primaryStatType);
		
		ApplyGain(gainRule.PrimaryStat, gainRule.PrimaryGain);
		ApplyGain(gainRule.SecondaryStat, gainRule.SecondaryGain);
		
		return gainRule.SecondaryStat;
	}

	private void AddWeight(StatType type)
	{
		// If Study Level is maxed (5), no Weight will be added to that 
		if (runData.GetLevel(type) >= 5) return;
		// TODO: Check to see if maxed Stat consume Upgrade Point
		// if yes, then the logic needs to be changed

		if (type == StatType.spd) runData.spdWeight++;
		if (type == StatType.wit) runData.witWeight++;
		if (type == StatType.mem) runData.memWeight++;
		if (type == StatType.luk) runData.lukWeight++;
	}
	
	private void ApplyGain(StatType statType, int amount)
	{
		int currentValue = runData.GetStatValue(statType);
		const int MaxValue = 1000;
		
		currentValue += amount;
		currentValue = Mathf.Min(currentValue, MaxValue);
		
		runData.SetStatValue(statType, currentValue);
	}
	
	// Getter function for StudyScreenController
	public int GetStatValue(StatType type)
	{
		if (runData == null) return 0;
		return runData.GetStatValue(type); // Move reading value to CurrentRunData object
	}
	
}