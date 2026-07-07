# AwsContainerInspection

[![NuGet](https://img.shields.io/nuget/v/AwsContainerInspection.svg)](https://www.nuget.org/packages/AwsContainerInspection/)
[![CI](https://github.com/tgourley/AwsContainerInspection/actions/workflows/ci.yml/badge.svg)](https://github.com/tgourley/AwsContainerInspection/actions/workflows/ci.yml)

A small .NET Standard 2.0 library that reads the metadata Amazon ECS exposes about a
running container and surfaces it as strongly-typed objects.

When a container runs on AWS ECS, the ECS agent publishes identifying and configuration
information about the task and container — the cluster, the task and container ARNs, the
Docker image, port mappings, networks, resource limits, and more. This library reads that
information from either of the two places ECS makes it available and deserializes it for
you, so you don't have to hand-roll the JSON parsing in every service.

## What it does

ECS exposes container metadata through two mechanisms, and this library supports both:

| Source | Method | What you get |
| --- | --- | --- |
| **Metadata file** | `GetMetadataFromFile()` | Container-level metadata written to a JSON file on disk (path provided via the `ECS_CONTAINER_METADATA_FILE` environment variable). Returns an `AwsContainerFile`. |
| **Task metadata endpoint** | `GetMetadataFromEndpoint()` | Task-level metadata served over HTTP from the ECS task metadata endpoint (`http://169.254.170.2/v2/metadata`), including every container in the task. Returns an `AwsContainerEndpoint`. |

Both are read-only lookups. When metadata is unavailable or cannot be parsed, the methods
return `null` rather than throwing, so calling them off-ECS (for example, on a developer
machine) is safe.

## Installation

Install from [NuGet](https://www.nuget.org/packages/AwsContainerInspection/):

```bash
dotnet add package AwsContainerInspection
```

Or via the Package Manager Console:

```powershell
Install-Package AwsContainerInspection
```

Or add a `PackageReference` directly:

```xml
<PackageReference Include="AwsContainerInspection" Version="1.0.4" />
```

## Usage

The entry point is the static `AwsContainerService` class.

### Read from the metadata file

Requires the ECS container agent to have metadata enabled (which sets the
`ECS_CONTAINER_METADATA_FILE` environment variable inside the container).

```csharp
using AwsContainerInspection;

AwsContainerFile metadata = AwsContainerService.GetMetadataFromFile();

if (metadata != null)
{
    Console.WriteLine($"Cluster:        {metadata.Cluster}");
    Console.WriteLine($"Container name: {metadata.ContainerName}");
    Console.WriteLine($"Image:          {metadata.ImageName}");
    Console.WriteLine($"Task GUID:      {metadata.GetTaskGuid()}");

    foreach (var mapping in metadata.PortMappings)
    {
        Console.WriteLine($"Port: {mapping.ContainerPort} -> {mapping.HostPort} ({mapping.Protocol})");
    }
}
```

### Read from the task metadata endpoint

Makes an HTTP request to the ECS task metadata endpoint. This works from inside any
container running in the task.

```csharp
using AwsContainerInspection;

AwsContainerEndpoint task = AwsContainerService.GetMetadataFromEndpoint();

if (task != null)
{
    Console.WriteLine($"Family:    {task.Family}");
    Console.WriteLine($"Task GUID: {task.GetTaskGuid()}");
    Console.WriteLine($"CPU/Mem:   {task.Limits.CPU} / {task.Limits.Memory}");

    foreach (var container in task.Containers)
    {
        Console.WriteLine($"- {container.Name} ({container.Image}) is {container.KnownStatus}");
    }
}
```

### Parse metadata you already have

If you have already fetched the endpoint payload (for example through your own HTTP client),
you can deserialize it directly. This overload is also handy in unit tests:

```csharp
string json = /* raw task metadata JSON */;

AwsContainerEndpoint task = AwsContainerService.GetMetadataFromEndpoint(json);
```

## API reference

### `AwsContainerService`

| Member | Returns | Description |
| --- | --- | --- |
| `GetMetadataFromFile()` | `AwsContainerFile` | Reads and parses the file at `ECS_CONTAINER_METADATA_FILE`. Returns `null` if the variable is unset, the file is missing, or the contents are not valid JSON. |
| `GetMetadataFromEndpoint()` | `AwsContainerEndpoint` | Requests and parses `http://169.254.170.2/v2/metadata`. Returns `null` on any request or parse failure. |
| `GetMetadataFromEndpoint(string metadata)` | `AwsContainerEndpoint` | Parses an already-retrieved JSON string. Returns `null` if the string is not valid JSON. |

### `AwsContainerFile` (metadata-file model)

`Cluster`, `ContainerInstanceARN`, `TaskARN`, `ContainerID`, `ContainerName`,
`DockerContainerName`, `ImageID`, `ImageName`, `MetadataFileStatus`,
`PortMappings` (`IEnumerable<AwsContainerPortMapping>`),
`Networks` (`IEnumerable<AwsContainerNetwork>`).

Helper methods:

- `GetTaskGuid()` — the GUID portion of `TaskARN`.
- `GetContainerInstanceGuid()` — the GUID portion of `ContainerInstanceARN`.

### `AwsContainerEndpoint` (task-endpoint model)

`Cluster`, `Family`, `Revision`, `DesiredStatus`, `KnownStatus`, `TaskARN`,
`PullStartedAt`, `PullStoppedAt`, `Limits` (`AwsContainerLimit`),
`Containers` (`IEnumerable<AwsContainer>`).

Helper method:

- `GetTaskGuid()` — the GUID portion of `TaskARN`.

### `AwsContainer` (a container within a task)

`Name`, `DockerId`, `DockerName`, `Image`, `ImageID`, `CreatedAt`, `StartedAt`,
`DesiredStatus`, `KnownStatus`, `Type`, `Labels` (`AwsContainerLabels`),
`Limits` (`AwsContainerLimit`), `Networks` (`IEnumerable<AwsContainerNetwork>`).

### Supporting types

- **`AwsContainerPortMapping`** — `ContainerPort`, `HostPort`, `BindIp`, `Protocol`.
- **`AwsContainerNetwork`** — `NetworkMode`, `IPv4Addresses` (`IEnumerable<string>`).
- **`AwsContainerLimit`** — `CPU`, `Memory` (both `decimal`, so fractional vCPU reservations parse correctly).
- **`AwsContainerLabels`** — the standard `com.amazonaws.ecs.*` labels (cluster, container name, task ARN, task-definition family, task-definition version).

### `StringExtensions`

- `string.GetGuidFromEndOfArn()` — extension method that returns the trailing segment
  (the GUID) of an ARN after the last `/`, or `string.Empty` if the input is null,
  whitespace, or has no `/`. This is what the `GetTaskGuid()` / `GetContainerInstanceGuid()`
  helpers use internally, and it is available for your own ARN parsing.

## Technical details

- **Target framework:** `netstandard2.0` — compatible with .NET Core 2.0+, .NET 5+, and
  .NET Framework 4.6.1+.
- **Dependencies:** [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json/) for
  deserialization. [LibLog](https://github.com/damianh/LibLog) is used internally for
  logging and is bundled as a private asset, so it does not appear as a transitive
  dependency in your project.
- **Logging:** LibLog transparently detects the logging framework already present in the
  host application (Serilog, NLog, log4net, Microsoft.Extensions.Logging via adapters,
  etc.). If none is configured, logging is a no-op. Errors encountered while reading or
  parsing metadata are logged at the error level.
- **Error handling:** every public method is designed to fail soft — it returns `null`
  instead of throwing when metadata is unavailable or malformed, so it is safe to call
  unconditionally and branch on the result.

## Metadata sources and versions

The file-based model (`AwsContainerFile`) maps the ECS
[container metadata file](https://docs.aws.amazon.com/AmazonECS/latest/developerguide/container-metadata.html),
which describes the current container. The endpoint-based model (`AwsContainerEndpoint`)
maps the task metadata endpoint response, which describes the whole task and every
container in it. Choose the source that matches the scope of information you need.

## Building and testing

The repository is a standard .NET SDK solution:

```bash
# Build the library
dotnet build src/AwsContainerInspection/AwsContainerInspection.csproj

# Run the test suite
dotnet test test/AwsContainerInspection.Tests/AwsContainerInspection.Tests.csproj
```

Tests run automatically on every push and pull request via GitHub Actions, and tagged
releases (`v*`) are published to NuGet.org through
[NuGet Trusted Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/trusted-publishing).
