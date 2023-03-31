# Get base SDK Image from Microsoft
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /Bookinist

# Copy the CSPROJ file and restore any dependecies (via NUGET)
COPY *.csproj ./
RUN dotnet restore

# Copy the project files and build our release
COPY . ./
RUN dotnet publish -c Release -o out

# Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /Bookinist
COPY --from=build-env /Bookinist/out .
ENTRYPOINT ["dotnet", "Bookinist.dll"]

