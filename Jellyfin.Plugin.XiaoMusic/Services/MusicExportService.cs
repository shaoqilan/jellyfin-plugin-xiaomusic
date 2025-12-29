using Jellyfin.Data.Enums;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Database.Implementations.Entities.Libraries;
using Jellyfin.Database.Implementations.Entities.Security;
using Jellyfin.Plugin.XiaoMusic.Models;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Playlists;
using MediaBrowser.Model.Dto;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="libraryManager"></param>
        public MusicExportService(ILibraryManager libraryManager, IPlaylistManager playlistManager, IUserManager userManager)
        {
            this.libraryManager = libraryManager;
            this.playlistManager = playlistManager;
            this.userManager = userManager;
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
        public IEnumerable<MusicPlayList> GetMusicPlayList(QueryMusicPlayList query, string baseUrl)
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
                        BaseUrl = baseUrl,
                        Name = p.Name,
                        Id = p.Id.ToString()
                    }).ToList()
                });
            }
            var customList = GetCustomMusicPlayList(user, baseUrl);
            playlists.AddRange(customList.Where(w=> !query.IsIgnore(w.Id)));
            //返回歌单
            return playlists;
        }
        /// <summary>
        /// 获取所有自定义的歌单
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private IEnumerable<MusicPlayList> GetCustomMusicPlayList(User? user, string baseUrl)
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
                        BaseUrl = baseUrl,
                        Name = p.Name,
                        Id = p.Id.ToString()
                    }).ToList()
                };
            });
        }
    }
}
