#Image base with aspnet core 8.0
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get install -y iputils-ping curl
WORKDIR /app
EXPOSE 80

#Image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

#Copy file project
COPY . ./

RUN dotnet restore

#Install dotnet-ef as global tool
RUN dotnet tool install --global dotnet-ef \
    && echo 'export PATH="$PATH:/root/.dotnet/tools"' >> /root/.bashrc

RUN dotnet publish -c Release -o /app/publish

#Final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

#Run the app
ENTRYPOINT ["dotnet", "DockerExample.dll"]