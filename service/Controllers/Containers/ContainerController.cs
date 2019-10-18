using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
using src.Repositories;

namespace src.Controllers.Containers
{
    public class ContainerController : ApiBaseController
    {
        private readonly IHubContext<BaseHub> _hub;
        private readonly ILogger<ContainerController> _log;

        public ContainerController(
            IHubContext<BaseHub> hub,
            ILogger<ContainerController> log
        ) : base(hub, log)
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
            Repository.MONITOR_THREAD.TryAdd($"{type}_{Token}", cancellationTokenSource);

            await GetClientAsync(async client =>
            {
                try
                {
                    if (type == "stats")
                    {
                        var progress = new Progress<ContainerStatsResponse>();
                        progress.ProgressChanged += async (obj, message) =>
                        {
                            await _hub.Clients.Group(Token).SendAsync("monitor", type, message);
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
                    _log.LogWarning(ex, "退出监控");
                    await _hub.Clients.Group(Token).SendAsync("notification", "info", "The normal exit");
                }
            });
        }

        /// <summary>
        /// 取消监视
        /// </summary>
        /// <param name="id"></param>
        [HttpGet("cancel/{type}")]
        public void CancelMonitor(string type)
        {
            if (Repository.MONITOR_THREAD.Remove($"{type}_{Token}", out var v))
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