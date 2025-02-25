using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlockAndGun.Services
{
    public class BinderService : MonoBehaviour
    {
        [SerializeField] public List<MonoBehaviour> servicesToBind = new List<MonoBehaviour>();

        private void Awake()
        {
            LocatorService serviceLocator = LocatorService.Instance;

            foreach(MonoBehaviour service in servicesToBind)
            {
                Type serviceType = service.GetType();

                Type serviceInterface = serviceType.GetInterfaces().FirstOrDefault();

                //Si il y a une interface on l'enregistre
                if(serviceInterface != null)
                {
                    serviceLocator.Register(serviceInterface, service);
                }
                // ou sinon on enregistre le service directement
                else
                {
                    serviceLocator.Register(serviceType, service);
                }
            }

        }
    }
}
