# Load .env file
Get-Content .env | ForEach-Object {
    if ($_ -match "^\s*([^#=]+)\s*=\s*(.*)\s*$") {
        [System.Environment]::SetEnvironmentVariable($matches[1], $matches[2])
    }
}

# Directories
$src = ".tls"
$dest = ".ks"

# Create output directory
New-Item -ItemType Directory -Force -Path $dest | Out-Null

# ------------------------
# Node certificate
# ------------------------
openssl pkcs12 -export `
  -in "$src\node1.pem" `
  -inkey "$src\node1-key.pem" `
  -certfile "$src\root-ca.pem" `
  -out "$dest\node1-cert.p12" `
  -name node1 `
  -passout "pass:$env:NODE1_P12_PASSWORD"

keytool -importkeystore `
  -srckeystore "$dest\node1-cert.p12" `
  -srcstoretype pkcs12 `
  -srcstorepass "$env:NODE1_P12_PASSWORD" `
  -destkeystore "$dest\keystore.jks" `
  -deststoretype jks `
  -deststorepass "$env:KEYSTORE_PASSWORD" `
  -noprompt

# ------------------------
# Admin certificate
# ------------------------
openssl pkcs12 -export `
  -in "$src\admin.pem" `
  -inkey "$src\admin-key.pem" `
  -certfile "$src\root-ca.pem" `
  -out "$dest\admin-cert.p12" `
  -name admin `
  -passout "pass:$env:ADMIN_P12_PASSWORD"

keytool -importkeystore `
  -srckeystore "$dest\admin-cert.p12" `
  -srcstoretype pkcs12 `
  -srcstorepass "$env:ADMIN_P12_PASSWORD" `
  -destkeystore "$dest\keystore.jks" `
  -deststoretype jks `
  -deststorepass "$env:KEYSTORE_PASSWORD" `
  -noprompt

# ------------------------
# Truststore
# ------------------------
keytool -importcert `
  -keystore "$dest\truststore.jks" `
  -file "$src\root-ca.pem" `
  -storepass "$env:TRUSTSTORE_PASSWORD" `
  -alias root-ca `
  -noprompt `
  -trustcacerts