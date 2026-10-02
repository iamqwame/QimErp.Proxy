#!/usr/bin/env python3
"""Print a JSON list of root projects affected by the changes between two commits.

A root (test project or WebApi) is affected when a changed file lies inside the
directory of the root or of any project it references, directly or transitively.
Changes to repo-wide build inputs, or an unknown base commit, select every root.
"""
import argparse
import glob
import json
import os
import re
import subprocess
import sys

GLOBAL_INPUTS = re.compile(
    r"(^|/)(Directory\.(Packages|Build)\.(props|targets)|global\.json|nuget\.config)$"
    r"|^\.github/scripts/"
    , re.IGNORECASE,
)
REF = re.compile(r'<ProjectReference\s+Include="([^"]+)"', re.IGNORECASE)
ITEM = re.compile(
    r'<(?:Compile|Content|None|EmbeddedResource|AdditionalFiles)\s+(?:Include|Update)="([^"]+)"',
    re.IGNORECASE,
)


def linked_paths(csproj):
    """(path, exact) for items a project pulls in from outside its own directory."""
    try:
        with open(csproj, encoding="utf-8-sig") as f:
            text = f.read()
    except OSError:
        return set()
    base = os.path.dirname(csproj)
    paths = set()
    for inc in ITEM.findall(text):
        for part in inc.split(";"):
            part = part.strip().replace("\\", "/")
            if not part.startswith(".."):
                continue
            literal = re.split(r"[*?$]", part, maxsplit=1)[0]
            path = os.path.relpath(os.path.normpath(os.path.join(base, literal)))
            exact = literal == part and not os.path.isdir(path)
            paths.add((path, exact))
    return paths


def touches(changed, path, exact):
    return changed == path if exact else changed.startswith(path)


def project_refs(csproj, cache):
    if csproj in cache:
        return cache[csproj]
    cache[csproj] = set()
    try:
        with open(csproj, encoding="utf-8-sig") as f:
            text = f.read()
    except OSError:
        return cache[csproj]
    base = os.path.dirname(csproj)
    refs = set()
    for inc in REF.findall(text):
        ref = os.path.relpath(os.path.normpath(os.path.join(base, inc.replace("\\", "/"))))
        refs.add(ref)
        refs |= project_refs(ref, cache)
    cache[csproj] = refs
    return refs


def changed_files(base, head):
    if not base or set(base) == {"0"}:
        return None
    try:
        out = subprocess.run(
            ["git", "diff", "--name-only", f"{base}...{head}"],
            check=True, capture_output=True, text=True,
        ).stdout
    except subprocess.CalledProcessError:
        return None
    return [line for line in out.splitlines() if line]


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--base", default="")
    ap.add_argument("--head", default="HEAD")
    ap.add_argument("--roots", required=True, help="glob for root .csproj files")
    ap.add_argument("--always", action="append", default=[],
                    help="path prefix whose change selects every root (repeatable)")
    args = ap.parse_args()

    roots = sorted(glob.glob(args.roots, recursive=True))
    files = changed_files(args.base, args.head)
    if files is None or any(
        GLOBAL_INPUTS.search(f) or any(f.startswith(p) for p in args.always) for f in files
    ):
        json.dump(roots, sys.stdout)
        return

    cache = {}
    affected = []
    for root in roots:
        projects = project_refs(root, cache) | {root}
        inputs = {(os.path.dirname(p) + "/", False) for p in projects}
        for p in projects:
            inputs |= linked_paths(p)
        if any(touches(f, path, exact) for f in files for path, exact in inputs):
            affected.append(root)
    json.dump(affected, sys.stdout)


if __name__ == "__main__":
    main()
