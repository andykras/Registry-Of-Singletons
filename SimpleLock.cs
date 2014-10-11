using System;
using System.Threading;

namespace RegistryOfSingletons
{
  /// <summary>
  /// Simple lock similar to Jeffrey Rithter's one from his book named CLR via C#
  /// </summary>
  public class SimpleLock
  {
    private Int32 waiters = 0;
    private readonly AutoResetEvent waiterLock = new AutoResetEvent(false);

    public void Enter()
    {
      if (Interlocked.Increment(ref waiters) == 1) return;
      waiterLock.WaitOne();
    }

    public void Exit()
    {
      if (Interlocked.Decrement(ref waiters) == 0) return;
      waiterLock.Set();
    }
  }
}

