using System;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CurrentRunData", menuName = "Current Run Data", order = 0)]
public class CurrentRunData : ScriptableObject
{
    //[HideInInspector] 
    public int Speed, Wit, Memory, Luck, Clarity, CurrentTurn, TotalTurns, Mood;
    [HideInInspector]
    public bool doneREvent = false; 
    
    [System.Serializable]
    public class Flag
    {
        public string Name;
        public bool IsSet;
    }

    // Cnay de add flag tu editor
    // Centralized place for flags
    [Header("Story Flags")]
    [Tooltip("Set the default state of all flags")]
    public List<Flag> DefaultFlags;

    // The actual runtime data
    private Dictionary<string, bool> _runtimeFlags = new();

    // Study Weight and Level
    [Header("Study Levels & Weights")]
    public int spdLevel = 1, witLevel = 1, memLevel = 1, lukLevel = 1;
    public int spdWeight = 1, witWeight = 1, memWeight = 1, lukWeight = 1;
    
    // Run once on a new run
    public void InitializeRun()
    {
        Speed = 70;
        Wit = 70;
        Memory = 70;
        Luck = 50;
        Clarity = 50;
        Mood = 4;
        
        doneREvent = false;

        // Reset Levels and Weights
        spdLevel = witLevel = memLevel = lukLevel = 1;
        spdWeight = witWeight = memWeight = lukWeight = 1;
        
        // Reset progress
        CurrentTurn = 1;
        
        InitializeFlags();
    }
	
	public int GetStatValue(StatType type)
	{
		switch (type)
		{
			case StatType.spd: return Speed;
			case StatType.wit: return Wit;
			case StatType.mem: return Memory;
			case StatType.luk: return Luck;
			default: return 0;
			// NOTE: In the finished prototype, default case won't be needed
		}
	}
	
	public void SetStatValue(StatType type, int value)
	{
		switch (type)
		{
			case StatType.spd: 
				Speed = value;
				break;
			case StatType.wit:
				Wit = value;
				break;
			case StatType.mem:
				Memory = value;
				break;
			case StatType.luk:
				Luck = value;
				break;
		}
	}

    // Helper method for the Lottery logic in StatsManager
    public int GetStatWeight(StatType type) => type switch {
        StatType.spd => spdWeight,
        StatType.wit => witWeight,
        StatType.mem => memWeight,
        StatType.luk => lukWeight,
        _ => 0 // If [type] if not recognized, return Weight = 0
    };

    public int GetStatLevel(StatType type) => type switch {
        StatType.spd => spdLevel,
        StatType.wit => witLevel,
        StatType.mem => memLevel,
        StatType.luk => lukLevel,
        _ => 1
    };

    public void AdvanceTurn()
    {
        CurrentTurn++;
    }
    
    public void InitializeFlags()
    {
        _runtimeFlags.Clear();
        foreach (var flag in DefaultFlags)
        {
            if (!_runtimeFlags.ContainsKey(flag.Name))
            {
                _runtimeFlags.Add(flag.Name, flag.IsSet);
            }
        }
    }
    
    public bool CheckFlag(string flagName)
    {
        if (_runtimeFlags.TryGetValue(flagName, out bool value))
        {
            return value;
        }
        Debug.LogWarning($"StoryFlag '{flagName}' does not exist.");
        return false;
    }
    
    public void SetFlag(string flagName, bool value)
    {
        if (_runtimeFlags.ContainsKey(flagName))
        {
            _runtimeFlags[flagName] = value;
        }
        else
        {
            Debug.LogWarning($"StoryFlag '{flagName}' does not exist. Cannot set.");
        }
    }

    public string ChangeMood(int change)
    {
	    Mood = Math.Min(4, Math.Max(0, Mood + change));
        return GetMood();
    }

    public string GetMood()
    {
	    return new string[] { "Depressed", "Bad", "Normal", "Good", "Umazing" }[Mood];
    }
}