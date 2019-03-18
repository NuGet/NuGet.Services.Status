# NuGet.Status

This repo contains the website running NuGet's [public status page](https://status.nuget.org). The status page is in an early preview.

## Contributing

Pull requests should be based on the `dev` branch. Once a release is planned, the `dev` branch or specific, cherry-picked changes can flow to `master`.

To deploy, merge into the `release-{environment}` branch.

* `release-dev` deploys to [http://nuget-dev-0-status.azurewebsites.net](http://nuget-dev-0-status.azurewebsites.net)
* `release-int` deploys to [http://nuget-int-0-status.azurewebsites.net](http://nuget-int-0-status.azurewebsites.net)
* `release-prod` deploys to [https://status.nuget.org](https://status.nuget.org) ([http://nuget-prod-status.trafficmanager.net](http://nuget-prod-status.trafficmanager.net))

## Feedback

If you're having trouble with the NuGet.org Website, file a bug on the [NuGet Gallery Issue Tracker](https://github.com/nuget/NuGetGallery/issues). 

If you're having trouble with the NuGet client tools (the Visual Studio extension, NuGet.exe command line tool, etc.), file a bug on [NuGet Home](https://github.com/nuget/home/issues).

Check out the [contributing](http://docs.nuget.org/contribute) page to see the best places to log issues and start discussions. The [NuGet Home](https://github.com/NuGet/Home) repo provides an overview of the different NuGet projects available.
