using UnityEngine;

public class Singelton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get;  set; }

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
         
                
        }
        else if ( Instance!= this)
        {
            Destroy(gameObject);
        }
           

    }
}
