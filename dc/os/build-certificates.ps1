# Root CA
$path = ".\.tls"

New-Item -ItemType Directory -Force -Path $path | Out-Null

openssl genrsa -out ./root-ca-key.pem 2048

openssl req -new -x509 -sha256 `
  -key ./root-ca-key.pem `
  -subj "/C=IR/ST=KHORASAN/L=MASHHAD/O=NO1/OU=FB/CN=OS" `
  -out ./root-ca.pem `
  -days 730 `
  -addext "basicConstraints = critical, CA:TRUE, pathlen:0" `
  -addext "keyUsage = critical, keyCertSign, cRLSign" `
  -addext "authorityKeyIdentifier = keyid"

# Admin cert
openssl genrsa -out ./admin-key-temp.pem 2048

openssl pkcs8 `
  -inform PEM `
  -outform PEM `
  -in ./admin-key-temp.pem `
  -topk8 `
  -nocrypt `
  -v1 PBE-SHA1-3DES `
  -out ./admin-key.pem

openssl req `
  -new `
  -key ./admin-key.pem `
  -subj "/C=IR/ST=KHORASAN/L=MASHHAD/O=NO1/OU=FB/CN=admin" `
  -out ./admin.csr

openssl x509 `
  -req `
  -in ./admin.csr `
  -CA ./root-ca.pem `
  -CAkey ./root-ca-key.pem `
  -CAcreateserial `
  -sha256 `
  -out ./admin.pem `
  -days 730

# Node cert 1
openssl genrsa -out ./node1-key-temp.pem 2048

openssl pkcs8 `
  -inform PEM `
  -outform PEM `
  -in ./node1-key-temp.pem `
  -topk8 `
  -nocrypt `
  -v1 PBE-SHA1-3DES `
  -out ./node1-key.pem

openssl req `
  -new `
  -key ./node1-key.pem `
  -subj "/C=IR/ST=KHORASAN/L=MASHHAD/O=NO1/OU=FB/CN=fb-os-engine" `
  -out ./node1.csr

# Include all DNS names the server should present/validate for (add os and fb-os-engine)
"subjectAltName=DNS:node1,DNS:os,DNS:fb-os-engine" | Set-Content ./node1.ext

openssl x509 `
  -req `
  -in ./node1.csr `
  -CA ./root-ca.pem `
  -CAkey ./root-ca-key.pem `
  -CAcreateserial `
  -sha256 `
  -out ./node1.pem `
  -days 730 `
  -extfile ./node1.ext

# Node cert 2
openssl genrsa -out ./node2-key-temp.pem 2048

openssl pkcs8 `
  -inform PEM `
  -outform PEM `
  -in ./node2-key-temp.pem `
  -topk8 `
  -nocrypt `
  -v1 PBE-SHA1-3DES `
  -out ./node2-key.pem

openssl req `
  -new `
  -key ./node2-key.pem `
  -subj "/C=IR/ST=KHORASAN/L=MASHHAD/O=NO1/OU=FB/CN=nothing-yet" `
  -out ./node2.csr

"subjectAltName=DNS:node2" | Set-Content ./node2.ext

openssl x509 `
  -req `
  -in ./node2.csr `
  -CA ./root-ca.pem `
  -CAkey ./root-ca-key.pem `
  -CAcreateserial `
  -sha256 `
  -out ./node2.pem `
  -days 730 `
  -extfile ./node2.ext

# Client cert
openssl genrsa -out ./client-key-temp.pem 2048

openssl pkcs8 `
  -inform PEM `
  -outform PEM `
  -in ./client-key-temp.pem `
  -topk8 `
  -nocrypt `
  -v1 PBE-SHA1-3DES `
  -out ./client-key.pem

openssl req `
  -new `
  -key ./client-key.pem `
  -subj "/C=IR/ST=KHORASAN/L=MASHHAD/O=NO1/OU=FB/CN=fb-api" `
  -out ./client.csr

@(
  "extendedKeyUsage = clientAuth"
  "subjectAltName=DNS:fb-api"
) | Set-Content ./client.ext

openssl x509 `
  -req `
  -in ./client.csr `
  -CA ./root-ca.pem `
  -CAkey ./root-ca-key.pem `
  -CAcreateserial `
  -sha256 `
  -out ./client.pem `
  -days 730 `
  -extfile ./client.ext

# Cleanup
Remove-Item admin-key-temp.pem -ErrorAction Ignore
Remove-Item admin.csr -ErrorAction Ignore

Remove-Item node1-key-temp.pem -ErrorAction Ignore
Remove-Item node1.csr -ErrorAction Ignore
Remove-Item node1.ext -ErrorAction Ignore

Remove-Item node2-key-temp.pem -ErrorAction Ignore
Remove-Item node2.csr -ErrorAction Ignore
Remove-Item node2.ext -ErrorAction Ignore

Remove-Item client-key-temp.pem -ErrorAction Ignore
Remove-Item client.csr -ErrorAction Ignore
Remove-Item client.ext -ErrorAction Ignore

Move-Item root-ca.srl "$path\root-ca.crt" -Force
Move-Item *.pem $path -Force