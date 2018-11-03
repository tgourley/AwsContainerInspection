# AwsContainerInspection .NET Standard Library

When a conatiner is launched in AWS ECS, the container can either read from a metadata file on the system or call into an external endpoint (web request) to pull the metadata information on that running container. 

If the Amazon ECS container agent is enabled it creates a metadata JSON file with identifying and configuration information.  This will look for that file and pull that information into an object that can be referenced.

The package provides a service library that creates an object containing all the metadata.