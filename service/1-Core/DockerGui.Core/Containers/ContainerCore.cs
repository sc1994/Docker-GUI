using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using DockerGui.Tools.Values;

namespace DockerGui.Core.Containers
{
    public class ContainerCore : IContainerCore
    {
        public async Task<IList<ContainerListResponse>> GetContainerListAsync(DockerClient client, bool refresh = false)
        {
            if (StaticValue.CONTAINERS.Count > 0
             && !refresh)
                return await Task.FromResult(StaticValue.CONTAINERS);

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
            StaticValue.CONTAINERS.Clear();
            foreach (var item in list)
            {
                item.ID = item.ID.Substring(0, 8);
                StaticValue.CONTAINERS.Add(item);
            }

            return list;
        }
    }
}
