using System;
using System.Threading;

namespace RegistryOfSingletons
{
  /// <summary>
  /// Abstract singleton
  /// One could simply direct inherits from Singleton, but in terms of your exercise we have only two invariant functions
  /// </summary>
  public abstract class  AbstractSingleton : ISingleton
  {
    /// <summary>
    /// Prints the name of
    /// </summary>
    public virtual void PrintMyName()
    {
      Console.WriteLine("from thread {0,3:X3} create {1}", Thread.CurrentThread.Name, GetType().Name);
    }

    /// <summary>
    /// Just do something - it doesn't matter (only for test purpose)
    /// </summary>
    /// <param name="action">Action.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public virtual void JustDoSomething<T>(Action<T> action)
    {
      action(default(T));
    }
  }
}

