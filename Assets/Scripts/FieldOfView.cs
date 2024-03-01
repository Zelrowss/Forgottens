using System;

internal class FieldOfView
{
    internal object transform;
    internal float radius;

    public static explicit operator FieldOfView(UnityEngine.Object v)
    {
        throw new NotImplementedException();
    }
}