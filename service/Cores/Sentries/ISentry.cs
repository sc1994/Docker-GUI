using System.Threading;
using Docker.DotNet;

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
        CancellationTokenSource StartLogs(DockerClient client, string id);

        /// <summary>
        /// 启动统计监听
        /// </summary>
        /// <param name="client"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        CancellationTokenSource StartStats(DockerClient client, string id);
    }
}