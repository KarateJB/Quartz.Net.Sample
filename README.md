# Quartz.Net.Sample

An .NET6 sample code using [Quartz.NET](https://github.com/quartznet/quartznet) and [Spectre.Console](https://github.com/spectreconsole/spectre.console).

## Features

- Self-hosted Scheduler to trigger a job automatically. 
- Console Interactive Mode to trigger a job manually. 

## Adding New Job and Trigger

I followed Andrew Lock's practice: [Using Quartz.NET with ASP.NET Core and worker services](https://andrewlock.net/using-quartz-net-with-asp-net-core-and-worker-services/) and here is the quick guide to add a new Quartz Job and Trigger.

### 1. Create New Implementation of Quartz.IJob

Create a new class (e.q. `NewDailyJob`) to implement `Quartz.IJob` in "src/Quartz.Net.Sample/Jobs/".

### 2. Add New Job and Trigger

Open "src/Quartz.Net.Sample/Utils/Extensions/IServiceCollectionQuartzConfiguratorExtensions.cs", add your new `Quartz.IJob` implementation in `UseQuartzJobs` function like following:

```cs
quartz.AddJobAndTrigger<NewDailyJob>(configuration);
```

### 3. Set the Cron Expression for the Job

Open "src/Quartz.Net.Sample/appsettings.jon" and add the new Job's cron schedule:

```json
{
  "Quartz": {
    "Job": {
      "NewDailyJob": "0 0 0 ? * *"
    }
  }
}
```

> See [Cron Expression Generator & Explainer - Quartz](https://www.freeformatter.com/cron-expression-generator-quartz.html).


## How to Test Trigger

To test the trigger, mock the **SystemTime.UtcNow** like this:

```cs
SystemTime.UtcNow = () => new DateTime(2023, 08, 31, 02, 0, 0);
```

I put the mocking options in "appsettings.Development.json" and `IServiceCollectionQuartzConfiguratorExtensions.UseQuartzJobs()` shows how to mock the `SystemTime.UtcNow`.


## (Optional) Publish Linux Executable

Run the command to publish the executable for Ubuntu:

```s
dotnet publish -c release -r ubuntu.18.04-x64 --self-contained
```

The executable binary `Quartz.Net.Sample` will be located in "src/Quartz.Net.Sample/bin/Release/net6.0/ubuntu.16.04-x64/publish/".

### Test the Executable by Docker

You can use Docker to run the executable.

```s
docker run --name quartz-ubuntu -it ubuntu:18.04
docker start
cd "src/Quartz.Net.Sample/bin/Release/net6.0/ubuntu.16.04-x64/"
docker cp ./publish/ quartz-ubuntu:/tmp/
docker exec -it jb-ubuntu bash

```

While we're in the container, we can either run the executable by
- `/tmp/publish/Quartz.Net.Sample` : start the self-hosted Quartz scheduler.
- `/tmp/publish/Quartz.Net.Sample -i` : start the interactive mode.

#### Trouble Shooting

`Error: Process terminated. Couldn't find a valid ICU package installed on the system. Please install libicu using your package manager and try again`

Solution: 

Set the environment variable before running the executable. (See [Runtime configuration options for globalization](https://learn.microsoft.com/en-us/dotnet/core/runtime-config/globalization))
```s
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
```

`Error: No usable version of libssl was found`

Solution:

- First try `apt-get update`.
- If you still get the error, see [No usable version of libssl was found #4749](https://github.com/dotnet/core/issues/4749#issuecomment-1200245422).


