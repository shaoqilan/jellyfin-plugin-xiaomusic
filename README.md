# XiaoMusic Jellyfin Plugin

将 **Jellyfin 音乐库** 转换为 **XiaoMusic 项目可用的歌单配置文件**的插件。  
插件会自动读取 Jellyfin 中的音乐与播放列表，并通过网络接口输出为 XiaoMusic 可识别的歌单格式。

---

# 插件仓库地址
https://raw.githubusercontent.com/shaoqilan/jellyfin-plugin-xiaomusic/refs/heads/main/10.11/manifest.json

安装插件
添加插件存储库：

国内加速：https://ghfast.top/https://raw.githubusercontent.com/shaoqilan/jellyfin-plugin-xiaomusic/refs/heads/main/10.11/manifest.json

国外访问：https://raw.githubusercontent.com/shaoqilan/jellyfin-plugin-xiaomusic/refs/heads/main/10.11/manifest.json

如果都无法访问，可以直接从 Release 页面下载，并解压到 jellyfin 插件目录中使用

## ✨ 功能特性

- 自动读取 Jellyfin 音乐库
- 支持读取用户自定义播放列表
- 支持过滤指定歌单
- 自动生成“全部歌曲”歌单（可选择忽略）
- 提供 HTTP 接口供 XiaoMusic 拉取歌单
- 支持多用户（按用户名区分）

---

## 🌐 网络接口说明

### **获取歌单列表**

http://jellyfinserver:8096/XiaoMusic/GetMusicPlayList?UserName=&Ignore=xx|xx


### 参数说明

| 参数名 | 说明 |
|--------|------|
| **UserName** | 指定要拉取其歌单的 Jellyfin 用户名 |
| **Ignore** | 忽略指定的歌单，多个使用 `|` 分隔 |

---

## 🎵 默认行为说明

接口默认会返回一个名为 **“全部歌曲”** 的歌单。

如果你希望忽略该歌单，请在 `Ignore` 参数中加入：all

