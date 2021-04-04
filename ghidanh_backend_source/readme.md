# 🌤️ Sample Project .Net Core 🌥️

> Sample Project .Net Core includes frequently used services to help the process of creating new projects fast - compact.

## Getting Started

This document will summarize the changes for each specific version such as: New Features, Bug Fixes and Enhancements as well as new updates to existing services and libraries.

## Release History

* **Version `0.0.1` (June 10, 2020)**:
  * **`[New Features]` Use Https:** Getting the current url of the request depends on whether you use Https or not?
  * **`[Enhancements]` Cors With Origins - Allow Specific Origin:** Only allow listed client sources.
  * **`[Enhancements]` Remove `UseUrls` In Program:** After removing `UseUrls`, when running dotnet on the server please run the command: `dotnet Project.dll --urls "http://0.0.0.0:9000"`.
  * **`[Bug Fixes + Enhancements]` Exception Handler Middleware:** New update for Status Bad Request and Fix error when Parse data is not Json.
  * **`[Bug Fixes + Enhancements]` Send Request Async In Helper:** Fix the error when not passing the `endpointURL` parameter and removing the case that` Message Content` is not null.
  * **`[New Features]` Request Table And Response Table:** Add two classes `RequestTable` and` ResponseTable` in `Helpers` to support paging.
  * **`[New Features]` Schedules Feature:** Add `Schedules` and the` Sample Job` template to perform periodic jobs.
  * **`[Enhancements]` Upgrade NETCore:** Upgrade NETCore from 2.2 to 3.1 and related libraries.

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.