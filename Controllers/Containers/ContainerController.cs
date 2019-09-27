using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using DockerGui.Controllers;
using Microsoft.AspNetCore.Mvc;
using src.Controllers.Containers.Dtos;

namespace src.Controllers.Containers
{
    public class ContainerController : ApiBaseController
    {
        public async Task<IList<ContainerListResponse>> GetContainerList()
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
                return list;
            });
        }

        [HttpGet("stop/{id}")]
        public async Task<SetStatusResponseDto> Stop(string id)
        {
            return await GetClientAsync(async client =>
            {
                var stoped = await client.Containers.StopContainerAsync(
                    id,
                    new ContainerStopParameters
                    {
                        // TODO:
                    }
                );
                var list = await GetContainerList();
                return new SetStatusResponseDto
                {
                    Result = stoped,
                    List = list
                };
            });
        }

        [HttpGet("start/{id}")]
        public async Task<SetStatusResponseDto> Start(string id)
        {
            return await GetClientAsync(async client =>
            {
                var started = await client.Containers.StartContainerAsync(
                    id,
                    new ContainerStartParameters
                    {
                        // TODO:
                    }
                );

                var list = await GetContainerList();
                return new SetStatusResponseDto
                {
                    Result = started,
                    List = list
                };
            });
        }
    }
}