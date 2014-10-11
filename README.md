
#Registry of Singletons 

This is a very simple example of registry for singletons. I was inspired the idea when I was reading GoF Design Patterns. There was an example to demonstrate how to create the same in C++ language.

And I started to search implementation in C#, but with no luck - I found a couple of stuff that is not satisfied. I also found Multiton pattern, but it is actually a kind of singleton with single instance per key - it has multiple instances of itself. I think that registry of singletons is a more general and flexible approach.

So I challenged to myself.


##The Idea
Singleton classes can register themselves by name in a well-known registry. The registry maps between string names and singletons. When clients needs a singleton, it asking registry for the singleton by name. The registry looks up the corresponding singleton and returns it. It was an original idea of GoF. 

We can improve this approach by using template functions. Singletons would be registered their instances just in a registry list and client could look up instance of singleton by its type:
```
Registry.GetInstanceOf<MySingleton>();
```
##Implementation
All it requires is a common interface for all Singleton classes
that includes operations for the registry:

```
  /// <summary>
  /// Registry of all singletons
  /// </summary>
  public static class Registry
  {
    /// <summary>
    /// You do not need to call it explicitly
    /// Every Singleton must call it by itself
    /// </summary>
    /// <param name="singleton">instance</param>
    public static void Register(ISingleton singleton);

    /// <summary>
    /// Get an instance or initialize it if doesn't exist
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ISingleton GetInstanceOf<T>();


    /// <summary>
    /// The registry.
    /// </summary>
    private static readonly List<ISingleton> registry;
  }
```

Two remarks. I've chosen a static class for registry implementation instead of making it a singleton option. Yeah, registry could be a singleton pattern, but it would be excessive complication. We don't need advantages of such approach like inheritance from interfaces or passing registry as parameters to function. The "static class" best situates the problem. Keep it simple stupid. More read at [Static vs Singleton].

I'd like to say is that as a way to access the object suggests to use the property instead of method. But unfortunately it is a limitation of C#. This [Generic Properties] blog post from Julian Bucknall is a pretty good explanation. Essentially it's a heap allocation problem.

Let's back to our muttons. Where do Singleton classes register themselves? One possibility is in their constructor. For example, a MySingleton subclass could do the following:
```
  public sealed class MySingleton : ISingleton
  {
    /// <summary>
    /// explicit static ctor that initialize our singleton
    /// </summary>
    static MySingleton()
    {
      new MySingleton();
    }

    /// <summary>
    /// private ctor that register itself in Registry
    /// </summary>
    private MySingleton()
    {
      Registry.Register(this);
    }
  }
```
But who does call the static constructor? Referring to MSDN it's called automatically before the creation of the first instance or reference to any static members. We could defined static field and touch it somewhere to initialize statics, like this:
```
public sealed class MySingleton
{
  private static readonly MySingleton instance = null;
  static MySingleton() { instance = new MySingleton(); }

  // any no-op method call accepting your object will do fine
  public static void TouchMe() { Equals(instance, null); }
  ...
}
```
Somewhere in the code:
```
// initialize statics
MySingleton.TouchMe();
```

Disadvantages of this way is that first of all we need remembering to call TouchMe(). We can get around this problem in C++ by defining a static instance of MySingleton in the cpp-file that contains MySingleton's implementation. But where do we have to call it in C#? Suppose we introduce the function, the-big-init-function, and call there all of our TouchMe. I might be the black sheep here, but I truly believe that this is a bad idea. Besides, it has a potential drawback, namely that instances of all possible Singletons must be created, or else they won't get registered. But I suppose we do not want initialize all exiting singletons before they actually called and even if they never will be called.

So, another way to force a class constructor to run without having a reference to the type (i.e. reflection), you can use:
```
RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
```
Thus GetInstanceOf could be as follows:
```
    public static ISingleton GetInstanceOf<T>()
    {
      RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
      return registry.OfType<T>().FirstOrDefault();
    }
```
Static constructor is called once, which is guaranted by CLR. Simple enough and just works! In this case, when we are asking Registry for the Singleton it run static constructor that register itself in registry list and returns the instance from the list. Good article about type constructors at [David Notario]'s WebLog, if I may suggest.

##Speed and Thread-safe
One thing which is of concern is a thread-safe. Suppose we have a bunch of threads that asking registry for singletons. What if the one thread is have to change registry list, while the other is accessing to it at the same time? Well one could use lock to prevent entering others thread to exclusive block:
```
  lock(registry) {
    RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
    return registry.OfType<T>().FirstOrDefault();
  }
```
On the one hand it is a good solution and the other redundancy. Because when the all singletons are initialized the list would stay unchanged to the exit and searching instance from the registry does not require a critical section. Moreover this solution does not protect Register() function accessing from other threads. Let's have a look at situation, if someone from the other thread called static constructor it invokes singleton registration in registry list and from another thread someone try to get instance of another singleton. In that case we will likely access to potentially changing list.

