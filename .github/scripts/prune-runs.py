#!/usr/bin/env python3
"""Delete this repository's completed workflow runs older than KEEP_DAYS (deletes their logs and artifacts)."""
import datetime
import json
import os
import sys
import urllib.error
import urllib.request

API = os.environ.get("GITHUB_API_URL", "https://api.github.com")
REPO = os.environ["GITHUB_REPOSITORY"]
TOKEN = os.environ["GH_TOKEN"]
KEEP_DAYS = int(os.environ.get("KEEP_DAYS", "30"))
SELF_RUN = os.environ.get("GITHUB_RUN_ID", "")


def call(method, path):
    req = urllib.request.Request(
        f"{API}{path}",
        method=method,
        headers={
            "Authorization": f"Bearer {TOKEN}",
            "Accept": "application/vnd.github+json",
            "X-GitHub-Api-Version": "2022-11-28",
        },
    )
    with urllib.request.urlopen(req, timeout=30) as resp:
        body = resp.read()
    return json.loads(body) if body else None


cutoff = (datetime.datetime.now(datetime.timezone.utc) - datetime.timedelta(days=KEEP_DAYS)).date()
ids = []
page = 1
while True:
    data = call("GET", f"/repos/{REPO}/actions/runs?status=completed&created=<{cutoff}&per_page=100&page={page}")
    runs = data.get("workflow_runs", [])
    ids += [r["id"] for r in runs if str(r["id"]) != SELF_RUN]
    if len(runs) < 100:
        break
    page += 1

failed = 0
for run_id in ids:
    try:
        call("DELETE", f"/repos/{REPO}/actions/runs/{run_id}")
    except urllib.error.HTTPError as e:
        failed += 1
        print(f"could not delete run {run_id}: HTTP {e.code}", file=sys.stderr)

print(f"deleted {len(ids) - failed} runs created before {cutoff}; {failed} failed")
sys.exit(1 if failed else 0)
