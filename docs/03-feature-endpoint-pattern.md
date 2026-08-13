# Feature endpoint pattern

Same as CoreHR gold standard:

1. Static feature class with `Query`/`Command` + `Handler`
2. `ICarterModule` endpoint: bind → `sender.Send` → `ToIResult()`
3. Routes from `MobileApiConstants.Url.*`
4. FluentValidation for auth bodies; passthrough features forward JSON body as-is
