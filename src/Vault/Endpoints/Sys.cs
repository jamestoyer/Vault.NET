﻿using System.Threading;
using System.Threading.Tasks;

namespace Vault.Endpoints
{
    public class Sys : ISysEndpoint
    {
        private readonly VaultClient _client;
        private const string UriPathBase = "/v1/sys";

        public Sys(VaultClient client)
        {
            _client = client;
        }

        public class SysInitResponse
        {
            public bool Initialized { get; set; }
        }

        public Task<SysInitResponse> ReadInit()
        {
            return _client.Get<SysInitResponse>($"{UriPathBase}/init", null, CancellationToken.None);
        }
    }
}