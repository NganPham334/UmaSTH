using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CurrentRunData", menuName = "Current Run Data", order = 0)]
public class CurrentRunData : ScriptableObject
{
    [HideInInspector] 
    public int Speed, Wit, Memory, Luck, Clarity, CurrentTurn, TotalTurns;
    [HideInInspector]
    public string CurrentDate;
    
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
    private Dictionary<string, bool> _runtimeFlags = new Dictionary<string, bool>();
    
    // Run once on a new run
    public void InitializeRun()
    {
        Speed = 200;
        Wit = 100;
        Memory = 10;
        Luck = 10;
        Clarity = 100;

        // Reset progress
        CurrentTurn = 1;
        CurrentDate = "Junior Year, Week 1";
        
        InitializeFlags();
    }
	
	// TODO: Transfer the Stat Storage from StatsManager.cs to CurrentRunData.cs
	// Map the enum and the int storage
	public int GetStatValue(StatType type)
	{
		switch (type)
		{
			case StatType.SPD: return Speed;
			case StatType.WIT: return Wit;
			case StatType.MEM: return Memory;
			case StatType.LUK: return Luck;
			default: return 0;
			// NOTE: In the finished prototype, default case won't be needed
		}
	}
	
	public void SetStatValue(StatType type, int value)
	{
		switch (type)
		{
			case StatType.SPD: 
				Speed = value;
				break;
			case StatType.WIT:
				Wit = value;
				break;
			case StatType.MEM:
				Memory = value;
				break;
			case StatType.LUK:
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
}