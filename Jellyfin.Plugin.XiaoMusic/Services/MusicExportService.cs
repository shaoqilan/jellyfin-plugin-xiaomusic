using Jellyfin.Data.Enums;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Extensions;
using Jellyfin.Plugin.XiaoMusic.Configuration;
using Jellyfin.Plugin.XiaoMusic.Models;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Model.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Data;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.XiaoMusic.Services
{
    /// <summary>
    /// 音乐导出服务
    /// </summary>
    public class MusicExportService : IMusicExportService
    {
        #region 操作声明
        /// <summary>
        /// 库管理器
        /// </summary>
        private readonly ILibraryManager libraryManager;
        private readonly IPlaylistManager playlistManager;
        private readonly IUserManager userManager;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ILogger<MusicExportService> logger;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="libraryManager"></param>
        public MusicExportService(ILibraryManager libraryManager, IPlaylistManager playlistManager, IUserManager userManager, IHttpContextAccessor httpContextAccessor, ILogger<MusicExportService> logger)
        {
            this.libraryManager = libraryManager;
            this.playlistManager = playlistManager;
            this.userManager = userManager;
            this.httpContextAccessor = httpContextAccessor;
            this.logger = logger;
        }
        #endregion
        /// <summary>
        /// 获取用户的信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private User? GetUser(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            return userManager.GetUserByName(name);
        }
        /// <summary>
        /// 获取音乐播放列表
        /// </summary>
        /// <param name="query"></param>
        /// <param name="baseUrl"></param>
        /// <returns></returns>
        public IEnumerable<MusicPlayList> GetMusicPlayList(QueryMusicPlayList query)
        {
            //指定的用户信息
            var user = GetUser(query.UserName ?? string.Empty);
            //获取所有音乐文件
            var items = libraryManager.GetItemList(new InternalItemsQuery(user)
            {
                //过滤媒体信息
                IncludeItemTypes = [
                    Data.Enums.BaseItemKind.Audio
                ],
                Recursive = true
            });
            //构建歌单
            var playlists = new List<MusicPlayList>();
            if (!query.IsIgnore("all"))
            {
                //默认全部音乐
                playlists.Add(new MusicPlayList()
                {
                    Name = "全部音乐",
                    Id = "all",
                    Musics = items.Select(p => new MusicItem()
                    {
                        Name = p.Name,
                        Id = p.Id.ToString(),
                        Url = GetPlayUrl(p, user)
                    }).ToList()
                });
            }
            var customList = GetCustomMusicPlayList(user);
            playlists.AddRange(customList.Where(w => !query.IsIgnore(w.Id)));
            //返回歌单
            return playlists;
        }
        /// <summary>
        /// 获取所有自定义的歌单
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private IEnumerable<MusicPlayList> GetCustomMusicPlayList(User? user)
        {
            if (user is null)
            {
                return new List<MusicPlayList>();
            }
            //获取所有歌单
            var playlists = playlistManager.GetPlaylists(user.Id).Where(w => w.MediaType == MediaType.Audio).ToList();
            return playlists.Select(item =>
            {
                var items = item.GetChildren(user, true, new InternalItemsQuery(user)
                {
                    //过滤媒体信息
                    IncludeItemTypes = [
                        BaseItemKind.Audio
                    ],
                    Recursive = true
                });
                return new MusicPlayList()
                {
                    Id = item.Id.ToString(),
                    Name = item.Name,
                    Musics = items.Select(p => new MusicItem()
                    {
                        Name = p.Name,
                        Id = p.Id.ToString(),
                        Url = GetPlayUrl(p, user)
                    }).ToList()
                };
            });
        }
        /// <summary>
        /// 根据规则获取播放地址
        /// </summary>
        private string GetPlayUrl(BaseItem item, User? user)
        {
            var config = Plugin.Instance?.Configuration;
            ArgumentNullException.ThrowIfNull(config);
            string url = config.DefaultPlayUrl ?? string.Empty;
            var playUrlRule = config.PlayUrlRule ?? new List<PlayUrlRule>();
            //过滤掉空的
            playUrlRule = playUrlRule.Where(w => !string.IsNullOrWhiteSpace(w.Filter)).ToList();
            // 遍历所有规则
            foreach (var rule in playUrlRule)
            {
                //分割规则
                var ruleValue = rule.Filter.Split('|', StringSplitOptions.RemoveEmptyEntries).ToList();
                //开始匹配
                if (ruleValue.Exists(r => FileSystemName.MatchesSimpleExpression(r,Path.GetFileName(item.Path))))
                {
                    //返回地址
                    url = rule.PlayUrl ?? string.Empty;
                    break;
                }
            }
            // 没有匹配任何规则，返回默认播放地址
            return ConvertPlayUrl(url, item, user);
        }
        /// <summary>
        /// 转换地址
        /// </summary>
        /// <returns></returns>
        private string ConvertPlayUrl(string url, BaseItem item, User? user)
        {
            //支持的参数
            var dict = new Dictionary<string, string>
            {
                ["Scheme"] = httpContextAccessor.HttpContext?.Request.Scheme ?? string.Empty,
                ["Host"] = httpContextAccessor.HttpContext?.Request.Host.Value ?? string.Empty,
                ["PathBase"] = httpContextAccessor.HttpContext?.Request.PathBase.Value ?? string.Empty,
                ["Path"] = httpContextAccessor.HttpContext?.Request.Path.Value ?? string.Empty,
                ["UserId"] = user?.Id.ToString() ?? string.Empty,
                ["Id"] = item.Id.ToString() ?? string.Empty,
            };
            var query = httpContextAccessor.HttpContext?.Request.Query;
            if (query != null)
            {
                foreach (var qvalue in query)
                {
                    if (!dict.Keys.Contains(qvalue.Key))
                    {
                        dict.Add(qvalue.Key, qvalue.Value.ToString());
                    }
                }
            }
            logger.LogInformation($"XiaoMusic ConvertPlayUrl url={url} dict={JsonSerializer.Serialize(dict)}");
            string payUrl = FormatTemplate(url, dict);
            return payUrl;
        }
        /// <summary>
        /// 格式化模板
        /// </summary>
        /// <param name="template"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private static string FormatTemplate(string template, IDictionary<string, string> values)
        {
            if (string.IsNullOrEmpty(template) || values == null)
                return template;
            foreach (var kv in values)
            {
                template = template.Replace("{" + kv.Key + "}", kv.Value ?? string.Empty);
            }
            return template;
        }
    }
}
