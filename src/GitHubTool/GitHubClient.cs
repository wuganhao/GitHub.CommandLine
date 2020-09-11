using GraphQL.Client;
using GraphQL.Common.Request;
using GraphQL.Common.Response;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WuGanhao.GitHub {
    [DebuggerDisplay("{version}")]
    public class PackageVersion {
        public string version { get; set; }
        public string id { get; set; }
    }

    public class GitHubClient
    {
        public class PackageVersions {
            public PackageVersion[] nodes { get; set; }
        }

        public class RegistryPackage
        {
            public PackageVersions versions { get; set; }
        }

        public class RegistryPackages {
            public RegistryPackage[] nodes { get; set; }
        }

        public class Repository {
            public RegistryPackages packages { get; set; }
        }
        private GraphQLClient _client = new GraphQLClient("https://api.github.com/graphql");
        private const string ACCEPT_DELETE_PACKAGE = "application/vnd.github.package-deletes-preview+json";

        public GitHubClient(string pat)
        {
            _client.DefaultRequestHeaders.Add("Authorization", $"bearer {pat}");
            _client.DefaultRequestHeaders.Add("User-Agent", "Nuget Package Cleanup");
            _client.DefaultRequestHeaders.Add("Accept", ACCEPT_DELETE_PACKAGE);
        }

        public async IAsyncEnumerable<PackageVersion> GetPackageVersions(string owner, string repository, string package, int maxCount = 100) {
            string query = $@"query {{ repository(owner: ""{owner}"", name:""{repository}"") {{
                        packages (names: ""{package}"", first: 1) {{
                            nodes {{
                                versions(last: {maxCount}) {{ nodes {{ version, id }}
                                }} }} }} }} }}";

            GraphQLResponse resp = await this._client.PostAsync(new GraphQLRequest { Query = query });
            if (resp.Errors?.Any() ?? false) {
                throw new InvalidOperationException(resp.Errors.First().Message);
            }

            Repository repo = resp.GetDataFieldAs<Repository>("repository");
            foreach(PackageVersion version in repo?.packages?.nodes?.FirstOrDefault()?.versions?.nodes) {
                yield return version;
            }
        }

        public async void DeletePackageVersion(string packageVersionId) {

            string deleteQuery =$"mutation {{ deletePackageVersion(input: {{ packageVersionId: \"{packageVersionId}\"}}) {{ success }} }}";
            GraphQLResponse resp = await _client.PostAsync(new GraphQLRequest { Query = deleteQuery });
            if (resp.Errors?.Any() ?? false) {
                throw new InvalidOperationException(resp.Errors.First().Message);
            }
        }
    }
}
