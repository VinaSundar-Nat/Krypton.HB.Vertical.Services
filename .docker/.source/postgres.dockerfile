FROM postgres:15-bullseye AS base
LABEL maintainer="vina"
ARG DB="Offering,Identity"

ENV POSTGIS_MAJOR 3
ENV POSTGIS_VERSION 3.4.0+dfsg-1.pgdg110+1
# ENV POSTGIS_VERSION 3.3.4+dfsg-1.pgdg110+1

RUN apt-get update \
      && apt-cache showpkg postgresql-$PG_MAJOR-postgis-$POSTGIS_MAJOR \
      && apt-get install -y --no-install-recommends \
           # ca-certificates: for accessing remote raster files;
           #   fix: https://github.com/postgis/docker-postgis/issues/307
           ca-certificates \
           \
           postgresql-$PG_MAJOR-postgis-$POSTGIS_MAJOR=$POSTGIS_VERSION \
           postgresql-$PG_MAJOR-postgis-$POSTGIS_MAJOR-scripts \
      && rm -rf /var/lib/apt/lists/*

ADD ./.docker/.script/postgres.sh /docker-entrypoint-initdb.d/
RUN chmod a+x /docker-entrypoint-initdb.d/postgres.sh

EXPOSE 5432