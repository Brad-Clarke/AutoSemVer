using NuGet.Common;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.Loader;

namespace ASV.Core.Extensions.Nuget
{
    public sealed class NugetAssemblyLoader : IAssemblyLoader
    {
        private readonly string _packageId;
        private readonly string? _version;
        private readonly string _source;

        private readonly Dictionary<string, Assembly> _loadedAssemblies = new();

        public NugetAssemblyLoader(string packageId, string? version, string? source)
        {
            _packageId = packageId;

            if (!string.IsNullOrWhiteSpace(version))
            {
                _version = version;
            }

            if (!string.IsNullOrWhiteSpace(source))
            {
                _source = source;
            }
            else
            {
                _source = "https://api.nuget.org/v3/index.json";
            }
        }

        public Assembly LoadAssembly()
        {
            Assembly assembly = LoadAssemblyFromNugetAsync().GetAwaiter().GetResult();

            return assembly;
        }

        private Assembly? CurrentDomainOnAssemblyResolve(object? sender, ResolveEventArgs args)
        {
            _loadedAssemblies.TryGetValue(args.Name, out Assembly? assembly);

            return assembly;
        }

        private async Task<Assembly> LoadAssemblyFromNugetAsync()
        {
            SourceRepository repository = Repository.Factory.GetCoreV3(_source);

            NuGetVersion packageVersion = await GetNugetVersionAsync(repository);

            using DownloadResourceResult downloadResourceResult = await DownloadPackageAsync(repository, packageVersion);

            await using Stream stream = GetPackageStream(downloadResourceResult);

            Assembly assembly = LoadAssembly(stream);

            return assembly;
        }

        private Assembly LoadAssembly(Stream stream)
        {
            AssemblyLoadContext assemblyLoadContext = new AssemblyLoadContext(null, isCollectible: true);

            stream.Position = 0;

            return assemblyLoadContext.LoadFromStream(stream);
        }

        private Stream GetPackageStream(DownloadResourceResult downloadResourceResult)
        {
            PackageReaderBase? reader = downloadResourceResult.PackageReader;

            ZipArchive archive = new ZipArchive(downloadResourceResult.PackageStream);

            string lib = reader.GetLibItems().First().Items.First();

            ZipArchiveEntry entry = archive.GetEntry(lib)!;

            MemoryStream decompressed = new MemoryStream();

            entry.Open().CopyTo(decompressed);

            return decompressed;
        }

        private async Task<DownloadResourceResult> DownloadPackageAsync(SourceRepository repository, NuGetVersion packageVersion)
        {
            DownloadResource downloadResource = await repository.GetResourceAsync<DownloadResource>();

            DownloadResourceResult downloadResourceResult = await downloadResource.GetDownloadResourceResultAsync(
                new PackageIdentity(_packageId, packageVersion),
                new PackageDownloadContext(new SourceCacheContext()),
                globalPackagesFolder: Path.GetTempPath(), new NullLogger(), CancellationToken.None);

            if (downloadResourceResult.Status != DownloadResourceResultStatus.Available)
            {
                throw new Exception($"Download of NuGet package {_packageId} failed. DownloadResult Status: {downloadResourceResult.Status}");
            }

            return downloadResourceResult;
        }

        private async Task<NuGetVersion> GetNugetVersionAsync(SourceRepository repository)
        {
            if (string.IsNullOrWhiteSpace(_version))
            {
                return await repository.GetLatestPackageVersion(_packageId);
            }

            if (!NuGetVersion.TryParse(_version, out var parsedPackagerVersion))
            {
                throw new Exception($"invalid version {_version} for nuget package {_packageId}");
            }

            return parsedPackagerVersion;
        }
    }

    public static class NugetExtensions
    {
        public static async Task<NuGetVersion> GetLatestPackageVersion(this SourceRepository repository, string packageName, SourceCacheContext? cache = null, ILogger? logger = null, CancellationToken cancellationToken = default)
        {
            PackageMetadataResource resource = await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken);

            IEnumerable<IPackageSearchMetadata> packages = await resource.GetMetadataAsync(
                "Newtonsoft.Json",
                includePrerelease: true,
                includeUnlisted: false,
                cache,
                logger,
                cancellationToken);

            return packages.OrderByDescending(p => p.Identity.Version.Version).First().Identity.Version;
        }
    }
}
