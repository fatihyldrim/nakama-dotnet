/**
 * Copyright 2020 The Nakama Authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace Nakama.Tests.Socket
{
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class WebSocketMatchmakerTest
    {
        private IClient _client;

        public WebSocketMatchmakerTest()
        {
            _client = ClientUtil.FromSettingsFile();
        }

        [Theory]
        [ClassData(typeof(WebSocketTestData))]
        public async Task ShouldJoinMatchmaker(TestAdapterFactory adapterFactory)
        {
            var session = await _client.AuthenticateCustomAsync($"{Guid.NewGuid()}");
            var socket = Nakama.Socket.From(_client, adapterFactory());

            await socket.ConnectAsync(session);
            var matchmakerTicket = await socket.AddMatchmakerAsync("*");

            Assert.NotNull(matchmakerTicket);
            Assert.NotEmpty(matchmakerTicket.Ticket);

            await socket.CloseAsync();
        }

        // flakey, needs improvement
        [Theory]
        [ClassData(typeof(WebSocketTestData))]
        public async Task ShouldJoinAndLeaveMatchmaker(TestAdapterFactory adapterFactory)
        {
            var session = await _client.AuthenticateCustomAsync($"{Guid.NewGuid()}");
            var socket = Nakama.Socket.From(_client, adapterFactory());

            await socket.ConnectAsync(session);
            var matchmakerTicket = await socket.AddMatchmakerAsync("*");

            Assert.NotNull(matchmakerTicket);
            Assert.NotEmpty(matchmakerTicket.Ticket);
            await socket.RemoveMatchmakerAsync(matchmakerTicket);

            await socket.CloseAsync();
        }
    }
}