using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace ImageProxy.Core.Static;

internal static class RangeHelper
{
    public static (bool isRangeRequest, RangeItemHeaderValue range) ParseRange(
        HttpContext context,
        RequestHeaders requestHeaders,
        long length,
        ILogger logger)
    {
        var rawRangeHeader = context.Request.Headers[HeaderNames.Range];
        if (StringValues.IsNullOrEmpty(rawRangeHeader))
        {
            logger.LogTrace("Range header's value is empty.");
            return (false, null);
        }

        // Perf: Check for a single entry before parsing it
        if (rawRangeHeader.Count > 1 || rawRangeHeader[0].IndexOf(',') >= 0)
        {
            logger.LogDebug("Multiple ranges are not supported.");

            // The spec allows for multiple ranges but we choose not to support them because the client may request
            // very strange ranges (e.g. each byte separately, overlapping ranges, etc.) that could negatively
            // impact the server. Ignore the header and serve the response normally.               
            return (false, null);
        }

        var rangeHeader = requestHeaders.Range;
        if (rangeHeader == null)
        {
            logger.LogDebug("Range header's value is invalid.");
            // Invalid
            return (false, null);
        }

        // Already verified above
        Debug.Assert(rangeHeader.Ranges.Count == 1);

        var ranges = rangeHeader.Ranges;
        if (ranges == null)
        {
            logger.LogDebug("Range header's value is invalid.");
            return (false, null);
        }

        if (ranges.Count == 0)
        {
            return (true, null);
        }

        if (length == 0)
        {
            return (true, null);
        }

        // Normalize the ranges
        var range = NormalizeRange(ranges.SingleOrDefault(), length);

        // Return the single range
        return (true, range);
    }

    // Internal for testing
    internal static RangeItemHeaderValue NormalizeRange(RangeItemHeaderValue range, long length)
    {
        var start = range.From;
        var end = range.To;

        // X-[Y]
        if (start.HasValue)
        {
            if (start.Value >= length)
            {
                // Not satisfiable, skip/discard.
                return null;
            }

            if (!end.HasValue || end.Value >= length)
            {
                end = length - 1;
            }
        }
        else
        {
            // suffix range "-X" e.g. the last X bytes, resolve
            if (end.Value == 0)
            {
                // Not satisfiable, skip/discard.
                return null;
            }

            var bytes = Math.Min(end.Value, length);
            start = length - bytes;
            end = start + bytes - 1;
        }

        return new RangeItemHeaderValue(start, end);
    }
}