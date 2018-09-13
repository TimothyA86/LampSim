using UnityEngine;

public class RangeDropDownAttribute : PropertyAttribute
{
	public int min;
	public int max;
	public string prefix;

	public RangeDropDownAttribute(int min, int max, string prefix = "")
	{
		this.min = min;
		this.max = max;
		this.prefix = prefix;
	}
}
