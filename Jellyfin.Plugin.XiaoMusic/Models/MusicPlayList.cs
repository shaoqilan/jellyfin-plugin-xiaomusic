using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.XiaoMusic.Models
{
    /// <summary>
    /// 音乐播放列表
    /// </summary>
    public class MusicPlayList
    {
        /// <summary>
        /// 歌单的唯一标识
        /// </summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// 歌单的名称
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 歌单中的音乐列表
        /// </summary>
        [JsonPropertyName("musics")]
        public List<MusicItem> Musics { get; set; } = new List<MusicItem>();
    }
}
