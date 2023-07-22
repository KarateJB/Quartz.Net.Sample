.PHONY: build
.PHONY: clean
.PHONY: run
.PHONY: run-interactive

build:
				MSBuild.exe -clp:ErrorsOnly -nologo -m

clean:
				dotnet clean -nologo -consoleLoggerParameters:NoSummary -verbosity:quiet

run:
				cd Quartz.Net.Sample/; \
				dotnet run --project ./Quartz.Net.Sample.csproj --property:Configuration=Debug

run-interactive:
				cd Quartz.Net.Sample/; \
				dotnet run --project ./Quartz.Net.Sample.csproj --property:Configuration=Debug -- -i
