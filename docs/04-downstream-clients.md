# Downstream clients

Named `IHttpClientFactory` clients registered in `AddProxyMobileShared`:

| Client | Config key | Purpose |
|---|---|---|
| Iam | `Downstream:Iam` | login, refresh, me |
| People | `Downstream:People` | profile |
| Leave | `Downstream:Leave` | time off |
| Payroll | `Downstream:Payroll` | payslips, summary |
| Performance | `Downstream:Performance` | ess-home |
| Workflow | `Downstream:Workflow` | approvals |

Every call copies the incoming `Authorization` and `X-Correlation-Id` headers. User JWT only — never `X-Internal-Api-Key` for ESS.
