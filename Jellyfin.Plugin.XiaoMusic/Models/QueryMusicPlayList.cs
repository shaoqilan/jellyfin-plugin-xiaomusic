using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.XiaoMusic.Models
{
    /// <summary>
    /// 查询音乐播放列表
    /// </summary>
    public class QueryMusicPlayList
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; } = string.Empty;
        /// <summary>
        /// 忽略的播放列表名称id|id
        /// </summary>
        public string? Ignore { get; set; } = string.Empty;
        /// <summary>
        /// 获取忽略列表
        /// </summary>
        /// <returns></returns>
        private List<string> GetIgnoreList()
        {
            if (string.IsNullOrEmpty(Ignore))
            {
                return new List<string>();
            }
            return Ignore.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        /// <summary>
        /// 是否忽略
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsIgnore(string id)
        {
            var ignore = GetIgnoreList();
            if (ignore.Count > 0 && ignore.Contains(id))
            {
                //忽略
                return true;
            }
            return false;
        }
    }
}
