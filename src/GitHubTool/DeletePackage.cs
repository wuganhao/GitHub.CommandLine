using WuGanhao.CommandLineParser;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace WuGanhao.GitHub {
    public enum DeleteMode {
        Current,
        All,
    }

    public class DeletePackage: SubCommand {
        [Required]
        [CommandOption("token", "t", "GitHub access token")]
        public string Token { get; set; }

        [CommandOption("owner", "o", "GitHub Repository owner")]
        public string Owner { get; set; }

        [CommandOption("repository", "r", "GitHub Repository")]
        public string Repository { get; set; }

        [Required]
        [CommandOption("package", "p", "GitHub Package Name")]
        public string Package { get; set; }

        [CommandOption("version-to-keep", "k", "Latest versions to keep. default to 5")]
        public int VersionToKeep { get; set; } = 5;

        [CommandOption("mode", "m", "Package deletion mode. default to current")]
        public DeleteMode Mode { get; set; } = DeleteMode.Current;

        [CommandOption("version", "v", "Package version to delete, or put a - to read from standard input")]
        public string Version { get; set; }

        public override async Task<bool> Run() {
            GitHubClient client = new GitHubClient(this.Token);

            string githubRepoVar = System.Environment.GetEnvironmentVariable("GITHUB_REPOSITORY") ?? string.Empty;
            string[] githubRepoVarParts = githubRepoVar.Split('/');
            string owner = this.Owner;
            if (string.IsNullOrWhiteSpace(owner)) {
                if (githubRepoVarParts.Length != 2) {
                    throw new CommandLineException($"Owner is required when running out of GitHub Actions");
                }
                owner = githubRepoVarParts[0];
            }

            string repo = this.Repository;
            if (string.IsNullOrWhiteSpace(repo)) {
                if (githubRepoVarParts.Length != 2)
                {
                    throw new CommandLineException($"Repository is required when running out of GitHub Actions");
                }
                repo = githubRepoVarParts[1];
            }

            Func<PackageVersion, bool> checkVersion;
            if (this.Mode == DeleteMode.Current) {
                string versionStr = this.Version;

                if (this.Version == "-") {
                    versionStr = Console.In.ReadLine();
                }

                if (!System.Version.TryParse(versionStr, out Version versionToDel)) {
                    throw new InvalidOperationException($"Version parameter is required when mode is current");
                }

                checkVersion = (v) => {
                    if (!System.Version.TryParse(v?.version, out Version version)) {
                        return false;
                    }
                    return version.Major == versionToDel.Major && version.Minor == versionToDel.Minor && version.Build == versionToDel.Build;
                };
            } else {
                checkVersion = (v) => true;
            }

            await foreach (var version in client.GetPackageVersions(owner, repo, this.Package)
                ?.Where(checkVersion)
                ?.OrderByDescending(v => System.Version.Parse(v.version))
                ?.Skip(this.VersionToKeep)) {
                Console.WriteLine($"Deleting package:{this.Package}, version: {version.version}");
                client.DeletePackageVersion(version.id);
            }

            return true;
        }
    }
}
