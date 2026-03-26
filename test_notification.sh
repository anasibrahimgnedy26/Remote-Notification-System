#!/bin/bash
curl -s -w '\nHTTP_STATUS: %{http_code}\nTIME_TOTAL: %{time_total}s\n' \
  -X POST http://localhost:5000/api/notification/send \
  -H 'Content-Type: application/json' \
  -d '{"title":"MTU Fix Test","body":"Testing after MTU 1300 fix","targetType":"All"}'
