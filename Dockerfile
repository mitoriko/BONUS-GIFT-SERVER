FROM mcr.microsoft.com/dotnet/core/runtime:2.1
WORKDIR /app
EXPOSE 80
ADD ACBC/obj/Docker/publish /app
RUN cp /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
ENTRYPOINT ["dotnet", "ACBC.dll"]
