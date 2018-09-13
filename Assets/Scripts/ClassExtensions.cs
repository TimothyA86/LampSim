using UnityEngine;
using UnityEngine.UI;

public static class ClassExtensions
{
	public static Vector3Int AsVector3Int(this Vector3 vector)
	{
		return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
	}

	public static string AsString<T>(this T[] array)
	{
		string s = "[ ";
		int count = array.Length;

		for (int i = 0; i < count - 1; ++i)
		{
			s += array[i] + ", ";
		}

		s += array[count - 1] + " ]";
		return s;
	}
}
