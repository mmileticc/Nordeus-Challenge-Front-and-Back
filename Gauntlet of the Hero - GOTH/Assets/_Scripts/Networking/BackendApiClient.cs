using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using GauntletOfTheHero.Backend.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine.Networking;

namespace GauntletOfTheHero.Backend.Networking
{
    public sealed class BackendApiClient
    {
        private readonly string baseUrl;
        private readonly JsonSerializerSettings serializerSettings;

        public BackendApiClient(string baseUrl = null)
        {
            this.baseUrl = string.IsNullOrWhiteSpace(baseUrl) ? "http://localhost:8080" : baseUrl.TrimEnd('/');
            StringEnumConverter enumConverter = new StringEnumConverter
            {
                NamingStrategy = new UpperCaseNamingStrategy()
            };
            serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Culture = CultureInfo.InvariantCulture,
                ContractResolver = new DefaultContractResolver()
            };
            serializerSettings.Converters.Add(enumConverter);
        }

        public Task<List<HeroDto>> GetHeroesAsync(CancellationToken cancellationToken = default)
        {
            return GetAsync<List<HeroDto>>("/api/heroes", cancellationToken);
        }

        public Task<List<MonsterDto>> GetMonstersAsync(CancellationToken cancellationToken = default)
        {
            return GetAsync<List<MonsterDto>>("/api/monsters", cancellationToken);
        }

        public Task<BattleSessionDto> CreateBattleSessionAsync(BattleSessionCreateRequestDto request, CancellationToken cancellationToken = default)
        {
            return PostAsync<BattleSessionCreateRequestDto, BattleSessionDto>("/api/battle-sessions", request, cancellationToken);
        }

        public Task<BattleSessionDto> GetBattleSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            return GetAsync<BattleSessionDto>($"/api/battle-sessions/{sessionId}", cancellationToken);
        }

        public Task<BattleTurnResultDto> PlaySessionTurnAsync(Guid sessionId, BattleSessionTurnRequestDto request, CancellationToken cancellationToken = default)
        {
            return PostAsync<BattleSessionTurnRequestDto, BattleTurnResultDto>($"/api/battle-sessions/{sessionId}/turn", request, cancellationToken);
        }

        public Task DeleteBattleSessionAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            return DeleteAsync($"/api/battle-sessions/{sessionId}", cancellationToken);
        }

        public Task<BattleTurnResultDto> ExecuteStatelessTurnAsync(BattleStateDto battleState, CancellationToken cancellationToken = default)
        {
            return PostAsync<BattleStateDto, BattleTurnResultDto>("/api/battle/turn", battleState, cancellationToken);
        }

        public Task<MoveDto> GetMonsterNextMoveAsync(long monsterId, int currentHealth, CancellationToken cancellationToken = default)
        {
            return GetAsync<MoveDto>($"/api/battle/monster-next-move?monsterId={monsterId}&currentHealth={currentHealth}", cancellationToken);
        }

        public Task<RunConfigDto> StartRunAsync(RunStartRequestDto request = null, CancellationToken cancellationToken = default)
        {
            return PostAsync<RunStartRequestDto, RunConfigDto>("/api/runs/start", request ?? new RunStartRequestDto(), cancellationToken);
        }

        public Task<RunConfigDto> GetRunConfigAsync(Guid runId, CancellationToken cancellationToken = default)
        {
            return GetAsync<RunConfigDto>($"/api/runs/{runId}/config", cancellationToken);
        }

        public Task<RunConfigDto> EquipMovesAsync(Guid runId, RunEquipMovesRequestDto request, CancellationToken cancellationToken = default)
        {
            return PutAsync<RunEquipMovesRequestDto, RunConfigDto>($"/api/runs/{runId}/moves/equipped", request, cancellationToken);
        }

        public Task<RunStartEncounterResponseDto> StartEncounterAsync(Guid runId, int encounterIndex, CancellationToken cancellationToken = default)
        {
            return PostAsync<object, RunStartEncounterResponseDto>($"/api/runs/{runId}/encounters/{encounterIndex}/start", new { }, cancellationToken);
        }

        public Task<RunResolveEncounterResponseDto> ResolveEncounterAsync(Guid runId, int encounterIndex, RunResolveEncounterRequestDto request, CancellationToken cancellationToken = default)
        {
            return PostAsync<RunResolveEncounterRequestDto, RunResolveEncounterResponseDto>($"/api/runs/{runId}/encounters/{encounterIndex}/resolve", request, cancellationToken);
        }

        private async Task<T> GetAsync<T>(string path, CancellationToken cancellationToken)
        {
            using UnityWebRequest request = CreateRequest(path, UnityWebRequest.kHttpVerbGET);
            await SendAsync(request, cancellationToken);
            return HandleJsonResponse<T>(request);
        }

        private async Task<TResponse> PostAsync<TRequest, TResponse>(string path, TRequest payload, CancellationToken cancellationToken)
        {
            using UnityWebRequest request = CreateRequest(path, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, serializerSettings)));
            request.uploadHandler.contentType = "application/json";

            await SendAsync(request, cancellationToken);
            return HandleJsonResponse<TResponse>(request);
        }

        private async Task<TResponse> PutAsync<TRequest, TResponse>(string path, TRequest payload, CancellationToken cancellationToken)
        {
            using UnityWebRequest request = CreateRequest(path, UnityWebRequest.kHttpVerbPUT);
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload, serializerSettings)));
            request.uploadHandler.contentType = "application/json";

            await SendAsync(request, cancellationToken);
            return HandleJsonResponse<TResponse>(request);
        }

        private async Task DeleteAsync(string path, CancellationToken cancellationToken)
        {
            using UnityWebRequest request = CreateRequest(path, UnityWebRequest.kHttpVerbDELETE);
            await SendAsync(request, cancellationToken);

            if (!IsSuccessStatusCode(request.responseCode))
            {
                ThrowBackendException(request);
            }
        }

        private UnityWebRequest CreateRequest(string path, string method)
        {
            UnityWebRequest request = new UnityWebRequest(BuildUrl(path), method)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };

            request.SetRequestHeader("Accept", "application/json");
            return request;
        }

        private string BuildUrl(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return baseUrl;
            }

            return path.StartsWith("/") ? baseUrl + path : baseUrl + "/" + path;
        }

        private async Task SendAsync(UnityWebRequest request, CancellationToken cancellationToken)
        {
            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (!IsSuccessStatusCode(request.responseCode))
            {
                ThrowBackendException(request);
            }
        }

        private T HandleJsonResponse<T>(UnityWebRequest request)
        {
            string body = request.downloadHandler?.text;
            if (string.IsNullOrWhiteSpace(body))
            {
                return default;
            }

            return JsonConvert.DeserializeObject<T>(body, serializerSettings);
        }

        private void ThrowBackendException(UnityWebRequest request)
        {
            string body = request.downloadHandler?.text;
            BackendErrorResponseDto error = null;

            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    error = JsonConvert.DeserializeObject<BackendErrorResponseDto>(body, serializerSettings);
                }
                catch (JsonException)
                {
                    error = null;
                }
            }

            throw new BackendApiException(request.responseCode, body, error);
        }

        private static bool IsSuccessStatusCode(long statusCode)
        {
            return statusCode >= 200 && statusCode <= 299;
        }
    }
}