using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;

namespace RegistryOfSingletons
{
  /// <summary>
  /// Test program.
  /// </summary>
  class TestProgram
  {
    private static readonly ManualResetEvent WaitHereForOthersBeforeStart = new ManualResetEvent(false);
    private const int ThreadsCount = 100;
    private const int LoopCountForEachThread = 10000;
    private const bool ShowOutput = false;
    private const bool InitializationIfSingleThread = false;

    /// <summary>
    /// Just print thread's name with thousand's prefix
    /// </summary>
    /// <param name="i">The index.</param>
    static void PrintMeWorking(int i)
    {
      if (i % 1000 == 0) Console.Write((char) ((int) '!' + i / 1000) + Thread.CurrentThread.Name);
    }

    /// <summary>
    /// call for singletons 
    /// JustDoSomething - all that is capable of imagination
    /// </summary>
    static void CallToSingletons()
    {
      Registry.InstanceOf<Singleton0>().JustDoSomething<double>(x => Math.Exp(x));
      Registry.InstanceOf<Singleton1>().JustDoSomething<double>(x => Math.Sqrt(Math.Exp(x)));
      Registry.InstanceOf<Singleton2>().JustDoSomething<double>(x => x = Math.Sin(x) * Math.Sin(x) + Math.Cos(x) * Math.Cos(x));
      Registry.InstanceOf<Singleton3>().JustDoSomething<double>(x => x = Math.Sin(x * x) + Math.Cos(x * x));
      Registry.InstanceOf<Singleton4>().JustDoSomething<double>(x => x = Math.Sign(x) * Math.Cos(x) + Math.Abs(Math.Sin(x)));
      Registry.InstanceOf<Singleton8>().JustDoSomething<double>(x => Math.Log(x, Math.E));
      Registry.InstanceOf<Singleton6>().JustDoSomething<int>(x => Math.Abs(x));
      Registry.InstanceOf<Singleton7>().JustDoSomething<int>(x => Math.Pow(x, x));
      Registry.InstanceOf<Singleton9>().JustDoSomething<int>(x => x = x.ToString().Length);
      Registry.InstanceOf<Singleton10>().JustDoSomething<bool>(x => x = !x);
    }

    /// <summary>
    /// The entry point of the program, where the program control starts and ends.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    static void Main(string[] args)
    {
      var watch = new Stopwatch();
      watch.Start();

      if (InitializationIfSingleThread) CallToSingletons(); // initialization in single thread

      var threads = new List<Thread>();
      for (var i = 0; i < ThreadsCount; i++) threads.Add(new Thread(Test) { IsBackground = true, Name = i.ToString() });
      foreach (var thread in threads) thread.Start();

      WaitHereForOthersBeforeStart.Set();
      foreach (var thread in threads) thread.Join();

      watch.Stop();
      Console.WriteLine("Done.");
      Console.WriteLine("time: {0}", watch.ElapsedMilliseconds);
    }

    /// <summary>
    /// Testing of Singletons
    /// </summary>
    public static void Test()
    {
      WaitHereForOthersBeforeStart.WaitOne();

      CallToSingletons();
      Console.Write("A"); // initialization complete

      for (var i = 0; i < LoopCountForEachThread; i++) {
        if (ShowOutput) PrintMeWorking(i);
        CallToSingletons();
      }
      Console.Write(".");
    }
  }
}
