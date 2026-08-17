#!/usr/bin/env bash
set -euo pipefail

ROOT="/Users/iamqwame/MyCode/QimErp-Limited/current-sprint/QimErp.Proxy"
PROJECT_ID="aa74d071-08a8-4921-b6f3-26ee184518ec"
ENV_NAME="Development"
SERVICE="qimerp-proxy-mobile-api"
DOCKERFILE_PATH="src/Modules/Mobile/QimErp.Proxy.Mobile.WebApi/Dockerfile"
SOURCE_SERVICE="qimerp-iam-core-api"

cd "$ROOT"
export RAILWAY_PROJECT_ID="$PROJECT_ID"

echo "Railway CLI: $(railway --version 2>/dev/null || true)"
railway whoami

if ! railway service list --json 2>/dev/null | jq -e --arg s "$SERVICE" '.[] | select(.name == $s)' >/dev/null; then
  echo "Creating service ${SERVICE}"
  railway add --service "$SERVICE" --json || railway service create "$SERVICE" || true
fi

copy_var() {
  local name="$1"
  local value
  value="$(railway variable list --service "$SOURCE_SERVICE" --environment "$ENV_NAME" --kv 2>/dev/null | awk -F= -v n="$name" '$1==n {print substr($0, index($0,"=")+1)}' | tail -n 1)"
  if [ -z "$value" ]; then
    value="$(railway variable list --service "$SOURCE_SERVICE" --environment "$ENV_NAME" --json | jq -r --arg n "$name" '.[] | select(.name==$n) | .value // empty')"
  fi
  if [ -n "$value" ]; then
    railway variable set "${name}=${value}" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys >/dev/null
    echo "Set ${name}"
  else
    echo "Missing source var ${name} on ${SOURCE_SERVICE}"
  fi
}

for key in \
  Jwt__Issuer \
  Jwt__Audience \
  Jwt__Secret \
  Jwt__ExpirationMinutes \
  RedisCache__ConnectionString \
  RedisCache__Database \
  RedisCache__ConnectTimeout \
  RedisCache__SyncTimeout \
  RedisCache__AbortOnConnectFail \
  Cors__AllowedOrigins__0 \
  Cors__AllowedOrigins__1
do
  copy_var "$key"
done

railway variable set "ASPNETCORE_ENVIRONMENT=Production" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "ASPNETCORE_URLS=http://0.0.0.0:\${PORT}" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "RAILWAY_DOCKERFILE_PATH=${DOCKERFILE_PATH}" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "Downstream__Iam=https://qimerp-iam-core-api-development.up.railway.app" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "Downstream__People=https://corehr-employee-webapi-development.up.railway.app" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "Downstream__Leave=https://hroperations-leave-webapi-development.up.railway.app" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "Downstream__Payroll=https://qimerp-payroll-core-api-development.up.railway.app" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "Downstream__Performance=https://corehr-performance-api-development.up.railway.app" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "Downstream__Workflow=https://platform-workflow-webapi-development.up.railway.app" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "Downstream__Benefit=https://hroperations-benefit-api-development.up.railway.app" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "Downstream__Surveys=https://hroperations-surveys-webapi-development.up.railway.app" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "Downstream__Notifications=https://platform-notifications-webapi-development.up.railway.app" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys
railway variable set "Downstream__TimeoutSeconds=90" --service "$SERVICE" --environment "$ENV_NAME" --skip-deploys

echo "Generating domain"
railway domain --service "$SERVICE" --environment "$ENV_NAME" || true

echo "Deploying"
railway up --service "$SERVICE" --environment "$ENV_NAME" --ci --detach

for i in $(seq 1 60); do
  STATUS="$(railway deployment list --service "$SERVICE" --environment "$ENV_NAME" --limit 1 --json | jq -r '.[0].status // empty')"
  echo "Attempt ${i}/60: ${STATUS}"
  case "${STATUS}" in
    SUCCESS) break ;;
    FAILED|CRASHED|REMOVED|SKIPPED) echo "Deploy failed: ${STATUS}"; exit 1 ;;
  esac
  sleep 10
done

DOMAIN_JSON="$(railway domain --service "$SERVICE" --environment "$ENV_NAME" --json || true)"
URL="$(printf '%s' "$DOMAIN_JSON" | jq -r '.domains[0] // .domain // empty')"
echo "URL=${URL}"

if [ -n "$URL" ]; then
  for i in $(seq 1 15); do
    if curl -fsS --max-time 10 "${URL%/}/ready"; then
      echo
      echo "READY_OK ${URL}"
      exit 0
    fi
    sleep 8
  done
fi

echo "Health check did not pass"
exit 1
