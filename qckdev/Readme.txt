------------------------------------------------------------------
--- Library compatible with .NET Standard 2.0 + .NET Portable. ---
------------------------------------------------------------------

.NET compatibility table:
https://docs.microsoft.com/en-us/dotnet/standard/net-standard

Multi-targeting framework setup:
https://stackoverflow.com/questions/44979449/net-multi-target-with-net-standard-and-portable-library-msbuild-15 (foro)
https://github.com/onovotny/MSBuildSdkExtras (github)
https://portablelibraryprofiles.stephencleary.com/ (profiles)

Most of the differences are in the following namespaces:
	- System.Reflection: several methods missing.
	- System.Data: not exists.