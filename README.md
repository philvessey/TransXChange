# TransXChange

TransXChange creates GTFS (General Transit Feed Specification) data sets for Great Britain's bus, coach, ferry, rail, tram and underground routes from TransXChange data files. The data sets created can be read with the NextDepartures .NET Library or any other GTFS parser.

[![Build Status](https://dev.azure.com/philvessey/TransXChange/_apis/build/status/philvessey.TransXChange?branchName=master)](https://dev.azure.com/philvessey/TransXChange/_build/latest?definitionId=7&branchName=master)

## Usage

```
dotnet run -n [--naptan] -t [--transxchange] -o [--output] -k [--key]
```

* [naptan] > Path to NaPTAN .csv .zip or directory. Required.
* [transxchange] > Path to TransXChange .zip or directory. Required.
* [output] > Path to output directory. Required.
* [key] > Key for Nager.Date NuGet package. See https://github.com/sponsors/nager for information. Required.
* (mode) > Specify transport mode for schedules. Default (all). Optional. Modes: all, bus, city-rail, ferry, light-rail
* (indexes) > Specify filename indexes for schedules. Default (all). Optional. Separate by comma.
* (filters) > Specify stop filters for schedules. Default (all). Optional. Separate by comma.
* (date) > Specify date for schedules. Default (today). Optional. Dates: yesterday, today, tomorrow, dd/MM/yyyy
* (days) > Specify days in advance for schedules. Default (7). Optional. Maximum is 28 days.

## License

Licensed under the [MIT License](./LICENSE).