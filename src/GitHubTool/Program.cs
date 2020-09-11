using WuGanhao.CommandLineParser;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace WuGanhao.GitHub {
    [SubCommand(typeof(DeletePackage), "delete-package", "Deletes package from GitHub")]
    [SubCommand(typeof(AutoMerge), "auto-merge", "Automatically merge all previous version")]
    [Description("Command line tool for manipulating GitHub packages")]
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            CommandLineParser<Program> CmdLineParser = new CommandLineParser<Program>();

            try
            {
                return await CmdLineParser.Invoke();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
                return -1;
            }
        }
    }
}
