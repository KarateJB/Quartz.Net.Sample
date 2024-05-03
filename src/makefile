.PHONY: build
.PHONY: clean
.PHONY: run
.PHONY: run-interactive
.PHONY: publish

build:
				# MSBuild.exe -clp:ErrorsOnly -nologo -m 
				dotnet build --no-restore /clp:ErrorsOnly

clean:
				dotnet clean -nologo -consoleLoggerParameters:NoSummary -verbosity:quiet

run:
				cd Quartz.Net.Sample/; \
				dotnet run --project ./Quartz.Net.Sample.csproj --property:Configuration=Debug

run-interactive:
				cd Quartz.Net.Sample/; \
				dotnet run --project ./Quartz.Net.Sample.csproj --property:Configuration=Debug -- -i

publish:
				MSBuild.exe -clp:ErrorsOnly -nologo -m /property:Configuration=Release
				# dotnet publish -c release -r centos.7-x64 --self-contained
				dotnet publish -c release -r ubuntu.18.04-x64 --self-contained
				# the binary is at "bin/Release/net6.0/ubuntu.16.04-x64/publish/"


