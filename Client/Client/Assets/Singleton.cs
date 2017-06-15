using System;
using System.Reflection;

public abstract class Singleton<T> where T : class
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if(_instance == null)
                {
                    Type t = typeof(T);

                    ConstructorInfo[] ctors = t.GetConstructors();
                    if(ctors.Length > 0)
                    {
                        throw new InvalidOperationException(String.Format("{0} has at least one accesible ctor making it impossible to enforce singleton behaviour", t.Name));
                    }

                    _instance = Activator.CreateInstance(t, true) as T;
                }
                return _instance;
            }
        }
    }
}