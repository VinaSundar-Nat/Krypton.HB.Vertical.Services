#!/bin/bash
source ./kr-variable.sh

function copyCert(){
    docker cp ../.cert/krcert.pfx $1:/krcerts
    docker cp ../.cert/krcert.crt $1:/etc/ssl/certs/krcert.crt
    # docker cp ../.cert/krcert.crt $1:/usr/local/share/ca-certificates/krcert.crt
}

function trustCert(){
    docker exec -it $1 bash -c "apt-get update"
    docker exec -it $1 bash -c "cd /etc/ssl/certs/ && update-ca-certificates"
}

function restart(){
    docker stop $1
    docker start $1
}

DOCUMENT_API_CN=$(docker ps -a -q --filter "name=$doc_api_name-*")
echo "Container id for document api : $DOCUMENT_API_CN"

echo 'Copy certificate to DOC host container'
copyCert $DOCUMENT_API_CN

if [  $? -eq 0 ]; then
    echo "Trust cert in container $DOCUMENT_API_CN started."
    trustCert $DOCUMENT_API_CN
    echo "Restart container $DOCUMENT_API_CN ."
    restart $DOCUMENT_API_CN
fi




