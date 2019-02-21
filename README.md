# Simple Tunnel

> See more: https://hub.docker.com/r/thiagobarradas/simple-tunnel

Simple HTTP Tunnel for API testing

### Running a Container

Run: `docker run --name simple-tunnel -p80:8087 -d thiagobarradas/simple-tunnel`

Call this API with original request, change base URL to query string `tunnel_url`, like:

`http://localhost/some-path/testing?some-query=1&tunnel_url=https://my-original-url.com`

Tunnel URL must be only base url. The previously request is sended to:

`https://my-original-url.com/some-path/testing?some-query=1`

## How can I contribute?
Please, refer to [CONTRIBUTING](.github/CONTRIBUTING.md)

## Found something strange or need a new feature?
Open a new Issue following our issue template [ISSUE_TEMPLATE](.github/ISSUE_TEMPLATE.md)

## Changelog
See in [nuget version history](https://www.nuget.org/packages/RestSharp.Serilog.Auto)

## Did you like it? Please, make a donate :)

if you liked this project, please make a contribution and help to keep this and other initiatives, send me some Satochis.

BTC Wallet: `1G535x1rYdMo9CNdTGK3eG6XJddBHdaqfX`

![1G535x1rYdMo9CNdTGK3eG6XJddBHdaqfX](https://i.imgur.com/mN7ueoE.png)
