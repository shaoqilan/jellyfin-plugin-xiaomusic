using Jellyfin.Plugin.XiaoMusic.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.XiaoMusic
{
    /// <summary>
    /// 插件信息
    /// </summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="appHost"></param>
        /// <param name="applicationPaths"></param>
        /// <param name="xmlSerializer"></param>
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }
        /// <summary>
        /// 插件的实例
        /// </summary>
        public static Plugin? Instance { get; private set; }
        /// <summary>
        /// 插件名称
        /// </summary>
        public override string Name => "XiaoMusic";
        /// <summary>
        /// 插件ID
        /// </summary>
        public override Guid Id => Guid.Parse("ac2c8cbd-1097-4fc7-8052-db71eb7c9da5");
        /// <summary>
        /// 插件描述
        /// </summary>
        public override string Description => "将库中音乐转成xiaomusic项目可用的歌单配置文件";
        /// <summary>
        /// 插件的配置也买你
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PluginPageInfo> GetPages()
        {
            return
            [
                new PluginPageInfo
                {
                    Name = Name,
                    EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", GetType().Namespace)
                }
            ];
        }
    }
}
