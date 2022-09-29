#Get Base Image (Full .Net SDK)
FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /source

#Copy csproj and restore
COPY AspNetCoreTodo/*.csproj ./AspNetCoreTodo/
RUN dotnet restore ./AspNetCoreTodo/AspNetCoreTodo.csproj

#Copy everything else, build, and publish
COPY AspNetCoreTodo/. ./AspNetCoreTodo
WORKDIR /source/AspNetCoreTodo
RUN dotnet publish -c release -o /app --no-restore

#Generate runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
EXPOSE 80
#ENV ASPNETCORE_URLS=http://+:5000
WORKDIR /app
COPY --from=build /app ./

#Create group and user
RUN groupadd -r dev && useradd -g dev dev

#Set ownership and permissions
RUN chown -R dev:dev /app

#Switch to user
USER dev

ENTRYPOINT ["dotnet", "AspNetCoreTodo.dll"]