#!/bin/sh
# Root CA
path="./.tls"
mkdir -p "$path"
openssl genrsa -out ./root-ca-key.pem 2048
openssl req -new -x509 -sha256 -key ./root-ca-key.pem -subj "/C=IR/ST=KHORASAN/L=MASHHAD/O=NO1/OU=FB/CN=OS" -out ./root-ca.pem -days 730 \
  -addext 'basicConstraints = critical, CA:TRUE, pathlen:0' \
  -addext 'keyUsage = critical, keyCertSign, cRLSign' \
  -addext 'authorityKeyIdentifier = keyid'

# Admin cert
openssl genrsa -out ./admin-key-temp.pem 2048
openssl pkcs8 -inform PEM -outform PEM -in ./admin-key-temp.pem -topk8 -nocrypt -v1 PBE-SHA1-3DES -out ./admin-key.pem
openssl req -new -key ./admin-key.pem -subj "/C=IR/ST=KHORASAN/L=MASHHAD/O=NO1/OU=FB/CN=admin" -out ./admin.csr
openssl x509 -req -in ./admin.csr -CA ./root-ca.pem -CAkey ./root-ca-key.pem -CAcreateserial -sha256 -out ./admin.pem -days 730

# Node cert 1
openssl genrsa -out ./node1-key-temp.pem 2048
openssl pkcs8 -inform PEM -outform PEM -in ./node1-key-temp.pem -topk8 -nocrypt -v1 PBE-SHA1-3DES -out ./node1-key.pem
openssl req -new -key ./node1-key.pem -subj "/C=IR/ST=KHORASAN/L=MASHHAD/O=NO1/OU=FB/CN=fb-os-engine" -out ./node1.csr
echo 'subjectAltName=DNS:node1.dns.a-record' > ./node1.ext
openssl x509 -req -in ./node1.csr -CA ./root-ca.pem -CAkey ./root-ca-key.pem -CAcreateserial -sha256 -out ./node1.pem -days 730 -extfile ./node1.ext

# Node cert 2
openssl genrsa -out ./node2-key-temp.pem 2048
openssl pkcs8 -inform PEM -outform PEM -in ./node2-key-temp.pem -topk8 -nocrypt -v1 PBE-SHA1-3DES -out ./node2-key.pem
openssl req -new -key ./node2-key.pem -subj "/C=IR/ST=KHORASAN/L=MASHHAD/O=NO1/OU=FB/CN=nothing-yet" -out ./node2.csr
echo 'subjectAltName=DNS:node2.dns.a-record' > ./node2.ext
openssl x509 -req -in ./node2.csr -CA ./root-ca.pem -CAkey ./root-ca-key.pem -CAcreateserial -sha256 -out ./node2.pem -days 730 -extfile ./node2.ext

# Client cert (API / Serilog — CN must match roles_mapping all_access users: fb-api)
openssl genrsa -out ./client-key-temp.pem 2048
openssl pkcs8 -inform PEM -outform PEM -in ./client-key-temp.pem -topk8 -nocrypt -v1 PBE-SHA1-3DES -out ./client-key.pem
openssl req -new -key ./client-key.pem -subj "/C=IR/ST=KHORASAN/L=MASHHAD/O=NO1/OU=FB/CN=fb-api" -out ./client.csr
printf 'extendedKeyUsage = clientAuth\nsubjectAltName=DNS:fb-api\n' > ./client.ext
openssl x509 -req -in ./client.csr -CA ./root-ca.pem -CAkey ./root-ca-key.pem -CAcreateserial -sha256 -out ./client.pem -days 730 -extfile ./client.ext

# Cleanup
rm admin-key-temp.pem
rm admin.csr
rm node1-key-temp.pem
rm node1.csr
rm node1.ext
rm node2-key-temp.pem
rm node2.csr
rm node2.ext
rm client-key-temp.pem
rm client.csr
rm client.ext
mv root-ca.srl $path/root-ca.crt
mv *.pem $path