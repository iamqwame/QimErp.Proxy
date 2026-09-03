#!/usr/bin/env bash
set -euo pipefail

REPO="iamqwame/QimErp.Proxy"
ENV_NAME="development"
SOURCE_REPO="iamqwame/QimErp.IAM"

gh api "repos/${REPO}/environments/${ENV_NAME}" -X PUT -f wait_timer=0 >/dev/null

TOKEN="$(gh api "repos/${SOURCE_REPO}/environments/${ENV_NAME}/variables/RAILWAY_TOKEN" --jq '.value' 2>/dev/null || true)"
if [ -z "${TOKEN}" ]; then
  TOKEN="$(gh variable get RAILWAY_TOKEN --repo "${SOURCE_REPO}" --env "${ENV_NAME}" 2>/dev/null || true)"
fi
if [ -z "${TOKEN}" ]; then
  echo "RAILWAY_TOKEN not found on ${SOURCE_REPO} ${ENV_NAME}"
  exit 1
fi

set_var() {
  local name="$1"
  local value="$2"
  if gh api "repos/${REPO}/environments/${ENV_NAME}/variables/${name}" >/dev/null 2>&1; then
    gh api "repos/${REPO}/environments/${ENV_NAME}/variables/${name}" -X PATCH -f value="${value}" >/dev/null
    echo "Updated ${name}"
  else
    gh api "repos/${REPO}/environments/${ENV_NAME}/variables" -X POST -f name="${name}" -f value="${value}" >/dev/null
    echo "Created ${name}"
  fi
}

set_var RAILWAY_TOKEN "${TOKEN}"
set_var RAILWAY_PROJECT_ID "aa74d071-08a8-4921-b6f3-26ee184518ec"
set_var RAILWAY_ENVIRONMENT "Development"
set_var RAILWAY_PROXY_MOBILE_WEBAPI_SERVICE "qimerp-proxy-mobile-api"

echo "--- ${REPO} ${ENV_NAME} variables ---"
gh api "repos/${REPO}/environments/${ENV_NAME}/variables" --jq '.variables[] | .name'
echo "DONE"

