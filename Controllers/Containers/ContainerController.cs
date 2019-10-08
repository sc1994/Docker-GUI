using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using DockerGui.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using src.Controllers.Containers.Dtos;
using src.Hubs;

namespace src.Controllers.Containers
{
    public class ContainerController : ApiBaseController
    {
        private readonly IHubContext<BaseHub> _hub;
        private readonly ILogger<ContainerController> _log;

        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _monitorThread = new ConcurrentDictionary<string, CancellationTokenSource>();

        private static readonly ConcurrentDictionary<string, CancellationTokenSource> _logThread = new ConcurrentDictionary<string, CancellationTokenSource>();

        public ContainerController(
            IHubContext<BaseHub> hub,
            ILogger<ContainerController> log
        ) : base(log)
        {
            _hub = hub;
            _log = log;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IList<ContainerListResponseDto>> GetContainerList()
        {
            return await GetClientAsync(async client =>
            {
                var list = await client.Containers.ListContainersAsync(new ContainersListParameters
                {
                    All = true
                });

                // TODO:参数文档 https://docs.docker.com/engine/api/v1.24/
                /* 
                all     – 1/True/true or 0/False/false, Show all containers. Only running containers are shown by default (i.e., this defaults to false)
                limit   – Show limit last created containers, include non-running ones.
                since   – Show only containers created since Id, include non-running ones.
                before  – Show only containers created before Id, include non-running ones.
                size    – 1/True/true or 0/False/false, Show the containers sizes
                filters - a JSON encoded value of the filters (a map[string][]string) to process on the containers list. Available filters:
                    exited=<int>; -- containers with exit code of <int> ;
                    status=(created	restarting	running	paused	exited	dead)
                    label=key or label="key=value" of a container label
                    isolation=(default	process	hyperv) (Windows daemon only)
                    ancestor=(<image-name>[:<tag>], <image id> or <image@digest>)
                    before=(<container id> or <container name>)
                    since=(<container id> or <container name>)
                    volume=(<volume name> or <mount point destination>)
                    network=(<network id> or <network name>)
                */
                return list.Select(x =>
                {
                    var r = JsonConvert.DeserializeObject<ContainerListResponseDto>(JsonConvert.SerializeObject(x));
                    r.CreatedStr = x.Created.ToString("yyyy-MM-dd HH:mm");
                    return r;
                }).ToList();
            });
        }

        /// <summary>
        /// 状态设置
        /// </summary>
        /// <returns></returns>
        [HttpGet("{type}/{id}")]
        public async Task<SetStatusResponseDto> SetStatus(string type, string id)
        {
            return await GetClientAsync(async client =>
            {
                bool result;
                if (type == "stop")
                {
                    result = await client.Containers.StopContainerAsync(
                        id,
                        new ContainerStopParameters
                        {
                            // TODO:
                        }
                    );
                }
                else if (type == "start")
                {
                    result = await client.Containers.StartContainerAsync(
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
            });
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
            _monitorThread.TryAdd($"{type}_{id}", cancellationTokenSource);

            await GetClientAsync(async client =>
            {
                try
                {
                    if (type == "stats")
                    {
                        var progress = new Progress<ContainerStatsResponse>();
                        progress.ProgressChanged += async (obj, message) =>
                        {
                            await _hub.Clients.Client(ConnectionId).SendCoreAsync("monitor", new object[] { type, message });
                        };
                        await client.Containers.GetContainerStatsAsync(
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
                        progress.ProgressChanged += async (obj, message) =>
                        {
                            await _hub.Clients.Client(ConnectionId).SendCoreAsync("monitor", new object[] { type, message });
                        };
                        await client.Containers.GetContainerLogsAsync(
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
                    await _hub.Clients.Client(ConnectionId).SendCoreAsync("cancelMonitor", new[] { type, ex.Message });
                }
            });
        }

        /// <summary>
        /// 取消监视
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("cancel/{type}/{id}")]
        public void CancelMonitor(string type, string id)
        {
            if (_monitorThread.Remove($"{type}_{id}", out var v))
            {
                v.Cancel();
                v.Dispose();
            }
        }
    }
}