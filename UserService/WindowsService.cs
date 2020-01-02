using System.ServiceProcess;

namespace UserService
{
    class WindowsService : ServiceBase
    {
        readonly Host host;

        public WindowsService(Host host) => this.host = host;

        protected override void OnStart(string[] args) => host.Start().GetAwaiter().GetResult();

        protected override void OnStop() => host.Stop().GetAwaiter().GetResult();
    }
}
