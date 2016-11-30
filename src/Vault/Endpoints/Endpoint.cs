﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Vault.Models;

namespace Vault.Endpoints
{
    public class Endpoint : IEndpoint
    {
        private readonly VaultClient _client;
        private readonly string _uriBasePath;

        private const string UriRootPath = "/v1";
        private const string WrappedResponseLocation = "cubbyhole/response";


        public Endpoint(VaultClient client, string basePath = null)
        {
            _client = client;

            var path = basePath != null ? $"/{basePath}" : "";
            _uriBasePath = $"{UriRootPath}{path}";

        }

        public Task<VaultResponse<TData>> Read<TData>(string path, CancellationToken ct = default(CancellationToken))
        {
            return _client.Get<VaultResponse<TData>>($"{_uriBasePath}/{path}", ct);
        }

        private Task<VaultResponse<TData>> Read<TData>(string path, string token, CancellationToken ct = default(CancellationToken))
        {
            return _client.Get<VaultResponse<TData>>($"{_uriBasePath}/{path}", token, ct);
        }

        public Task<WrappedVaultResponse> Read(string path, TimeSpan wrapTtl, CancellationToken ct = default(CancellationToken))
        {
            return _client.Get<WrappedVaultResponse>($"{_uriBasePath}/{path}", wrapTtl, ct);
        }
 
        public Task<VaultResponse<TData>> List<TData>(string path, CancellationToken ct = default(CancellationToken))
        {
            return _client.List<VaultResponse<TData>>($"{_uriBasePath}/{path}", ct);
        }

        public Task Write<TParameters>(string path, TParameters data, CancellationToken ct = default(CancellationToken))
        {
            return _client.PutVoid($"{_uriBasePath}/{path}", data, ct);
        }

        public Task<VaultResponse<TData>> Write<TData>(string path, CancellationToken ct = default(CancellationToken))
        {
            return _client.Put<VaultResponse<TData>>($"{_uriBasePath}/{path}", ct);
        }

        public Task<VaultResponse<TData>> Write<TParameters, TData>(string path, TParameters data, CancellationToken ct = default(CancellationToken))
        {
            return _client.Put<TParameters, VaultResponse<TData>>($"{_uriBasePath}/{path}", data, ct);
        }

        public Task Delete(string path, CancellationToken ct = default(CancellationToken))
        {
            return _client.DeleteVoid($"{_uriBasePath}/{path}", ct);
        }

        public async Task<VaultResponse<TData>> Unwrap<TData>(string unwrappingToken, CancellationToken ct = default(CancellationToken))
        {
            var wrappedSecret = await Read<WrappedSecretData>(WrappedResponseLocation, unwrappingToken, ct).ConfigureAwait(false);
            return await Task.Run(() => JsonConvert.DeserializeObject<VaultResponse<TData>>(wrappedSecret.Data.Response), ct).ConfigureAwait(false);
        }

        internal class WrappedSecretData
        {
            public string Response { get; set; }
        }
    }
}