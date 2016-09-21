// The MIT License (MIT) 
// 
// Copyright (c) 2016 Ion Native App Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Ion10.Services {
    public sealed class OAuthSession : IDisposable {
        private readonly Uri baseUri;
        private readonly Uri callbackUri;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly HttpClient httpClient;

        public OAuthSession(Uri baseUri, string clientId, string clientSecret, Uri callbackUri) {
            this.baseUri = baseUri;
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.callbackUri = callbackUri;
            httpClient = new HttpClient();
        }

        public void Dispose() {
            httpClient.Dispose();
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

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, OAuthToken token) {
            request.Headers.Authorization = new HttpCredentialsHeaderValue("Bearer", token.AccessToken);
            return await httpClient.SendRequestAsync(request);
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
