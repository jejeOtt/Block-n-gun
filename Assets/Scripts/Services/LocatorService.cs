using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockAndGun.Services
{
    public class LocatorService : MonoBehaviour
    {
        private static LocatorService _instance;
        public static LocatorService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("Locator Service", typeof(LocatorService)).GetComponent<LocatorService>();
                }

                return _instance;
            }

            private set
            {
                _instance = value;
            }
        }

        private Dictionary<Type, object> services = new Dictionary<Type, object>();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Register<T>(T serviceInstance)
        {
            if (!services.ContainsKey(typeof(T)))
            {
                services.Add(typeof(T), serviceInstance);
            }
            else
            {
                services[typeof(T)] = serviceInstance;
            }
        }

        public void Register(Type type, object serviceInstance)
        {
            if (!services.ContainsKey(type))
            {
                services.Add(type, serviceInstance);
            }
            else
            {
                services[type] = serviceInstance;
            }
        }

        public T GetService<T>()
        {
            if (services.TryGetValue(typeof(T), out object serviceObject))
            {
                return (T)serviceObject;
            }
            else
            {
                Debug.LogWarning($"T'as oubli� d'enregistrer le service de type {typeof(T)}");
                return default;
            }
        }

        public bool TryGetService<T>(out T service)
        {
            if (services.TryGetValue(typeof(T), out object serviceObject))
            {
                service = (T)serviceObject;
                return true;
            }
            else
            {
                service = default;
                Debug.LogWarning($"T'as oubli� d'enregistrer le service de type {typeof(T)}");
                return false;
            }
        }
    }
}