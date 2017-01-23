using System;
using System.Linq;
using System.Threading;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

namespace NugetTest
{
    public class NugetAdaptor
    {
        private readonly SourceRepository _repository;

        public NugetAdaptor(string url, string username, string password)
        {
            var source = new PackageSource(url, "my-feed");
            if (!string.IsNullOrEmpty(username))
            {
                source.Credentials = new PackageSourceCredential(source.Name, username, password, isPasswordClearText: true);
            }
            _repository = new SourceRepository(source, Repository.Provider.GetCoreV3());
        }

        public void CreateReleaseTest(string packageId)
        {
            var task = _repository
                .GetResource<PackageMetadataResource>(CancellationToken.None)
                .GetMetadataAsync(packageId, false, false, NullLogger.Instance, CancellationToken.None);

            task.Wait();

            Console.WriteLine("{0} results found", task.Result.Count());
        }
    }
}