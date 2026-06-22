#!/bin/sh
# Create pgpass file in pgAdmin's storage directory
mkdir -p /var/lib/pgadmin
echo "pgdb:5432:*:pg:${POSTGRES_DATABASE_PASSWORD}" > /var/lib/pgadmin/.pgpass
chmod 600 /var/lib/pgadmin/.pgpass

# Process servers.json
python3 << 'EOF'
import os
with open('/tmp/servers.template.json', 'r') as f:
    data = f.read()
data = data.replace('${POSTGRES_DATABASE_PASSWORD}', os.environ['POSTGRES_DATABASE_PASSWORD'])
with open('/var/lib/pgadmin/servers.json', 'w') as f:
    f.write(data)
print("servers.json created successfully")
EOF

/entrypoint.sh