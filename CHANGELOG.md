# Changelog

All notable changes to this project will be documented in this file

Log format based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/).

## [3.3.13] - 2022-06-15

### Added

* Support right serialization for `JToken` fact values 

## [3.3.12] - 2022-02-10

### Changed

* Search `req-id` and `trace-id` in both `ActionLogScope` and `HostingLogScope` to support .NET5 case

## [3.3.10] - 2021-12-17

### Changed

* The `console log formatter` now adds `req-id` and `trace-id` instead all log scopes 

## [3.2.10] - 2021-11-19

### Added

* `MyLab` console log formatter 

## [3.1.9] - 2021-09-29

### Changed

* Short serialization of `System.Reflection.*` classes	

## [3.1.8] - 2021-08-29

### Fixed

* Fix serialization when one of fact value property throw an exception: null in `json`, text in`yaml`.

## [3.1.7] - 2021-08-19

### Fixed

* Bug with null value fact
* Bug with empty string value fact
* Bug with exception labels with the same names
* Bug with exception facts with the same names

### Removed

* Writing not default types in `yaml` serialization

## [3.1.5] - 2021-06-28

### Added

* Support handling logging errors

## [3.1.3] - 2021-06-28

### Added

* Extensions for `Exception`

## [3.0.3] - 2021-02-25

### Added

* Support implicit conversion `NULL-Exception` -> `NULL-ExceptionDto`

### Changed

* `PredefinedFacts` -> `PredefinedLabels`

### Fixed 

* Fix wrong `yaml` format in readme

## [3.0.1] - 2021-02-22

### Changed

* Support implicit conversion `NULL-Exception` -> `NULL-ExceptionDto`

## [3.0.0] - 2021-02-22

### Added

- This changelog
- Readme

### Changed

* Remove actually unnecessary log properties: `Id`, `EventId`
* Support built-in `yaml` serialization
* Support built-in `json` serialization  
* Rename log-attributes to facts
* Remake markers to labels
* Default formatter log representation form is `yaml`   
* Change exception around log functional
* Renaming `MyLab.Logging` -> `MyLab.Log`

