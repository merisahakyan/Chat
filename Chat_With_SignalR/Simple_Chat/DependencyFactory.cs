using BLL;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Repo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simple_Chat
{
    public class DependencyFactory
    {
        private static IUnityContainer _container;

        public static IUnityContainer Container
        {
            get
            {
                return _container;
            }
            private set
            {
                _container = value;
            }
        }

        static DependencyFactory()
        {
            var container = new UnityContainer();
            container.RegisterType<IDataManager, DataManager>(new ContainerControlledLifetimeManager());
            container.RegisterType<IManager, Manager>(new ContainerControlledLifetimeManager());
            
            var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            if (section != null)
            {
                section.Configure(container);
            }
            _container = container;
        }

        public static T Resolve<T>()
        {
            T ret = default(T);

            if (Container.IsRegistered(typeof(T)))
            {
                ret = Container.Resolve<T>();
            }

            return ret;
        }
    }
}
