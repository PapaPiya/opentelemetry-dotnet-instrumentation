// <copyright file="StatusServiceClient.cs" company="OpenTelemetry Authors">
// Copyright The OpenTelemetry Authors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;

namespace TestApplication.Wcf.Client.NetFramework;

public class StatusServiceClient : ClientBase<IStatusServiceContract>, IStatusServiceContract
{
    public StatusServiceClient(Binding binding, EndpointAddress remoteAddress)
        : base(binding, remoteAddress)
    {
    }

    public Task<StatusResponse> PingAsync(StatusRequest request)
    {
        return this.Channel.PingAsync(request);
    }

    public StatusResponse PingSync(StatusRequest request)
    {
        return Channel.PingSync(request);
    }

    public IAsyncResult BeginPing(StatusRequest request, AsyncCallback callback, object asyncState)
    {
        return Channel.BeginPing(request, callback, asyncState);
    }

    public StatusResponse EndPing(IAsyncResult asyncResult)
    {
        return Channel.EndPing(asyncResult);
    }

    public Task OpenAsync()
    {
        ICommunicationObject communicationObject = this;
        return Task.Factory.FromAsync(communicationObject.BeginOpen, communicationObject.EndOpen, null);
    }

    public Task CloseAsync()
    {
        ICommunicationObject communicationObject = this;
        return Task.Factory.FromAsync(communicationObject.BeginClose, communicationObject.EndClose, null);
    }
}
