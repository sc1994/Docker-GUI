using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Docker.DotNet.Models;

namespace src.Repositories
{
    public static class Repository
    {
        public static List<ImagesListResponse> ALL_IMAGES { get; } = new List<ImagesListResponse>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="string">type_token</typeparam>
        /// <typeparam name="CancellationTokenSource"></typeparam>
        /// <returns></returns>
        public static ConcurrentDictionary<string, CancellationTokenSource> MONITOR_THREAD { get; } = new ConcurrentDictionary<string, CancellationTokenSource>();
    }
}