#!/bin/sh

sed -i 's/\r$//' ".env"

set -a
. ./.env
set +a

src=".tls"
dest=".ks"

mkdir -p "$dest"

# Optional: remove old keystore to avoid corruption/mismatch
rm -f "$dest/keystore.jks"
rm -f "$dest/truststore.jks"

# ------------------------
# Node certificate
# ------------------------
openssl pkcs12 -export \
  -in "$src/node1.pem" \
  -inkey "$src/node1-key.pem" \
  -certfile "$src/root-ca.pem" \
  -out "$dest/node1-cert.p12" \
  -name node1 \
  -passout pass:$NODE1_P12_PASSWORD

keytool -importkeystore \
  -srckeystore "$dest/node1-cert.p12" \
  -srcstoretype pkcs12 \
  -srcstorepass "$NODE1_P12_PASSWORD" \
  -destkeystore "$dest/keystore.p12" \
  -deststoretype pkcs12 \
  -deststorepass "$KEYSTORE_PASSWORD" \
  -noprompt

# ------------------------
# Admin certificate
# ------------------------
openssl pkcs12 -export \
  -in "$src/admin.pem" \
  -inkey "$src/admin-key.pem" \
  -certfile "$src/root-ca.pem" \
  -out "$dest/admin-cert.p12" \
  -name admin \
  -passout pass:$ADMIN_P12_PASSWORD

keytool -importkeystore \
  -srckeystore "$dest/admin-cert.p12" \
  -srcstoretype pkcs12 \
  -srcstorepass "$ADMIN_P12_PASSWORD" \
  -destkeystore "$dest/keystore.p12" \
  -deststoretype pkcs12 \
  -deststorepass "$KEYSTORE_PASSWORD" \
  -noprompt

# ------------------------
# Client certificate
# ------------------------
openssl pkcs12 -export \
  -in "$src/client.pem" \
  -inkey "$src/client-key.pem" \
  -certfile "$src/root-ca.pem" \
  -out "$dest/client-cert.p12" \
  -name client \
  -passout pass:$CLIENT_P12_PASSWORD

keytool -importkeystore \
  -srckeystore "$dest/client-cert.p12" \
  -srcstoretype pkcs12 \
  -srcstorepass "$CLIENT_P12_PASSWORD" \
  -destkeystore "$dest/keystore.p12" \
  -deststoretype pkcs12 \
  -deststorepass "$KEYSTORE_PASSWORD" \
  -noprompt

# ------------------------
# Truststore
# ------------------------
keytool -importcert \
  -keystore "$dest/truststore.p12" \
  -storetype pkcs12 \
  -file "$src/root-ca.pem" \
  -storepass "$TRUSTSTORE_PASSWORD" \
  -alias root-ca \
  -noprompt \
  -trustcacerts