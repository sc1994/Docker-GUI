using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using src.Controllers.Images.Dtos;
using src.Repositories;

namespace DockerGui.Controllers.Images
{
    public class ImageController : ApiBaseController
    {
        [HttpGet("refresh")]
        public async Task<bool> RefreshImagesAsync()
        {
            return await GetClientAsync(async client =>
            {
                var imageList = await client.Images.ListImagesAsync(new ImagesListParameters
                {
                    All = true
                });
                ImageRepository.ALL_IMAGES.Clear();
                foreach (var item in imageList)
                {
                    ImageRepository.ALL_IMAGES.Add(item);
                }
                return true;
            });
        }

        /// <summary>
        /// 获取本地镜像
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ImageListResponseDto>> SearchLocalListAsync(string match = "")
        {
            if (!ImageRepository.ALL_IMAGES.Any()) await RefreshImagesAsync();

            var m = ImageRepository.ALL_IMAGES?.Where(
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
                    Term = match
                });
                return images.AsEnumerable()
                            .OrderByDescending(x => x.IsOfficial)
                            .ThenByDescending(x => x.StarCount)
                            .ThenBy(x => x.Name);
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
                                 var f = ImageRepository.ALL_IMAGES.FirstOrDefault(x => x.RepoTags.Any(a => a == s));
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

                                 var c = ImageRepository.ALL_IMAGES.Where(a => a.ParentID == f.ID);
                                 if (c.Any())
                                 {
                                     t.Children = MapToImageListDto(c);
                                 }
                                 return t;
                             }).ToList();
                             return i;
                         });
        }
    }
}