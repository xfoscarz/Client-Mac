using System;

public class BindableEventArgs<T> : EventArgs
{
    public T Value;

    public T OldValue;

    public BindableEventArgs(T value, T oldValue)
    {
        Value = value;
        OldValue = oldValue;
    }
}
