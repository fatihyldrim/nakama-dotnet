﻿/**
 * Copyright 2018 The Nakama Authors
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

using System.Net.Http;

namespace Nakama.Tests.Api
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;

    [TestFixture]
    public class ChannelTest
    {
        private IClient _client;

        // ReSharper disable RedundantArgumentDefaultValue
        [SetUp]
        public void SetUp()
        {
            _client = new Client("defaultkey", "127.0.0.1", 7350, false);
        }

        [Test]
        public async Task ShouldListMessagesInvalid()
        {
            var session = await _client.AuthenticateCustomAsync($"{Guid.NewGuid()}");

            var ex = Assert.ThrowsAsync<HttpRequestException>(() =>
                _client.ListChannelMessagesAsync(session, "roomname", 100, true));
            Assert.NotNull("400 (Bad Request)", ex.Message);
        }
    }
}