﻿/**
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Nakama.Tests.Api
{
    public class AuthenticateTest
    {
        private IClient _client;

        public AuthenticateTest()
        {
            _client = new Client("http", "127.0.0.1", 7350, "defaultkey");
        }

        [Fact]
        public async Task ShouldAuthenticateCustomId()
        {
            var customid = Guid.NewGuid();
            var session = await _client.AuthenticateCustomAsync(customid.ToString());

            Assert.NotNull(session);
            Assert.NotNull(session.UserId);
            Assert.NotNull(session.Username);
            Assert.False(session.IsExpired);
        }

        [Fact]
        public async Task ShouldAuthenticateDeviceId()
        {
            var deviceid = Guid.NewGuid().ToString();
            var session = await _client.AuthenticateDeviceAsync(deviceid);

            Assert.NotNull(session);
            Assert.NotNull(session.UserId);
            Assert.NotNull(session.Username);
            Assert.False(session.IsExpired);

            var account = await _client.GetAccountAsync(session);

            Assert.NotNull(account);
            Assert.Equal(account.Devices.Count(d => d.Id == deviceid), 1);
        }

        [Fact]
        public async Task ShouldAuthenticateEmail()
        {
            var session = await _client.AuthenticateEmailAsync("super@heroes.com", "batsignal");

            Assert.NotNull(session);
            Assert.NotNull(session.UserId);
            Assert.NotNull(session.Username);
            Assert.False(session.IsExpired);
        }

        [Fact]
        public async void ShouldNotAuthenticateFacebook()
        {
            var ex = await Assert.ThrowsAsync<ApiResponseException>(() => _client.AuthenticateFacebookAsync("invalid"));
            Assert.Equal((int) HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        [Fact]
        public async void ShouldNotAuthenticateGameCenter()
        {
            var bundleId = string.Empty;
            var playerId = string.Empty;
            var publicKeyUrl = string.Empty;
            var salt = string.Empty;
            var signature = string.Empty;
            var timestamp = string.Empty;

            var ex = await Assert.ThrowsAsync<ApiResponseException>(() =>
                _client.AuthenticateGameCenterAsync(bundleId, playerId, publicKeyUrl, salt, signature, timestamp));

            Assert.Equal((int) HttpStatusCode.BadRequest, ex.StatusCode);
        }

        [Fact]
        public async void ShouldNotAuthenticateGoogle()
        {
            var ex = await Assert.ThrowsAsync<ApiResponseException>(() => _client.AuthenticateGoogleAsync("invalid"));
            Assert.Equal((int) HttpStatusCode.Unauthorized, ex.StatusCode);
        }

        [Fact]
        public async void ShouldNotAuthenticateSteam()
        {
            var ex = await Assert.ThrowsAsync<ApiResponseException>(() => _client.AuthenticateSteamAsync("invalid"));

            // Precondition failed because Steam requires special configuration with the server.
            // Maps to 400, because gRPC precondition failed != HTTP precondition failed.
            Assert.Equal((int) HttpStatusCode.BadRequest, ex.StatusCode);
        }

        [Fact]
        public async void ShouldNotAuthenticateApple()
        {
            // Fails because Apple requires special configuration with the server.
            var ex = await Assert.ThrowsAsync<ApiResponseException>(() => _client.AuthenticateAppleAsync("some_username", "some_token", new Dictionary<string, string>()));
            Assert.Equal((int) HttpStatusCode.Unauthorized, ex.StatusCode);
        }
    }
}