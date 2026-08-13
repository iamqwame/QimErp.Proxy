# QimErp.Proxy — Overview

Edge composition host for the Flutter ESS app (`qimerp-ess-mobile`).

- **No database**, no Temporal workers, no tenant onboarding step
- Carter + MediatR features under `/api/mobile/...`
- JWT-forwards to IAM, People, Leave, Payroll, Performance, Workflow
- Local port: `5310`

Architecture exception: see `docs/shared/architecture-invariants.md` (edge hosts) and `docs/plans/adr-mobile-ess-edge.md`.