So I wrote a class that guarantees access to the critical section when all other threads that do not require exclusive access are gone and any other thread does not enter a critical section of code. I called it SmartLock:
```
  /// <summary>
  /// Smart lock written by myself
  /// It can be used when you need to block some piece of code with exclusive access
  /// </summary>
  public class SmartLock
  {
    private int nonblocking = 0;
    private int blocking = 0;

    // false - you shall not pass, true - all pass
    private readonly ManualResetEvent nonblockers = new ManualResetEvent(true);

    // false - you shall not pass, true - first pass
    private readonly AutoResetEvent blockers = new AutoResetEvent(false);

    /// <summary>
    /// All threads that does not require critical section
    /// are starting from here
    /// </summary>
    public void In();

    /// <summary>
    /// All threads that does not require critical section
    /// are exiting from here
    /// </summary>
    public void Out();

    /// <summary>
    /// Exclusive threads are entering here
    /// </summary>
    public void Enter();

    /// <summary>
    /// Exclusive threads are exiting here
    /// </summary>
    public void Exit();
  }
```
According to this we need to rewrite Registry:
```
    public static void Register(ISingleton singleton)
    {
      if (singleton == null) return;
      smart.Enter();
      registry.Add(singleton); // critical section
      smart.Exit();
    }

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
```
Finally I give you a time intervals which are obtained by a particular solution:
```
      // 25s
      ISingleton instance;
      lock (registry) {
        if (!registry.OfType<T>().Any()) RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
        instance = registry.OfType<T>().FirstOrDefault();
      }

      // 16s
      ISingleton instance;
      lock (registry) {
        RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
        instance = registry.OfType<T>().FirstOrDefault();
      }

      // 624s
      ISingleton instance;
      var handle = typeof(T).TypeHandle;
      simple.Enter();
      RuntimeHelpers.RunClassConstructor(handle);
      instance = registry.OfType<T>().FirstOrDefault();
      simple.Exit();

      // 12s
      smart.In();
      var exist = registry.OfType<T>().Any();
      smart.Out();
      if (!exist) {
        smart.Enter();
        RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
        smart.Exit();
      }
      smart.In();
      var instance = registry.OfType<T>().FirstOrDefault();
      smart.Out();

      // 44s
      smart.Enter();
      RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
      smart.Exit();

      smart.In();
      var instance = registry.OfType<T>().FirstOrDefault();
      smart.Out();

      // unknown
      ISingleton instance;
      bool gotLock = false;
      try {
        spin.Enter(ref gotLock);
        RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
        instance = registry.OfType<T>().FirstOrDefault();
      } finally {
        if (gotLock) spin.Exit();
      }

      // 4s, non thread-safe for initialization (just for testing a maximum speed)
      RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle); // initialization was in single thread before
      var instance = registry.OfType<T>().FirstOrDefault();

      // 6.7s
      RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle); // with Enter\Exit in Register()
      smart.In();
      var instance = registry.OfType<T>().FirstOrDefault();
      smart.Out();
      if (instance == null) {
        smart.Enter();
        instance = registry.OfType<T>().FirstOrDefault();
        smart.Exit();
      }
```
As we can see lock-way is 2-3 times slower than my solution.

##General Registry
I would wondering if someone wants to create a general registry, but it is possible. It could look like this:
```
  public static class Registry<I> where I : class
  {
    private static readonly List<I> registry = new List<I>();
    private static readonly SmartLock smart = new SmartLock();

    public static void Register(I singleton) { ... }

    public static I InstanceOf<T>() where T : class, I { ... }
  }
```
And we are able to write something like this:
```
Registry<ISingletion>.InstanceOf<MySingleton>().AnySingletonFunctionDo();
Registry<AbstractFactory>.InstanceOf<ConcreteFactory>().Create();
```
So we can add singletons in the appropriate registeries without any changes to the registry's code. This is possible due to the fact that Registry<ISingletion> and Registry<AbstractFactory> represents different static classes with corresponding sets of class fields. So that they will work independently and will not interfere with each other.

But be aware and do not write stuff like that
```
Registry<ISingletion>.InstanceOf<MySingleton>().AnySingletonFunctionDo();
Registry<MySingleton>.InstanceOf<MySingleton>().AnySingletonFunctionDo();
```
Whatever you want it.

**Code is a poetry**

[andykras]:http://andykras.org
[Static vs Singleton]:http://dotnet.dzone.com/news/c-singleton-pattern-vs-static-
[Generic Properties]:http://www.boyet.com/Articles/GenericProperties.html
[David Notario]:http://blogs.msdn.com/b/davidnotario/archive/2005/02/08/369593.aspx
