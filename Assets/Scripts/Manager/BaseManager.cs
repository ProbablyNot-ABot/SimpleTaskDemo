public abstract class BaseManager<T> where T : new()
{
    private static T instance;
    public static T Instance 
    {
        get
        {
            instance ??= new T();
            return instance;
        }
     }
}
