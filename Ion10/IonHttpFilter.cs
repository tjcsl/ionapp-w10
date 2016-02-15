// Copyright (c) 2015, Ion Native App Team
// All rights reserved.

using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Windows.Web.Http.Headers;

namespace Ion10 {
    public sealed class IonHttpFilter : IHttpFilter {
        private readonly Uri baseUri;
        private readonly HttpBaseProtocolFilter baseFilter;
        private readonly HttpCredentialsHeaderValue credential;

        public IonHttpFilter(string username, string password, Uri baseUri) {
            this.baseUri = baseUri;
            baseFilter = new HttpBaseProtocolFilter() {
                AllowUI = false
            };
            var buffer = Windows.Security.Cryptography.CryptographicBuffer.ConvertStringToBinary(username + ":" + password,
                Windows.Security.Cryptography.BinaryStringEncoding.Utf16LE);
            var base64Token = Windows.Security.Cryptography.CryptographicBuffer.EncodeToBase64String(buffer);
            credential = new HttpCredentialsHeaderValue("Basic", base64Token);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            baseFilter.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Send an HTTP request on the IHttpFilter instance as an asynchronous operation.
        /// </summary>
        /// <returns>
        /// The object representing the asynchronous operation.
        /// </returns>
        /// <param name="request">The HTTP request message to send.</param>
        public IAsyncOperationWithProgress<HttpResponseMessage, HttpProgress> SendRequestAsync(HttpRequestMessage request) {
            return AsyncInfo.Run<HttpResponseMessage, HttpProgress>(async (cancellationToken, progress) => {
                request.RequestUri = new Uri(baseUri, request.RequestUri);
                request.Headers.Authorization = credential;
                var response = await baseFilter.SendRequestAsync(request).AsTask(cancellationToken, progress);
                cancellationToken.ThrowIfCancellationRequested();
                return response;
            });
        }
    }
}
