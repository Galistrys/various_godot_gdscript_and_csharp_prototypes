using Godot;
using System;

public class Singletons : Node
{
    public static Logger Logger { get; private set; }
    public static Database Database { get; private set; }
    public static MainManager MainManager { get; private set; }
    public static NetworkManager NetworkManager { get; private set; }
    
    static Singletons Instance { get; set; }

    public override void _Ready()
    {
        if (Instance != null) throw new InvalidOperationException();
        Instance = this;

        Logger = GetSingleton<Logger>();
        MainManager = GetSingleton<MainManager>();
        NetworkManager = GetSingleton<NetworkManager>();
        Database = GetSingleton<Database>();
    }

    public static T GetSingleton<T>() where T : class, ISingleton
    {
        T singleton = GetSingletonOrNull(typeof(T).ToString()) as T;
        if (singleton == null)
        {
            Logger.Log("There isn't any singleton of type " + typeof(T), LogLevel.Error);
            return null;
        }
        return singleton;
    }

    static Node GetSingletonOrNull(string name)
    {
        return Instance.GetNodeOrNull("/root/" + name);
    }

    public static void ResetSingletons()
    {
        Logger.ResetSingleton();
        MainManager.ResetSingleton();
        NetworkManager.ResetSingleton();
        Database.ResetSingleton();
    }
}
