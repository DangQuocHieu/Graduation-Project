namespace DQHieu.Framework
{
    using UnityEngine;
    public class PersistentSingleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        public static T Instance => instance;
        protected virtual void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}