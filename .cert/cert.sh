#!/bin/bash

function generate_cert_from_ca(){
#Generate Root certificate
openssl genrsa -out krrootcert.key 2048
openssl req -x509 -new -nodes -key krrootcert.key -sha256 -days 1825 -out krCA.pem -config ./.conf/krroot.conf

if [  $? -eq 0 ]; then
    echo 'krrootcert created.'
else
   exit 1
fi

#Generate certificate from pem (N/Req for dev)
openssl x509 -outform der -in krCA.pem -out krroot.crt

if [  $? -eq 0 ]; then
    echo 'krCA created.'
else
   exit 1
fi

#Generate CSR
openssl genrsa -out krcsr.key 2048
openssl req -new -key krcsr.key -out kr.csr -subj \
 "/C=AU/ST=NSW/L=SYD/O=Ammanukkaale/OU=Sentiments/CN=acc/emailAddress=vinasundar.kr@hotmail.com"

if [  $? -eq 0 ]; then
    echo 'krcsr created.'
else
   exit 1
fi

#Generate cert
openssl x509 -req -in kr.csr -CA krCA.pem -CAkey krrootcert.key -CAcreateserial  -days 365 \
 -sha256 -out krcert.crt -extensions v3_req -extfile ./.conf/krhost.conf

openssl pkcs12 -export -out krcert.pfx -inkey krcsr.key -in krcert.crt -password pass:'kr2023c'
}

function generate_cert_self_signed(){
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout krcert.key \
    -out krcert.crt -config ./.conf/krhost.conf

    openssl pkcs12 -export -out krcert.pfx -inkey krcert.key -in krcert.crt -password pass:'kr2023c'

    if [  $? -eq 0 ]; then
        echo 'krcert.pfx created.'
    else
    exit 1
    fi
}

generate_cert_self_signed

#working eg conf - selfsigned with no root CA
# openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout krcert.key -out krcert.crt -config ./.conf/krhost.conf
# openssl pkcs12 -export -out krcert.pfx -inkey krcert.key -in krcert.crt
# RUN apt-get install ca-certificates &&/ dpkg-reconfigure ca-certificates