FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
LABEL maintainer="vina"
WORKDIR /src
EXPOSE 7189

COPY ../Directory.Packages.props .
COPY ../SharedKernal/ ./SharedKernal
RUN dotnet restore SharedKernal/KR.Common/*.csproj &&\ 
    dotnet restore SharedKernal/KR.Infrastructure/*.csproj

COPY ../Core/Document/ ./Core/Document/
RUN dotnet restore Core/Document/KR.Document.HB.Domain/*.csproj &&\
    dotnet restore Core/Document/KR.Document.HB.Application/*.csproj &&\ 
    dotnet restore Core/Document/KR.Document.HB.Adapter/*.csproj &&\ 
    dotnet restore Core/Document/KR.Document.HB.Api/*.csproj


WORKDIR /src/Core/Document/KR.Document.HB.Api
RUN dotnet publish KR.Document.HB.Api.csproj -c Release --no-restore -o /krdocument

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled
WORKDIR /krdocument

COPY --from=build /krdocument .
ENTRYPOINT ["dotnet", "KR.Document.HB.Api.dll"]