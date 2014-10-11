using System;

namespace RegistryOfSingletons
{
  /// <summary>
  /// Interface of all singletons
  /// </summary>
  public interface ISingleton
  {
    /// <summary>
    /// Prints the name of
    /// </summary>
    void PrintMyName();

    /// <summary>
    /// Just do something - it doesn't matter (only for test purpose)
    /// </summary>
    /// <param name="action">Action.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    void JustDoSomething<T>(Action<T> action);
  }
}

