using UnityEngine;

public class InitializedArray<T>
{
    public readonly int Length;
    public readonly T[] Array;
    private int _iterator;

    public InitializedArray(int length)
    {
        Length = length;
        Array = new T[length];
        _iterator = 0;
    }

    public void Init(T value)
    {
        Array[_iterator] = value;
        _iterator++;
    }
}