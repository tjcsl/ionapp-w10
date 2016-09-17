using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.UI.Popups;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ion10.Services {
    public sealed class OAuthSession : IDisposable {
        private readonly Uri baseUri;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly Uri callbackUri;
        private readonly HttpClient httpClient;

        public OAuthSession(Uri baseUri, string clientId, string clientSecret, Uri callbackUri) {
            this.baseUri = baseUri;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.callbackUri = callbackUri;
            httpClient = new HttpClient();
        }

        public async Task<string> GetOAuthCodeAsync() {
            var oauthUri = new Uri(baseUri, string.Format(
                "/oauth/authorize/?client_id={0}&redirect_uri={1}&response_type=code&scope=read write",
                clientId,
                Uri.EscapeUriString(callbackUri.ToString())));
            var result = await WebAuthenticationBroker.AuthenticateAsync(
                WebAuthenticationOptions.None,
                oauthUri,
                callbackUri);
            
            var part = result.ResponseData.Replace(callbackUri + "?", "");
            if(!part.StartsWith("code=")) {
                return null;
            }
            return part.Replace("code=", "");
        }

        public async Task<OAuthToken> GetOAuthTokenAsync(string oauthCode) {
            var oauthUri = new Uri(baseUri, "/oauth/token/");
            var req = new HttpRequestMessage(HttpMethod.Post, oauthUri);
            req.Content = new HttpFormUrlEncodedContent(
                new Dictionary<string, string> {
                    {"code", oauthCode},
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                    {"redirect_uri", callbackUri.ToString()},
                    {"grant_type", "authorization_code"}
                });
            var result = await httpClient.SendRequestAsync(req);
            var content = await result.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            return new OAuthToken(
                (string)json["refresh_token"],
                (string)json["access_token"],
                DateTime.UtcNow.AddSeconds((double)json["expires_in"])
                );
        }

        public async Task<OAuthToken> RefreshOAuthTokenAsync(string oauthToken) {
            var oauthUri = new Uri(baseUri, "/oauth/token/");
            var req = new HttpRequestMessage(HttpMethod.Post, oauthUri);
            req.Content = new HttpFormUrlEncodedContent(
                new Dictionary<string, string> {
                    {"refresh_token", oauthToken},
                    {"client_id", clientId},
                    {"client_secret", clientSecret},
                    {"redirect_uri", callbackUri.ToString()},
                    {"grant_type", "refresh_token"}
                });
            var result = await httpClient.SendRequestAsync(req);
            var content = await result.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            return new OAuthToken(
                (string)json["refresh_token"],
                (string)json["access_token"],
                DateTime.UtcNow.AddSeconds((double)json["expires_in"])
                );
        }

        public async Task<HttpResponseMessage> SendAsync(string uri, HttpRequestMessage request, OAuthToken token) {
            request.Headers.Authorization = new HttpCredentialsHeaderValue("Bearer", token.AccessToken);
            request.RequestUri = new Uri(baseUri, uri);
            return await httpClient.SendRequestAsync(request);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }
    }

    public struct OAuthToken {
        public string RefreshToken { get; private set; }
        public string AccessToken { get; private set; }
        public DateTime ExpireTime { get; private set; }

        public OAuthToken(string refreshToken, string accessToken, DateTime expireTime) {
            RefreshToken = refreshToken;
            AccessToken = accessToken;
            ExpireTime = expireTime;
        }
    }
}
