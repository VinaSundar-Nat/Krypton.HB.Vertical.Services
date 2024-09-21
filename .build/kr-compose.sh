#!/bin/bash
source ./kr-variable.sh
file="./runtime.release.env"

function remove_file() {
        rm $file
        rm $file'.bak' 
}

function remove_deps() {
    img=$(docker images -q)
    echo $img
    if [  $? -eq 0 ]; then
        echo 'Removing all images.'
        docker rmi $(docker images -q)
    fi

    docker volume ls -q
    if [  $? -eq 0 ]; then
        echo 'Removing all volumes.'
        docker volume  rm $(docker volume  ls -q)
    fi
}

remove_file

if [$prune == "true"]; then
    docker builder prune -f
fi

cp ../.docker/.env/runtime.release.env ./runtime.release.env

if [  $? -eq 0 ]; then
    echo 'Copy completed.'
    ls ./
else
   exit 1
fi

echo 'replace variable'

sed -i.bak -e "s/\$version/$krversion/g" \
           -e "s/\$volume/$krvolume/g" \
           -e "s/\$arg/$buildArg/g" \
           ./runtime.release.env

if [  $? -eq 0 ]; then
    echo 'variable replacement completed.'
else
   exit 1
fi

COMPOSE_FILES="-f ../docker-compose-utility.yml -f ../docker-compose-services.yml"

if [$includeComp == 'true'] ; then
    COMPOSE_FILES="$COMPOSE_FILES -f ../docker-compose-components.yml"
fi

if [ $printRenderedComposeFile == 'true' ]  ; then
    echo 'print replaced compose file'
    docker-compose $COMPOSE_FILES --env-file ./runtime.release.env config
fi

if [ $build == 'true' ]  ; then
    echo "Build compose version $krversion"   
    docker-compose $COMPOSE_FILES --env-file ./runtime.release.env build

    if [  $? -eq 0 ]; then
        echo "Build - $krversion completed sucessfully."
    fi
fi

if [ $compose == 'true' ]  ; then
    echo "create container layer for build version - $krversion"
    docker-compose $COMPOSE_FILES --env-file ./runtime.release.env --verbose up -d

    if [  $? -eq 0 ]; then
        echo "Service and dependencies started sucessfully."
        if [ $certtrust == 'true' ]  ; then
            ./kr-ssl-trust.sh
        fi
    fi

elif [ $destroy == 'true' ]; then
    echo "destroy container layer for build version - $krversion"
    docker-compose $COMPOSE_FILES --env-file ./runtime.release.env down
    remove_deps   
fi

remove_file


