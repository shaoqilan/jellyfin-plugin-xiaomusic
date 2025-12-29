using Jellyfin.Plugin.XiaoMusic.Models;
using Jellyfin.Plugin.XiaoMusic.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jellyfin.Plugin.XiaoMusic.Controllers
{
    /// <summary>
    /// 小爱音乐控制器
    /// </summary>
    [ApiController]
    [Route("[controller]/[action]")]
    public class XiaoMusicController : ControllerBase
    {
        #region 操作声明
        /// <summary>
        /// 
        /// </summary>
        private readonly IMusicExportService musicExportService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="musicExportService"></param>
        public XiaoMusicController(IMusicExportService musicExportService)
        {
            this.musicExportService = musicExportService;
        }
        #endregion
        /// <summary>
        /// 获取音乐播放列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMusicPlayList([FromQuery] QueryMusicPlayList query)
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var result = musicExportService.GetMusicPlayList(query, baseUrl);
            return Ok(result);
        }
    }
}
