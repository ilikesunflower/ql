#nullable disable

using System.Text;

namespace ImageProxy.Core.Models;

public class ResizeParams
{
    public bool IsImage { get; set; }
    public bool IsPhysicalFile { get; set; }
    public string ImagePath { get; set; }
    public string Extension { get; set; }
    public bool HasParams { get; set; }
    public int W { get; set; }
    public int H { get; set; }
    public bool Autorotate { get; set; }
    public int Quality { get; set; } // 0 - 100
    public string Format { get; set; } // png, jpg, jpeg
    public string Mode { get; set; } // pad, max, crop, stretch

    public static string[] Modes = new string[] { "pad", "max", "crop", "stretch" };

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"w: {W}, ");
        sb.Append($"h: {H}, ");
        sb.Append($"quality: {Quality}, ");
        sb.Append($"format: {Format}, ");
        sb.Append($"IsPhysicalFile: {IsPhysicalFile}");
        sb.Append($"Extension: {Extension}");
        return sb.ToString();
    }
}