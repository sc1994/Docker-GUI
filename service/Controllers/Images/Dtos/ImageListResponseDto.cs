using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DockerGui.Controllers.Images.Dtos
{
    public class ImageListResponseDto
    {
        public string Repository { get; set; }
        public List<ImageTagListResponseDto> Tags { get; set; } = new List<ImageTagListResponseDto>();
    }

    public class ImageTagListResponseDto
    {
        public string Tag { get; set; }
        public string ImageId { get; set; }
        public string Created { get; set; }
        public long Size { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<string> RepoDigests { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Labels { get; set; }
        public long SharedSize { get; set; }
        public long VirtualSize { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<ImageListResponseDto> Children { get; set; }
    }
}