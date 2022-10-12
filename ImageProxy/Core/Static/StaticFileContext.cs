using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace ImageProxy.Core.Static;

internal struct StaticFileContext
{
    private readonly HttpContext _context;
    private readonly StaticFileOptions _options;
    private readonly PathString _matchUrl;
    private readonly HttpRequest _request;
    private readonly HttpResponse _response;
    private readonly ILogger _logger;
    private readonly IFileProvider _fileProvider;
    private readonly IContentTypeProvider _contentTypeProvider;
    private string? _method;
    private bool _isGet;
    private bool _isHead;
    private PathString _subPath;
    private string? _contentType;
    private IFileInfo? _fileInfo;
    private long _length;
    private DateTimeOffset _lastModified;
    private EntityTagHeaderValue? _etag;

    private RequestHeaders _requestHeaders;
    private ResponseHeaders _responseHeaders;

    private PreconditionState _ifMatchState;
    private PreconditionState _ifNoneMatchState;
    private PreconditionState _ifModifiedSinceState;
    private PreconditionState _ifUnmodifiedSinceState;

    private RangeItemHeaderValue? _range;
    private bool _isRangeRequest;

    public StaticFileContext(HttpContext context, StaticFileOptions options, PathString matchUrl, ILogger logger,
        IFileProvider fileProvider, IContentTypeProvider contentTypeProvider)
    {
        _context = context;
        _options = options;
        _matchUrl = matchUrl;
        _request = context.Request;
        _response = context.Response;
        _logger = logger;
        _requestHeaders = _request.GetTypedHeaders();
        _responseHeaders = _response.GetTypedHeaders();
        _fileProvider = fileProvider;
        _contentTypeProvider = contentTypeProvider;

        _method = null;
        _isGet = false;
        _isHead = false;
        _subPath = PathString.Empty;
        _contentType = null;
        _fileInfo = null;
        _length = 0;
        _lastModified = new DateTimeOffset();
        _etag = null;
        _ifMatchState = PreconditionState.Unspecified;
        _ifNoneMatchState = PreconditionState.Unspecified;
        _ifModifiedSinceState = PreconditionState.Unspecified;
        _ifUnmodifiedSinceState = PreconditionState.Unspecified;
        _range = null;
        _isRangeRequest = false;
    }

    internal enum PreconditionState
    {
        Unspecified,
        NotModified,
        ShouldProcess,
        PreconditionFailed
    }

    public bool IsHeadMethod
    {
        get { return _isHead; }
    }

    public bool IsRangeRequest
    {
        get { return _isRangeRequest; }
    }

    public string SubPath
    {
        get { return _subPath.Value; }
    }

    public string PhysicalPath
    {
        get { return _fileInfo?.PhysicalPath; }
    }

    public bool ValidateMethod()
    {
        _method = _request.Method;
        _isGet = HttpMethods.IsGet(_method);
        _isHead = HttpMethods.IsHead(_method);
        return _isGet || _isHead;
    }

    // Check if the URL matches any expected paths
    public bool ValidatePath()
    {
        return Helpers.TryMatchPath(_context, _matchUrl, forDirectory: false, subpath: out _subPath);
    }

    public bool LookupContentType()
    {
        if (_contentTypeProvider.TryGetContentType(_subPath.Value, out _contentType))
        {
            return true;
        }

        if (_options.ServeUnknownFileTypes)
        {
            _contentType = _options.DefaultContentType;
            return true;
        }

        return false;
    }

    public bool LookupFileInfo()
    {
        _fileInfo = _fileProvider.GetFileInfo(_subPath.Value);
        if (_fileInfo.Exists)
        {
            _length = _fileInfo.Length;

            DateTimeOffset last = _fileInfo.LastModified;
            // Truncate to the second.
            _lastModified =
                new DateTimeOffset(last.Year, last.Month, last.Day, last.Hour, last.Minute, last.Second, last.Offset)
                    .ToUniversalTime();

            long etagHash = _lastModified.ToFileTime() ^ _length;
            _etag = new EntityTagHeaderValue('\"' + Convert.ToString(etagHash, 16) + '\"');
        }

        return _fileInfo.Exists;
    }

