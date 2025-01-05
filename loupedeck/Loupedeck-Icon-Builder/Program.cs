using System.IO.Compression;
using System.Reflection;
using System.Xml.Linq;

// Define directories
var font = "materialsymbolsoutlined";
var root = Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program))!.Location)!, "..", "..", "..", "..");
var icons = Path.Combine(root, "..", "symbols", "web");
var build = Path.Combine(root, "Build");

// Copy files
var info = new DirectoryInfo(icons);

var directories = info.GetDirectories("", SearchOption.TopDirectoryOnly);

foreach (var directory in directories)
{
    var svg = Path.Combine(directory.FullName, font, $"{directory.Name}_48px.svg");

    if (File.Exists(svg) == false)
        continue;

    var copy = Path.Combine(build, $"{directory.Name}.svg");

    File.Copy(svg, copy, true);
}

// Update fill

info = new DirectoryInfo(build);

var files = info.GetFiles("*.svg", SearchOption.TopDirectoryOnly);

foreach (var file in files)
{
    var document = XDocument.Load(file.FullName);

    if (document.Root == null)
        continue;

    document.Root.SetAttributeValue("fill", "#000");

    File.WriteAllText(file.FullName, document.ToString());
}

// Create ZIP file
ZipFile.CreateFromDirectory(build, Path.Combine(root, $"{font}.licons"), CompressionLevel.Optimal, false);
