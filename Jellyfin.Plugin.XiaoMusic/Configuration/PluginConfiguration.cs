using MediaBrowser.Model.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.XiaoMusic.Configuration
{
    /// <summary>
    /// 插件配置
    /// </summary>
    public class PluginConfiguration : BasePluginConfiguration
    {
        public PluginConfiguration()
        {
            this.DefaultPlayUrl = "{Scheme}://{Host}/Audio/{Id}/stream?static=true";
            this.PlayUrlRule = new List<PlayUrlRule>();
        }
        /// <summary>
        /// 默认的播放地址
        /// </summary>
        public string DefaultPlayUrl { get; set; }
        /// <summary>
        /// 播放地址规则
        /// </summary>
        public List<PlayUrlRule> PlayUrlRule { get; set; }
    }
    /// <summary>
    /// 播放地址规则
    /// </summary>
    public class PlayUrlRule
    {
        /// <summary>
        /// 文件过滤
        /// </summary>
        public string Filter { get; set; } = string.Empty;
        /// <summary>
        /// 播放地址
        /// </summary>
        public string PlayUrl { get; set; } = string.Empty;
    }
}
