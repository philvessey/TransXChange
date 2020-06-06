# TransXChange

TransXChange creates GTFS (General Transit Feed Specification) data sets for Great Britain's bus, coach, ferry, rail, tram and underground routes from Traveline TransXChange data files. The data sets created can be read with the NextDepartures .NET Standard Library or any other GTFS parser.

* Build Status: [![Build Status](https://dev.azure.com/philvessey/TransXChange/_apis/build/status/philvessey.TransXChange?branchName=master)](https://dev.azure.com/philvessey/TransXChange/_build/latest?definitionId=7&branchName=master)

## Prerequisites

Ensure you already have:

* NaPTAN (National Public Transport Access Nodes) data can be downloaded from [here](https://data.gov.uk/dataset/ff93ffc1-6656-47d8-9155-85ea0b8f2251/national-public-transport-access-nodes-naptan). The CSV dataset should be used.
* Traveline (Traveline National Dataset) data can be downloaded from [here](https://www.travelinedata.org.uk/traveline-open-data/traveline-national-dataset/). Requires free sign up.

## Usage

```
TransXChange.England > dotnet run -n [naptan] -t [traveline] -o [output] -m (mode) -f (filters) -d (days)
TransXChange.Scotland > dotnet run -n [naptan] -t [traveline] -o [output] -m (mode) -f (filters) -d (days)
TransXChange.Wales > dotnet run -n [naptan] -t [traveline] -o [output] -m (mode) -f (filters) -d (days)
```

* [naptan] > Path to NaPTAN csv .zip or directory. Required.
* [traveline] > Path to regional Traveline TNDS .zip or directory. Required.
* [output] > Path to output directory. Required.
* (mode) > Specify transport mode. Default (all). Optional. Modes: all, bus, city-rail, ferry, light-rail
* (filters) > Specify stop filters. Default (all). Optional. Separate by comma.
* (days) > Specify days in advance for schedules. Default (7). Optional. Maximum is 28 days.

## License

Licensed under the [MIT License](./LICENSE).