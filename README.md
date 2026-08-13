# QimErp.Proxy

Edge composition host (backend-for-frontend) for the Flutter ESS app (`qimerp-ess-mobile`). Exposes a single `/api/mobile/...` surface that forwards the incoming user JWT to the platform's real backend modules — it holds no database of its own and runs no Temporal workers.

- **No database**, no Temporal workers, no tenant onboarding step
- Carter + MediatR features, one file per HTTP operation
- Forwards the caller's `Authorization` + `X-Correlation-Id` headers to IAM, People, Leave, Payroll, Performance, and Workflow
- Local port: `5310`

Architecture exception: this module intentionally breaks the platform's "no direct inter-module HTTP" rule since it exists purely to compose those modules for the mobile client. See [`../docs/shared/architecture-invariants.md`](../docs/shared/architecture-invariants.md) (edge hosts) and `../docs/plans/adr-mobile-ess-edge.md`.

## Stack & shared standards

- Authoritative stack, module boundaries, tenant isolation, error handling, code quality, observability, deployment, git branching, testing, and security: see [`../docs/shared/`](../docs/shared/)
- Platform doc index: [`../docs/shared/MODULES.md`](../docs/shared/MODULES.md)

## Module documentation (`docs/`)

| Doc | Topic |
| --- | --- |
| [01-overview.md](docs/01-overview.md) | Bounded context, module map |
| [02-module-architecture.md](docs/02-module-architecture.md) | Solution layout, Shared/WebApi split |
| [03-feature-endpoint-pattern.md](docs/03-feature-endpoint-pattern.md) | How to write an endpoint |
| [04-downstream-clients.md](docs/04-downstream-clients.md) | Named `HttpClient`s per downstream module |

## Solution layout

```
QimErp.Proxy/
  src/Modules/Mobile/
    QimErp.Proxy.Mobile.Shared/     # constants, DownstreamOptions, downstream HTTP clients, contracts
    QimErp.Proxy.Mobile.WebApi/     # Features/ (Auth, Home, Profile, Payslips, Payroll, TimeOff,
                                     #   Performance, Benefits, Compensation, People, Surveys,
                                     #   Approvals, Notifications, Feed, Security, Me), Program.cs
    QimErp.Proxy.Mobile.WebApi.Tests/
```

`Shared` owns the outbound downstream clients; `WebApi` owns one-file-per-HTTP-operation features. There is no `*DbContext`, `Activities/`, or `Workflows/` in this module — see [`docs/02-module-architecture.md`](docs/02-module-architecture.md).

## Downstream modules

Every request's `Authorization` and `X-Correlation-Id` headers are copied through as-is — user JWT only, never an internal service key, since every call is made on behalf of the signed-in mobile user.

| Client | Config key | Purpose |
| --- | --- | --- |
| Iam | `Downstream:Iam` | login, refresh, current user, tenant activities |
| People | `Downstream:People` | profile, education, documents, next of kin, emergency contacts |
| Leave | `Downstream:Leave` | time off, holidays, travel permissions |
| Payroll | `Downstream:Payroll` | payslips, compensation, claims, advances, loans |
| Performance | `Downstream:Performance` | goals, reviews, check-ins, feedback 360, appraisal plans |
| Workflow | `Downstream:Workflow` | pending approvals |

## Quick start

```bash
dotnet run --project src/Modules/Mobile/QimErp.Proxy.Mobile.WebApi
```

Requires the downstream modules (IAM, CoreHr/People, HROperations/Leave, Payroll, Platform/Workflow) running locally on the ports configured in `appsettings.Development.json`. Health checks are exposed at `/health` and `/ready`.

## Testing

```bash
dotnet test src/Modules/Mobile/QimErp.Proxy.Mobile.WebApi.Tests
```
