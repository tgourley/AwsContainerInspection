using System;

namespace AwsContainerInspectionTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Environment.SetEnvironmentVariable("ECS_CONTAINER_METADATA_FILE", args[0]);
            }
            
            var metadata = AwsContainerInspection.AwsContainerService.GetMetadata();

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
