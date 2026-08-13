# Module architecture

```
QimErp.Proxy/
  src/Modules/Mobile/
    QimErp.Proxy.Mobile.Shared/   # constants, DownstreamOptions, Http clients, contracts
    QimErp.Proxy.Mobile.WebApi/   # Features/, Program.cs
    QimErp.Proxy.Mobile.WebApi.Tests/
```

Shared owns outbound clients. WebApi owns one-file-per-HTTP-operation features. No `*DbContext`, `Activities/`, or `Workflows/`.
