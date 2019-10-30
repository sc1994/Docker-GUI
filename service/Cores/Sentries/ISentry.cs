using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using DockerGui.Cores.Sentries.Models;
using DockerGui.Repositories;
using DockerGui.Values;

namespace DockerGui.Cores.Sentries
{
    public interface ISentry
    {
        /// <summary>
        /// 启动日志监听(每次开启会重置掉已被存储的日志数据)
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        CancellationTokenSource StartLogs(DockerClient client,
                                          string id,
                                          Action<string, string, long> backCall = null);

        /// <summary>
        /// 启动统计监听
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        CancellationTokenSource StartStats(DockerClient client,
                                           string id,
                                           Action<string, SentryStats, SentryStatsGapEnum, long> backCall = null);

        /// <summary>
        /// 提供给job运行的监听程序
        /// </summary>
        /// <param name="id"></param>
        void StartStats(string id);

        /// <summary>
        /// 获取日志数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        Task<List<string>> GetLogsAsync(string id, int page, int count);

        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timeRange"></param>
        /// <returns></returns>
        Task<List<SentryStats>> GetStatsAsync(string id, DateTime[] timeRange);
    }
}