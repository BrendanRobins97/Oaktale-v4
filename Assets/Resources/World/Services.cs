/// 
/// Simple service manager. Allows global access to a single instance of any class.
/// Copyright (c) 2014-2015 Eliot Lash
/// 
using System;
using System.Collections.Generic;

public static class GameManager
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();

    /// Set the specified service instance. Usually called like Set(this).
    /// 
    /// Service instance object.
    /// Type of the instance object.
    public static void Set<T>(T service) where T : class
    {
        services.Add(typeof(T), service);
    }

    /// Gets the specified service instance. Called like Get().
    /// 
    /// Type of the service.
    /// Service instance, or null if not initialized
    public static T Get<T>() where T : class
    {
        T ret = null;
        try
        {
            ret = services[typeof(T)] as T;
        }
        catch (KeyNotFoundException)
        {
        }
        return ret;
    }

    /// Clears internal dictionary of service instances.
    /// This will not clear out any global state that they contain,
    /// unless there are no other references to the object.
    /// 
    public static void Clear()
    {
        services.Clear();
    }
}