using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.XiaoMusic
{
    /// <summary>
    /// 注入管理
    /// </summary>
    public class RegistratorManager : IPluginServiceRegistrator
    {
        /// <summary>
        /// 服务注入
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="applicationHost"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            //注入音乐导出服务
            serviceCollection.AddScoped<Services.IMusicExportService, Services.MusicExportService>();
        }
    }
}
