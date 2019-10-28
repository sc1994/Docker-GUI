using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerGui.Controllers.Containers.Dtos;
using Newtonsoft.Json;

namespace DockerGui.Cores.Containers
{
    public class ContainerCore : IContainerCore
    {
        private static IList<ContainerListResponseDto> containerList = null;

        public async Task<IList<ContainerListResponseDto>> GetContainerListAsync(DockerClient client, bool refresh = false)
        {
            if (containerList != null && !refresh) return await Task.FromResult(containerList);

            var list = await client.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true,
                Size = true
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

            return containerList = list.Select(x =>
            {
                var r = JsonConvert.DeserializeObject<ContainerListResponseDto>(JsonConvert.SerializeObject(x));
                r.CreatedStr = x.Created.ToString("yyyy-MM-dd HH:mm");
                return r;
            }).ToList();
        }
    }
}
