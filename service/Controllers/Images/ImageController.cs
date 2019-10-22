using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DockerGui.Controllers.Images.Dtos;
using DockerGui.Hubs;
using DockerGui.Repositories;

namespace DockerGui.Controllers.Images
{
    public class ImageController : ApiBaseController
    {
        private readonly IHubContext<BaseHub> _hub;
        private readonly ILogger<ImageController> _log;

        public ImageController(
            IHubContext<BaseHub> hub,
            ILogger<ImageController> log
        ) : base(hub, log)
        {
            _hub = hub;
            _log = log;
        }

        [HttpGet("refresh")]
        public async Task<bool> RefreshImagesAsync()
        {
            return await GetClientAsync(async client =>
            {
                var imageList = await client.Images.ListImagesAsync(new ImagesListParameters
                {
                    All = true
                });
                StaticValue.ALL_IMAGES.Clear();
                foreach (var item in imageList)
                {
                    StaticValue.ALL_IMAGES.Add(item);
                }
                return true;
            });
        }

        /// <summary>
        /// 获取本地镜像
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<ImageListResponseDto>> SearchLocalListAsync(string match = "")
        {
            if (!StaticValue.ALL_IMAGES.Any()) await RefreshImagesAsync();

            var m = StaticValue.ALL_IMAGES?.Where(
               x => (x.RepoDigests?.Any(a => a.Contains(match)) ?? false)
               || (x.RepoTags?.Any(a => a.Contains(match)) ?? false)
            ).Where(
                x => string.IsNullOrWhiteSpace(x.ParentID)
            );

            var r = MapToImageListDto(m);

            return r;
        }

        /// <summary>
        /// 获取远程镜像
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        [HttpGet("search/{match}")]
        public async Task<IEnumerable<ImageSearchResponse>> SearchRemoteListAsync(string match)
        {
            return await GetClientAsync(async client =>
            {
                var images = await client.Images.SearchImagesAsync(new ImagesSearchParameters
                {
                    Term = match,
                    RegistryAuth = new AuthConfig
                    {

                    }
                });
                return images.AsEnumerable()
                            .OrderByDescending(x => x.IsOfficial)
                            .ThenByDescending(x => x.StarCount)
                            .ThenBy(x => x.Name);
            });
        }

        /// <summary>
        /// 获取远程tag根据镜像名称
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpGet("search/tags")]
        public async Task<List<RemoteTagListDto>> GetRemoteTagListAsync(string image)
        {
            using (var client = new HttpClient())
            {
                var res = await client.GetAsync($"https://registry.hub.docker.com/v1/repositories/{image}/tags");
                var str = await res.Content.ReadAsStringAsync();
                _log.LogInformation(str);
                return JsonConvert.DeserializeObject<List<RemoteTagListDto>>(str);
            }
        }

        /// <summary>
        /// 拉取远程镜像
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("pull")]
        public async Task PullRemoteImage(ImagesCreateParameters input)
        {
            await GetClientAsync(async client =>
            {
                var progress = new Progress<JSONMessage>();
                progress.ProgressChanged += async (obj, message) =>
                 {
                     _log.LogInformation(JsonConvert.SerializeObject(message));
                     await _hub.Clients.Group(Token).SendCoreAsync("pull", new[] { message });
                 };
                await client.Images.CreateImageAsync(
                    input,
                    new AuthConfig
                    {

                    },
                    progress
                );
            });
        }

        private IEnumerable<ImageListResponseDto> MapToImageListDto(IEnumerable<ImagesListResponse> source)
        {
            return source.SelectMany(x => x.RepoTags)
                         .GroupBy(x => x.Split(':')[0])
                         .Select(x =>
                         {
                             var i = new ImageListResponseDto();
                             i.Repository = x.Key;
                             i.Tags = x.Select(s =>
                             {
                                 var f = StaticValue.ALL_IMAGES.FirstOrDefault(x => x.RepoTags.Any(a => a == s));
                                 var t = new ImageTagListResponseDto
                                 {
                                     Tag = s.Split(':')[1],
                                     ImageId = f.ID,
                                     Size = f.Size,
                                     Created = f.Created.ToString("yyyy-MM-dd HH:mm"),
                                     RepoDigests = f.RepoDigests,
                                     Labels = f.Labels,
                                     SharedSize = f.SharedSize,
                                     VirtualSize = f.VirtualSize
                                 };

                                 var c = StaticValue.ALL_IMAGES.Where(a => a.ParentID == f.ID);
                                 if (c.Any())
                                 {
                                     t.Children = MapToImageListDto(c);
                                 }
                                 return t;
                             })
                             .OrderByDescending(x => x.Tag)
                             .ToList();
                             return i;
                         });
        }
    }
}