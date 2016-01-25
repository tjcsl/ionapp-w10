using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.Web.Http;

namespace Ion10 {
    public sealed class EighthSignupCollection : ObservableCollection<IGrouping<DateTime, EighthSignup>>, ISupportIncrementalLoading {
        /// <summary>
        /// Initializes incremental loading from the view.
        /// </summary>
        /// <returns>
        /// The wrapped results of the load operation.
        /// </returns>
        /// <param name="count">The number of items to load.</param>
        public IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count) {
            return AsyncInfo.Run(c => LoadMoreItemsAsync(c, count));
        }

        private async Task<LoadMoreItemsResult> LoadMoreItemsAsync(CancellationToken c, uint count) {
            var baseIndex = Count;
            var lastDate = this[Count - 1].Key;
            var signups = (await EighthSignup.GetAsync(lastDate.AddDays(1), (int)count)).GroupBy(s => s.Date);
            uint loaded = 0;
            foreach(var signup in signups) {
                Add(signup);
                loaded++;
            }
            return new LoadMoreItemsResult {Count = loaded};
        }

        /// <summary>
        /// Gets a sentinel value that supports incremental loading implementations.
        /// </summary>
        /// <returns>
        /// true if additional unloaded items remain in the view; otherwise, false.
        /// </returns>
        public bool HasMoreItems { get; }
    }
}
