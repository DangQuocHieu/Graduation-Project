namespace DQHieu.Framework
{
    using UnityEngine;

    public class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T instance;
        public static T Instance => instance;
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Destroy(instance.gameObject);
            }
            instance = this as T;
        }
    }

}