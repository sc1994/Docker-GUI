using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Docker.DotNet.Models;
using DockerGui.Application.Containers.Dtos;
using DockerGui.Core.Containers;
using DockerGui.Core.Hubs;
using DockerGui.Tools.Values;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace DockerGui.Application.Containers
{
    public class ContainerController : ApiBaseController
    {
        private readonly IHubContext<BaseHub> _hub;
        private readonly ILogger<ContainerController> _log;
        private readonly IContainerCore _container;
        private readonly IMapper _mapper;

        public ContainerController(
            IHubContext<BaseHub> hub,
            ILogger<ContainerController> log,
            IContainerCore container,
            IMapper mapper) : base(log)
        {
            _hub = hub;
            _log = log;
            _container = container;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IList<ContainerListResponseDto>> GetContainerList(bool refresh = false)
        {
            var r = await _container.GetContainerListAsync(Client);
            return r.Select(_mapper.Map<ContainerListResponseDto>).ToList();
        }

        /// <summary>
        /// 状态设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("{type}/{id}")]
        public async Task<SetStatusResponseDto> SetStatus(string type, string id)
        {
            bool result;
            if (type == "stop")
            {
                result = await Client.Containers.StopContainerAsync(
                    id,
                    new ContainerStopParameters
                    {
                        // TODO:
                    }
                );
            }
            else if (type == "start")
            {
                result = await Client.Containers.StartContainerAsync(
                    id,
                    new ContainerStartParameters
                    {
                        // TODO:
                    }
                );
            }
            else
            {
                throw new Exception("不能识别的操作类型");
            }

            var list = await GetContainerList();
            return new SetStatusResponseDto
            {
                Result = result,
                List = list
            };
        }

        /// <summary>
        /// 添加监视
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("add/{type}/{id}")]
        public async Task AddMonitor(string type, string id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var key = $"{type}_{Token}";
            if (StaticValue.MONITOR_THREAD.ContainsKey(key))
            {
                CancelMonitor(type);
            }
            StaticValue.MONITOR_THREAD.TryAdd(key, cancellationTokenSource);

            try
            {
                if (type == "stats")
                {
                    var progress = new Progress<ContainerStatsResponse>();
                    progress.ProgressChanged += async (obj, message) =>
                    {
                        await _hub.Clients.Group(Token).SendAsync("monitorStats", message);
                    };
                    await Client.Containers.GetContainerStatsAsync(
                        id,
                        new ContainerStatsParameters
                        {
                            //TODO:
                        },
                        progress,
                        cancellationTokenSource.Token
                    );
                }
                else if (type == "log")
                {
                    var progress = new Progress<string>();
                    var queue = new ConcurrentQueue<string>();

                    progress.ProgressChanged += (obj, message) =>
                    {
                        queue.Enqueue(message);
                    };

                    _ = Task.Run(async () => // 每1ms发送一次
                    {
                        while (true)
                        {
                            if (queue.TryDequeue(out var message))
                            {
                                var rule = "[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}.[0-9]{9}Z";
                                var time = Regex.Matches(message, rule)[0].Value;
                                var v = message.Split(new[] { time }, StringSplitOptions.None)[1];
                                v = v.Replace("\u001b[40m\u001b[1m\u001b[33mwarn\u001b[39m\u001b[22m\u001b[49m:", "[warn]");
                                v = v.Replace("\u001B[41m\u001B[30mfail\u001B[39m\u001B[22m\u001B[49m", "[fail]");
                                await _hub.Clients.Group(Token).SendAsync("monitorLog", v);
                            }

                            await Task.Delay(1);
                        }
                    });

                    await Client.Containers.GetContainerLogsAsync(
                        id,
                        new ContainerLogsParameters
                        {
                            ShowStdout = true, // as the error said, you have to choose one stream either stdout or stderr. If you don't input any of these option to be true, it would panic.
                            ShowStderr = true,
                            Timestamps = true,
                            Follow = true
                            //TODO:
                        },
                        cancellationTokenSource.Token,
                        progress
                    );
                }
                else
                {
                    throw new Exception("不能识别的操作类型");
                }
            }
            catch (IOException ex)
            {
                _log.LogWarning(ex, "退出监控");
                await _hub.Clients.Group(Token).SendAsync("notification", "info", "The normal exit");
            }
        }

        /// <summary>
        /// 取消监视
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("cancel/{type}")]
        public void CancelMonitor(string type)
        {
            if (StaticValue.MONITOR_THREAD.Remove($"{type}_{Token}", out var v))
            {
                v.Cancel();
                v.Dispose();
            }
            else
            {
                _log.LogWarning($"{type}_{Token}----取消失败");
            }
        }
    }
}