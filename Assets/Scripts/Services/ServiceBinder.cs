using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockAndGun.Services
{
    public class ServiceBinder : MonoBehaviour
    {
        [SerializeField] public List<MonoBehaviour> servicesToBind = new List<MonoBehaviour>();

        private void Awake()
        {
            ServiceLocator serviceLocator = ServiceLocator.Instance;

            foreach(MonoBehaviour service in servicesToBind)
            {
                Type serviceType = service.GetType();

                Type serviceInterface = serviceType.GetInterfaces()[0];

                if(serviceInterface != null)
                {
                    serviceLocator.Register(serviceInterface, service);
                }
                else
                {
                    serviceLocator.Register(serviceType, service);
                }
            }

        }
    }
}
