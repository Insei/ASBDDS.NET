#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
ARG TARGETARCH

WORKDIR /app
COPY ./build/linux-$TARGETARCH .
ENTRYPOINT ["./ASBDDS.API"]