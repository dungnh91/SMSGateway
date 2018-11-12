using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SMSService
{
    public partial class Service : ServiceBase
    {
         public ServiceHost serviceHost = null;
         public Service()
        {
            // Name the Windows Service
            ServiceName = "SMS Service";
        }

        public static void Main(string[] args)
        {

            if (Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            }
            else
            {

                ServiceBase.Run(new Service());
            }

        }

        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
            }

            // Create a ServiceHost for the CalculatorService type and 
            // provide the base address.

            //Uri baseAddress = new Uri("net.tcp://localhost:6969/SMSManagement");

            //NetTcpBinding binding = new NetTcpBinding();

            //serviceHost = new ServiceHost(typeof(SMSManagement), baseAddress);
            //serviceHost.AddServiceEndpoint(typeof(ISMSManagement), binding, baseAddress);

            serviceHost = new ServiceHost(typeof(SMSManagement));

            // Open the ServiceHostBase to create listeners and start 
            // listening for messages.
            serviceHost.Open();
        }

        protected override void OnStop()
        {
            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }
        }
    }

    // Provide the ProjectInstaller class which allows 
    // the service to be installed by the Installutil.exe tool
    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        private ServiceProcessInstaller process;
        private ServiceInstaller service;

        public ProjectInstaller()
        {
            process = new ServiceProcessInstaller();
            process.Account = ServiceAccount.LocalSystem;
            service = new ServiceInstaller();
            service.ServiceName = "SMS Service";
            service.Description = "SMS Service for management";
            Installers.Add(process);
            Installers.Add(service);
        }
    }
}
