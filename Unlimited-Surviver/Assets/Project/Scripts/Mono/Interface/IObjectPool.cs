using UnityEngine;

public interface IObjectPool<T>
{
    public T Get();
    public void Return(T poolObject);
}