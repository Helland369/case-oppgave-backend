#!/usr/bin/env bash

set -euo pipefail

INPUT_FILE="events.jsonl"
API_URL="http://localhost:5091/events"

current_student_id=""

while IFS= read -r line; do
  
  [[ -z "$line" ]] && continue

  event_type=$(echo "$line" | jq -r '.type')

  if [[ "$event_type" == "student_registrert" ]]; then
    echo "Registrerer student..."

    response=$(curl -s -X POST "$API_URL" \
      -H "Content-Type: application/json" \
      -d "$line")

    current_student_id=$(echo "$response" | jq -r '.studentId')

    if [[ "$current_student_id" == "null" || -z "$current_student_id" ]]; then
      echo "FEIL: fikk ikke studentId"
      echo "Response: $response"
      exit 1
    fi

    echo "→ studentId = $current_student_id"
  else
    if [[ -z "$current_student_id" ]]; then
      echo "FEIL: event før student_registrert"
      echo "$line"
      exit 1
    fi

    event_with_student=$(echo "$line" | jq \
      --arg sid "$current_student_id" \
      '. + {studentId: $sid}')

    echo "Poster event ($event_type) for student $current_student_id"

    curl -s -X POST "$API_URL" \
      -H "Content-Type: application/json" \
      -d "$event_with_student" > /dev/null
  fi

done < "$INPUT_FILE"

echo "Import ferdig."
