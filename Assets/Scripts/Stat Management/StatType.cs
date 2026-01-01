public enum StatType
{
    spd,
	wit,
	mem,
	luk // NOTE: Value 0, 1, 2, 3 respectively
}

[System.Serializable]
public struct StatGain
{
	public StatType PrimaryStat; // Chosen stat
	public StatType SecondaryStat; // Collateral damage
}