using Jellyfin.Plugin.XiaoMusic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.XiaoMusic.Services
{
    /// <summary>
    /// 音乐导出服务接口
    /// </summary>
    public interface IMusicExportService
    {
        /// <summary>
        /// 获取音乐播放列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        IEnumerable<MusicPlayList> GetMusicPlayList(QueryMusicPlayList query);
    }
}
