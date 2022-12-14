# TransXChange

TransXChange creates GTFS (General Transit Feed Specification) data sets for Great Britain's bus, coach, ferry, rail, tram and underground routes from Traveline TransXChange data files. The data sets created can be read with the NextDepartures .NET Library or any other GTFS parser.

* Build Status: [![Build Status](https://dev.azure.com/philvessey/TransXChange/_apis/build/status/philvessey.TransXChange?branchName=master)](https://dev.azure.com/philvessey/TransXChange/_build/latest?definitionId=7&branchName=master)

## Usage

```
dotnet run -n [--naptan] -t [--traveline] -o [--output] -k [--key]
```

* [naptan] > Path to NaPTAN .csv .zip or directory. Required.
* [traveline] > Path to regional Traveline TNDS .zip or directory. Required.
* [output] > Path to output directory. Required.
* [key] > Key for Nager.Date NuGet package. See https://github.com/sponsors/nager for information. Required.
* (mode) > Specify transport mode for schedules. Default (all). Optional. Modes: all, bus, city-rail, ferry, light-rail
* (indexes) > Specify filename indexes for schedules. Default (all). Optional. Separate by comma.
* (filters) > Specify stop filters for schedules. Default (all). Optional. Separate by comma.
* (days) > Specify days in advance for schedules. Default (7). Optional. Maximum is 28 days.

## License

Licensed under the [MIT License](./LICENSE).