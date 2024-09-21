FROM  redis:latest
LABEL maintainer="vina"

COPY   ./.docker/.config/redis.prod.conf /cache/redis.conf

ENTRYPOINT  ["redis-server", "/cache/redis.conf"]