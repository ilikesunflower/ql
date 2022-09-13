using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace CMS.Extensions.Channel
{
    public class ChannelReaderExtensions
    {
        public static async ValueTask<List<T>> ReadBatchAsync<T>(ChannelReader<T> channelReader,int batchSize, TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            var items = new List<T>(batchSize);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts.CancelAfter(timeout);
            while (true)
            {
                var token = items.Count == 0 ? cancellationToken : linkedCts.Token;
                T item;
                try
                {
                    item = await channelReader.ReadAsync(token);
                }
                catch (OperationCanceledException)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    break;
                }
                catch (ChannelClosedException)
                {
                    if (items.Count == 0) throw;
                    break;
                }
                items.Add(item);
                if (items.Count >= batchSize) break;
            }
            return items;
        }
    }
}