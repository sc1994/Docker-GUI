using System.Collections.Generic;
using Docker.DotNet.Models;

namespace src.Repositories
{
    public static class ImageRepository
    {
        public static List<ImagesListResponse> ALL_IMAGES { get; } = new List<ImagesListResponse>();
    }
}