    public void ComprehendRequestHeaders()
    {
        ComputeIfMatch();

        ComputeIfModifiedSince();

        ComputeRange();

        ComputeIfRange();
    }

    private void ComputeIfMatch()
    {
        // 14.24 If-Match
        var ifMatch = _requestHeaders.IfMatch;
        if (ifMatch != null && ifMatch.Any())
        {
            _ifMatchState = PreconditionState.PreconditionFailed;
            foreach (var etag in ifMatch)
            {
                if (etag.Equals(EntityTagHeaderValue.Any) || etag.Compare(_etag, useStrongComparison: true))
                {
                    _ifMatchState = PreconditionState.ShouldProcess;
                    break;
                }
            }
        }

        // 14.26 If-None-Match
        var ifNoneMatch = _requestHeaders.IfNoneMatch;
        if (ifNoneMatch != null && ifNoneMatch.Any())
        {
            _ifNoneMatchState = PreconditionState.ShouldProcess;
            foreach (var etag in ifNoneMatch)
            {
                if (etag.Equals(EntityTagHeaderValue.Any) || etag.Compare(_etag, useStrongComparison: true))
                {
                    _ifNoneMatchState = PreconditionState.NotModified;
                    break;
                }
            }
        }
    }

    private void ComputeIfModifiedSince()
    {
        var now = DateTimeOffset.UtcNow;

        // 14.25 If-Modified-Since
        var ifModifiedSince = _requestHeaders.IfModifiedSince;
        if (ifModifiedSince.HasValue && ifModifiedSince <= now)
        {
            bool modified = ifModifiedSince < _lastModified;
            _ifModifiedSinceState = modified ? PreconditionState.ShouldProcess : PreconditionState.NotModified;
        }

        // 14.28 If-Unmodified-Since
        var ifUnmodifiedSince = _requestHeaders.IfUnmodifiedSince;
        if (ifUnmodifiedSince.HasValue && ifUnmodifiedSince <= now)
        {
            bool unmodified = ifUnmodifiedSince >= _lastModified;
            _ifUnmodifiedSinceState =
                unmodified ? PreconditionState.ShouldProcess : PreconditionState.PreconditionFailed;
        }
    }

    private void ComputeIfRange()
    {
        // 14.27 If-Range
        var ifRangeHeader = _requestHeaders.IfRange;
        if (ifRangeHeader != null)
        {
            // If the validator given in the If-Range header field matches the
            // current validator for the selected representation of the target
            // resource, then the server SHOULD process the Range header field as
            // requested.  If the validator does not match, the server MUST ignore
            // the Range header field.
            if (ifRangeHeader.LastModified.HasValue)
            {
                if (_lastModified != null && _lastModified > ifRangeHeader.LastModified)
                {
                    _isRangeRequest = false;
                }
            }
            else if (_etag != null && ifRangeHeader.EntityTag != null &&
                     !ifRangeHeader.EntityTag.Compare(_etag, useStrongComparison: true))
            {
                _isRangeRequest = false;
            }
        }
    }

    private void ComputeRange()
    {
        // 14.35 Range
        // http://tools.ietf.org/html/draft-ietf-httpbis-p5-range-24

        // A server MUST ignore a Range header field received with a request method other
        // than GET.
        if (!_isGet)
        {
            return;
        }

        (_isRangeRequest, _range) = RangeHelper.ParseRange(_context, _requestHeaders, _length, _logger);
    }


    public PreconditionState GetPreconditionState()
    {
        return GetMaxPreconditionState(_ifMatchState, _ifNoneMatchState,
            _ifModifiedSinceState, _ifUnmodifiedSinceState);
    }

    private static PreconditionState GetMaxPreconditionState(params PreconditionState[] states)
    {
        PreconditionState max = PreconditionState.Unspecified;
        for (int i = 0; i < states.Length; i++)
        {
            if (states[i] > max)
            {
                max = states[i];
            }
        }

        return max;
    }


    private ContentRangeHeaderValue ComputeContentRange(RangeItemHeaderValue range, out long start, out long length)
    {
        start = range.From.Value;
        long end = range.To.Value;
        length = end - start + 1;
        return new ContentRangeHeaderValue(start, end, _length);
    }
}