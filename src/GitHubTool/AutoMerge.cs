using LibGit2Sharp;
using WuGanhao.CommandLineParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WuGanhao.GitHub {
    public enum ConflictMode {
        Skip,
        Break,
    }

    public class AutoMerge: SubCommand {
        [CommandOption("target",      "t", "Target version/branch where you merge to")]
        public string TargetBranch { get; set; }

        [CommandOption("on-conflict", "c", "Action on conflicted, default to break")]
        public ConflictMode ConflictMode { get; set; } = ConflictMode.Break;

        public override async Task<bool> Run() {
            string repositoryPath = Repository.Discover(Directory.GetCurrentDirectory());
            Console.WriteLine($"Starting with path {repositoryPath}...");

            using var repository = new Repository(repositoryPath);
            string currentBranch = this.TargetBranch ?? repository.Head.FriendlyName;
            Console.WriteLine($"Current branch {currentBranch}...");

            string strCurrentVersion = currentBranch.StartsWith("release/") ? currentBranch.Substring(8) : currentBranch;
            if (!System.Version.TryParse(strCurrentVersion, out System.Version currentVersion)) {
                Console.WriteLine("Current branch is not a release");
                return false;
            }

            IEnumerable<Branch> branches = repository.Branches.Where(b => b.FriendlyName.StartsWith("origin/"))
                .Select(b => {
                     string refs = b.FriendlyName.Substring(7);
                     string str = refs.StartsWith("release/") ? refs.Substring(8) : refs;
                     if (!System.Version.TryParse(str, out System.Version version)) {
                         version = null;
                     }
                     return (Branch: b, Version: version);
                 })
                .Where(bv => bv.Version != null)
                .Where(bv => bv.Version < currentVersion)
                .OrderByDescending(bv => bv.Version)
                .Select(bv => bv.Branch);

            Console.WriteLine($"Start merging...");

            Signature merger = repository.Config.BuildSignature(DateTimeOffset.Now);

            foreach (Branch b in branches) {
                if (repository.Index.Conflicts.Any()) {
                    Console.WriteLine($"!!! Working folder already in conflicted status, please resolve it first.");
                    return false;
                }

                Console.WriteLine($"Merging {b.CanonicalName}...");
                repository.Merge(b, merger);

                if (repository.Index.Conflicts.Any()) {
                    if (this.ConflictMode == ConflictMode.Break) {
                        Console.WriteLine($"!!! Conflicted, Resolve your local conflicts and try again.");
                        return false;
                    } else {
                        Console.WriteLine($"!!! Conflicted, skipping {b.CanonicalName}...");
                        repository.Reset(ResetMode.Hard);
                    }
                }
            }
            return true;
        }
    }
}
