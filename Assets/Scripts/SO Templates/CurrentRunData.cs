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
    
    // Run once on a new run
    public void InitializeRun()
    {
        Speed = 70;
        Wit = 70;
        Memory = 70;
        Luck = 50;
        Clarity = 100;
        Mood = 4;
        
        doneREvent = false;
        
        // Reset progress
        CurrentTurn = 1;
        
        InitializeFlags();
    }
	
	// TODO: Transfer the Stat Storage from StatsManager.cs to CurrentRunData.cs
	// Map the enum and the int storage
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