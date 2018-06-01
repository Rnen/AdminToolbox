using Smod2.Events;
using Smod2.API;

namespace AdminToolbox
{
    class AuthCheck : IEventAuthCheck
    {
        private AdminToolbox plugin;

        public AuthCheck(AdminToolbox plugin)
        {
            this.plugin = plugin;
        }
        public void OnAuthCheck(Player admin, AuthType authType, string entered_password, string server_password, bool allowOverwrite, out bool allowOutput)
        {
            plugin.Info(admin + " " + authType + " " + entered_password + " " + server_password + " " + allowOverwrite);
            allowOutput = allowOverwrite;
        }
    }
    class AdminQuery : IEventAdminQuery
    {
        private AdminToolbox plugin;

        public AdminQuery(AdminToolbox plugin)
        {
            this.plugin = plugin;
        }
        public void OnAdminQuery(Player admin, string adminIp, string query, out string queryOutput)
        {
            plugin.Info(admin + " " + adminIp + " " + query);
            queryOutput = query;
        }
    }
}
