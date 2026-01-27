FROM alpine:3.20

RUN apk add --no-cache bash ca-certificates

COPY --from=bluenviron/mediamtx:1.15.6 /mediamtx /mediamtx

EXPOSE 8554 8889

ENTRYPOINT ["/mediamtx"]