using UnityEngine;
using System;
using System.Collections.Generic;

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
	
	// REMOVED: Dictionary and Study Gain relationship (moved to StudyProgressionHandler.cs)

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
		
		ApplyStatGain(gainRule.PrimaryStat, gainRule.PrimaryGain);
		ApplyStatGain(gainRule.SecondaryStat, gainRule.SecondaryGain);
		
		return gainRule.SecondaryStat;
	}

	// REMOVED: AddWeight method (moved to StudyProgressionHandler)
	
	private void ApplyStatGain(StatType statType, int amount)
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