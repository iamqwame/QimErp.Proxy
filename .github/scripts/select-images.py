#!/usr/bin/env python3
"""Filter the CATALOGUE env (JSON list of {service, project, image}) to a build matrix.

SERVICES (space- or comma-separated) wins when set; otherwise entries whose project
is in AFFECTED (JSON list of .csproj paths) are kept. Unknown services are an error.
"""
import json
import os
import re
import sys

catalogue = json.loads(os.environ["CATALOGUE"])
requested = [s for s in re.split(r"[\s,]+", os.environ.get("SERVICES", "")) if s]

if requested:
    known = {e["service"] for e in catalogue}
    unknown = [s for s in requested if s not in known]
    if unknown:
        sys.exit(f"Unknown services: {' '.join(unknown)}. Allowed: {' '.join(sorted(known))}")
    selected = [e for e in catalogue if e["service"] in requested]
else:
    affected = set(json.loads(os.environ.get("AFFECTED", "[]")))
    selected = [e for e in catalogue if e["project"] in affected]

for e in selected:
    e.setdefault("dockerfile", os.path.join(os.path.dirname(e["project"]), "Dockerfile"))
json.dump(selected, sys.stdout, separators=(",", ":"))
