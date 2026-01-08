public enum StatType
{
    spd,
	wit,
	mem,
	luk, // NOTE: Value 0, 1, 2, 3 respectively
	clr
}

[System.Serializable]
public struct StatGain
{
	public StatType primaryStat; // Chosen stat
	public StatType secondaryStat; // Collateral damage
}