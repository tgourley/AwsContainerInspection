using AwsContainerInspection;
using System;
using System.IO;

namespace AwsContainerInspectionTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                switch (args[0])
                {
                    case "file":
                        TestFile(args[1]);
                        break;
                    case "endpoint":
                        TestEndpoint(args[1]);
                        break;
                    default:
                        Console.WriteLine("No valid option.");
                        break;
                }
            }
        }

        private static void TestEndpoint(string filename)
        {
            FileInfo metadataFileInfo = new FileInfo(filename);

            if (metadataFileInfo.Exists)
            {
                StreamReader metadataFileStream = metadataFileInfo.OpenText();

                string metadataFileContents = metadataFileStream.ReadToEnd();

                if (!string.IsNullOrWhiteSpace(metadataFileContents))
                {
                    var metadata = AwsContainerService.GetMetadataFromEndpoint(metadataFileContents);

                    if (metadata != null)
                    {
                        Console.WriteLine($"Metadata found: {metadata.GetTaskGuid()}");
                    }
                    else
                    {
                        Console.WriteLine("Metadata NOT found.");
                    }
                }
            }
        }

        private static void TestFile(string filename)
        {
            Environment.SetEnvironmentVariable("ECS_CONTAINER_METADATA_FILE", filename);

            var metadata = AwsContainerService.GetMetadataFromFile();

            if (metadata != null)
            {
                Console.WriteLine("Metadata found.");
            }
            else
            {
                Console.WriteLine("Metadata NOT found.");
            }
        }
    }
}
