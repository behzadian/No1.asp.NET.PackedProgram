#!/usr/bin/env bash
# Version 260604.1757
# Version 260604.1110
# Version 260604.1109
# Version 260604.1049

files=(
  "../.dockerignore"
  "../.editorconfig"
  "../.gitattributes"
  "../.gitignore"
  ".version.ps1"
  ".version.sh"
)

timestamp=$(date +"%y%m%d.%H%M")

# Add version line as second line in each file
for f in "${files[@]}"; do
    if [ -f "$f" ]; then
        head -n 1 "$f" > "$f.tmp"
        echo "# Version $timestamp" >> "$f.tmp"
        tail -n +2 "$f" >> "$f.tmp" 2>/dev/null || true
        mv "$f.tmp" "$f"
    fi
done

# Compute final hash
combined=""

for f in $(printf '%s\n' "${files[@]}" | sort); do
    # Normalize, remove comments, trim lines, remove empty lines
    clean=$(sed 's/\r$//' "$f" | grep -v '^[[:space:]]*#' | sed 's/^[[:space:]]*//;s/[[:space:]]*$//' | sed '/^$/d')

    # Hash this file
    filehash=$(echo -n "$clean" | md5sum | awk '{print toupper($1)}')
    combined="${combined}${filehash}"
done

# Final hash
finalhash=$(echo -n "$combined" | md5sum | awk '{print toupper($1)}')
formatted=$(echo "$finalhash" | sed 's/\(..\)/\1_/g; s/_$//')

echo "${timestamp}:${formatted}" >> .version
echo "${timestamp}:${formatted}"