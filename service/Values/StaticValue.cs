using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Docker.DotNet.Models;

namespace DockerGui.Values
{
    /// <summary>
    /// 静态值,
    /// </summary>
    public static class StaticValue
    {
        public static string DOCKERAPI = "http://localhost:2375";

        public static IList<ImagesListResponse> LOCAL_IMAGES { get; } = new List<ImagesListResponse>();

        public static IList<ContainerListResponse> CONTAINERS { get; } = new List<ContainerListResponse>();

        /// <summary>
        /// 监控中的线程集合
        /// </summary>
        /// <returns></returns>
        public static ConcurrentDictionary<string, CancellationTokenSource> MONITOR_THREAD { get; } = new ConcurrentDictionary<string, CancellationTokenSource>();
    }
}