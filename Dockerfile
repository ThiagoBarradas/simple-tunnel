# How publish this image? After publish dotnet package, do:
# docker login
# docker build --no-cache -t thiagobarradas/simple-tunnel:latest .
# docker tag thiagobarradas/simple-tunnel:latest thiagobarradas/simple-tunnel:latest
# docker push thiagobarradas/simple-tunnel:latest

FROM microsoft/dotnet:2.1-runtime

# Default Environment
ENV ASPNETCORE_ENVIRONMENT="Development"

# Args
ARG distFolder=SimpleTunnel/bin/Release/netcoreapp2.1/publish
ARG apiProtocol=http
ARG apiPort=8087
ARG appFile=SimpleTunnel.dll

# Copy files to /app
RUN ls
COPY ${distFolder} /app
 
# Expose port for the Web API traffic
ENV ASPNETCORE_URLS ${apiProtocol}://+:${apiPort}
EXPOSE ${apiPort}

# Run application
WORKDIR /app
RUN ls
ENV appFile=$appFile
ENTRYPOINT dotnet $appFile