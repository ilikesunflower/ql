namespace ImageProxy.Core.Models;

public class ImageProcessResult
{
    public byte[] ImageData { get; set; } = null!;

    public string ContentType { get; set; } = null!;
}