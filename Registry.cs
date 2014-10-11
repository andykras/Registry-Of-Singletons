using System.Linq;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RegistryOfSingletons
{
  /// <summary>
  /// Registry of all singletons
  /// </summary>
  public static class Registry
  {
    /// <summary>
    /// The registry.
    /// </summary>
    private static readonly List<ISingleton> registry = new List<ISingleton>();

    /// <summary>
    /// For thread-safe operations
    /// </summary>
    private static readonly SmartLock smart = new SmartLock();

    /// <summary>
    /// You do not need to call it explicitly
    /// Every Singleton must call it by itself
    /// </summary>
    /// <param name="singleton">instance</param>
    public static void Register(ISingleton singleton)
    {
      if (singleton == null) return;
      smart.Enter();
      registry.Add(singleton);
      smart.Exit();
    }

    /// <summary>
    /// Get an instance or initialize it if doesn't exist
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ISingleton InstanceOf<T>() where T : class, ISingleton
    {
      RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
      smart.In();
      var instance = registry.OfType<T>().FirstOrDefault();
      smart.Out();
      if (instance == null) {
        smart.Enter();
        instance = registry.OfType<T>().FirstOrDefault();
        smart.Exit();
      }
      return instance;
    }
  }
}

