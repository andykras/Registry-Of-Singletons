namespace RegistryOfSingletons
{
  /// <summary>
  /// Singleton #0
  /// </summary>
  public sealed class Singleton0 : AbstractSingleton
  {
    /// <summary>
    /// explicit static ctor that initialize our singleton
    /// </summary>
    static Singleton0()
    {
      new Singleton0();
    }

    /// <summary>
    /// private ctor that register itself in Registry
    /// </summary>
    private Singleton0()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }


  /// <summary>
  /// Singleton #1
  /// </summary>
  public sealed class Singleton1 : AbstractSingleton
  {
    static Singleton1()
    {
      new Singleton1();
    }
    private Singleton1()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }


  /// <summary>
  /// Singleton #2
  /// </summary>
  public sealed class Singleton2 : AbstractSingleton
  {
    static Singleton2()
    {
      new Singleton2();
    }
    private Singleton2()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }


  /// <summary>
  /// Singleton #3
  /// </summary>
  public sealed class Singleton3 : AbstractSingleton
  {
    static Singleton3()
    {
      new Singleton3();
    }
    private Singleton3()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }


  /// <summary>
  /// Singleton #4
  /// </summary>
  public sealed class Singleton4 : AbstractSingleton
  {
    static Singleton4()
    {
      new Singleton4();
    }
    private Singleton4()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }


  /// <summary>
  /// Singleton #5
  /// </summary>
  public sealed class Singleton5 : AbstractSingleton
  {
    static Singleton5()
    {
      new Singleton5();
    }
    private Singleton5()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }


  /// <summary>
  /// Singleton #6
  /// </summary>
  public sealed class Singleton6 : AbstractSingleton
  {
    static Singleton6()
    {
      new Singleton6();
    }
    private Singleton6()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }


  /// <summary>
  /// Singleton #7
  /// </summary>
  public sealed class Singleton7 : AbstractSingleton
  {
    static Singleton7()
    {
      new Singleton7();
    }
    private Singleton7()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }


  /// <summary>
  /// Singleton #8
  /// </summary>
  public sealed class Singleton8 : AbstractSingleton
  {
    static Singleton8()
    {
      new Singleton8();
    }
    private Singleton8()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }


  /// <summary>
  /// Singleton #9
  /// </summary>
  public sealed class Singleton9 : AbstractSingleton
  {
    static Singleton9()
    {
      new Singleton9();
    }
    private Singleton9()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }


  /// <summary>
  /// Singleton #10
  /// </summary>
  public sealed class Singleton10 : AbstractSingleton
  {
    static Singleton10()
    {
      new Singleton10();
    }
    private Singleton10()
    {
      Registry.Register(this);
      PrintMyName();
    }
  }
}

