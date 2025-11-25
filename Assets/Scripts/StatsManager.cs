using UnityEngine;
using System;
using System.Collections.Generic;

public enum StatType
{
    SPD,
	WIT,
	MEM,
	LUK // Value 0, 1, 2, 3 respectively
}

[Serializable]
public class CharacterStat
{
	public StatType Type;
	public int CurrentValue = 0;
	public const int MaxValue = 1000;
}

[Serializable]
public struct StatGain // Define a structure for PrimaryStat + SecondaryStat (string), and PrimaryGain + SecondaryGain (int)
{
	public StatType PrimaryStat; // Chosen stat
	public StatType SecondaryStat; // Collateral damage
	public int PrimaryGain;
	public int SecondaryGain;
}

public class StatsManager : MonoBehaviour
{
	public static StatsManager Instance { get; private set; }
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
	
	// Storing character stats in Dictionary
	private Dictionary<StatType, CharacterStat> _stats = new Dictionary<StatType, CharacterStat>();
	
	private void Start()
	{
		// Initialize all stats to (0, 1000)
		foreach (StatType type in System.Enum.GetValues(typeof(StatType)))
		{
			_stats.Add(type, new CharacterStat {Type = type, CurrentValue = 0});
		}
	}
	
	public void IncrementStat(StatType primaryStatType)
	{
		// Find the relationship/rule
		StatGain gainRule = StudyGains.Find(g => g.PrimaryStat == primaryStatType);
		
		if (_stats.TryGetValue(gainRule.PrimaryStat, out CharacterStat primaryStat))
		{
			ApplyGain(primaryStat, gainRule.PrimaryGain);
			
			Debug.Log($"Study {gainRule.PrimaryStat}, new value: {primaryStat.CurrentValue}");
		}
		
		if (_stats.TryGetValue(gainRule.SecondaryStat, out CharacterStat secondaryStat))
		{
			ApplyGain(secondaryStat, gainRule.SecondaryGain);
			
			Debug.Log($"Collateral damage to {gainRule.SecondaryStat}, new value: {secondaryStat.CurrentValue}");
		}
	}
	
	// ApplyGain function
	private void ApplyGain(CharacterStat stat, int amount)
	{
		stat.CurrentValue += amount;
		stat.CurrentValue = Mathf.Min(stat.CurrentValue, CharacterStat.MaxValue); // Enforce MaxValue
	}
}