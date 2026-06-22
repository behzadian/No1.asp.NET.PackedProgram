#!/bin/bash

# Prevent sourcing
if [[ "${BASH_SOURCE[0]}" != "${0}" ]]; then
    echo "ERROR: Do not source this script. Use: bash $0"
    exit 1
fi

export LC_ALL=C

ROOT_DIR="$(cd "$(dirname "$0")" && pwd)/../app"

echo "Root directory: $ROOT_DIR"
echo ""
echo "=== Files in hashing order ==="
echo ""

temp_file=$(mktemp)

declare -a file_list=()

# ====================== Collect Files ======================

while IFS= read -r -d '' file; do
    file_list+=("$file")
done < <(
    find "$ROOT_DIR" \
        \( -path '*/bin/*' -o -path '*/obj/*' \) -prune -o \
        -type f \
        \( -name '*.cs' -o -name '*.csproj' -o -name 'appsettings*.json' \) \
        -print0
)

# Add only root-level .slnx files
while IFS= read -r -d '' file; do
    file_list+=("$file")
done < <(
    find "$ROOT_DIR" \
        -maxdepth 1 \
        -type f \
        -name '*.slnx' \
        -print0
)

echo "Found ${#file_list[@]} matching files"
echo ""

# ====================== Hash Files ======================

printf '%s\n' "${file_list[@]}" | sort | while read -r file; do
    filename=$(basename "$file")
    hash=$(sha256sum "$file" | awk '{print $1}')

    relative_path="${file#$ROOT_DIR/}"

    echo "HASH: $hash"
    echo "FILE: $filename"
    echo "PATH: $relative_path"
    echo "--------------------------------------------------"

    echo "$hash" >> "$temp_file"
done

echo ""
echo "=== Generating final hash ==="

final_hash=$(sha256sum "$temp_file" | awk '{print $1}' | cut -c1-48)

echo "Final combined hash: $final_hash"

export BUILD_HASH="$final_hash"

rm -f "$temp_file"

echo "BUILD_HASH=$final_hash"