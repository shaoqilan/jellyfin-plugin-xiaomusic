using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.XiaoMusic.Models
{
    /// <summary>
    /// 音乐项
    /// </summary>
    public class MusicItem
    {
        /// <summary>
        /// 媒体文件id
        /// </summary>
        [JsonIgnore]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 地址
        /// </summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    }
}
