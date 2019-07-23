using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImportMultipleGitReposInVsts
{
    class Program
    {
        static void Main(string[] args)
        {
            TfsStatic.SourceTeamProjectBaseUri = "https://dev.azure.com/source account/source team project";
            TfsStatic.TargetTeamProjectBaseUri = "https://dev.azure.com/target account/target team project";
            SetConsoleThings();
            if (!GetPatToken())
            {
                return;
            }

            WriteSampleImportFile();
            //ImportReposFromFile();

            WriteLine();
            WriteLine();
            WriteLine();
            WriteLine("Done!");
            Console.ReadLine();
        }

        private static bool GetPatToken()
        {
            Console.WriteLine("PAT keys can be generated in TFS, keep this safe. With this key we are able to impersonate you using the TFS API's.");
            Console.WriteLine("Steps to create: https://www.visualstudio.com/en-us/docs/setup-admin/team-services/use-personal-access-tokens-to-authenticate");
            Console.WriteLine("TFS Uri: https://{account}/{tpc}/_details/security/tokens");
            Console.WriteLine();
            Console.WriteLine($"Source: {TfsStatic.SourceTeamProjectBaseUri}");
            Console.Write("Enter you Source PAT key: ");
            TfsStatic.SourcePatKey = Console.ReadLine();
            if ((TfsStatic.SourcePatKey?.Trim() ?? string.Empty).Length == 0)
            {
                Console.WriteLine();
                Console.WriteLine("Seems you didn't supply a key.");
                Console.ReadLine();
                return false;
            }
            Console.WriteLine($"Target: {TfsStatic.TargetTeamProjectBaseUri}");
            Console.Write("Enter you Target PAT key: ");
            TfsStatic.TargetPatKey = Console.ReadLine();
            if ((TfsStatic.TargetPatKey?.Trim() ?? string.Empty).Length == 0)
            {
                Console.WriteLine();
                Console.WriteLine("Seems you didn't supply a key.");
                Console.ReadLine();
                return false;
            }
            Console.Clear();
            return true;
        }

        private static void SetConsoleThings()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Clear();
        }

        private static void Write(string message = "", ConsoleColor colour = ConsoleColor.White)
        {
            Console.ForegroundColor = colour;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void WriteLine(string message = "", ConsoleColor colour = ConsoleColor.White)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void ImportReposFromFile()
        {
            var inputLines = File.ReadAllLines(".\\input.txt");
            var errorLines = new List<string>();
            foreach (var line in inputLines.Select(o => o.Trim()).Where(o => !o.StartsWith("#") && o.Length > 0))
            {
                var lineSplit = line.Split('\t');
                var remoteUrl = lineSplit[0];
                var newRepoName = lineSplit[1];

                try
                {
                    Write($"{newRepoName}...");
                    var newRepoResponse = CreateRepo(newRepoName);
                    Write($"repo created...", ConsoleColor.Green);
                    var serviceEndPointResponse = CreateServiceEndPoint(remoteUrl, newRepoName);
                    Write($"end point created...", ConsoleColor.Green);
                    var importRequestResponse = CreateImportRequest(remoteUrl, newRepoName, serviceEndPointResponse);
                    Write($"import request created...", ConsoleColor.Green);
                    WriteLine($"done!", ConsoleColor.Green);
                }
                catch (Exception ex)
                {
                    errorLines.Add(line);
                    WriteLine($"failed! - {ex.Message}", ConsoleColor.Red);
                }
            }
            File.WriteAllText(".\\errors.txt", string.Join(Environment.NewLine, errorLines));
        }

        private static CreateImportRequestResponse CreateImportRequest(string remoteUrl, string newRepoName, CreateServiceEndpointResponse serviceEndPointResponse)
        {
            return TfsStatic.CreateImportRequest(false, newRepoName, new CreateImportRequestRequest
            {
                parameters = new CreateImportRequestRequest_Parameters
                {
                    deleteServiceEndpointAfterImportIsDone = true,
                    gitSource = new CreateImportRequestRequest_Gitsource
                    {
                        url = remoteUrl,
                    },
                    serviceEndpointId = serviceEndPointResponse.id,
                }
            });
        }

        private static CreateRepoResponse CreateRepo(string newRepoName)
        {
            return TfsStatic.CreateRepo(false, new CreateRepoRequest
            {
                name = newRepoName,
                project = new CreateRepoRequest_Project
                {
                    id = TfsStatic.GetTeamProjectId(false),
                }
            });
        }

        private static CreateServiceEndpointResponse CreateServiceEndPoint(string remoteUrl, string newRepoName)
        {
            return TfsStatic.CreateServiceEndpoint(false, new CreateServiceEndpointRequest
            {
                authorization = new CreateServiceEndpointRequest_Authorization
                {
                    scheme = "UsernamePassword",
                    parameters = new CreateServiceEndpointRequest_Parameters
                    {
                        username = "",
                        password = TfsStatic.SourcePatKey,
                    }
                },
                name = $"Import-{newRepoName.Replace(" ", "-")}-{Guid.NewGuid()}-Git",
                type = "git",
                url = remoteUrl,
            });
        }

        private static void WriteSampleImportFile()
        {
            var output = string.Empty;
            var repos = TfsStatic.GetGitRepos(true);
            foreach (var repo in repos.value.OrderBy(o => o.magic_repo_name))
            {
                WriteLine(repo.magic_repo_name);
                output += $"{repo.remoteUrl}\t{repo.magic_repo_name}{Environment.NewLine}";
            }
            File.WriteAllText(".\\output.txt", output);
        }
    }
}
