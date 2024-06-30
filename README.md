# TestTask-HOCOOS
### Following points were achieved:

#### General
- Add support for Dependency Injection (Microsoft.Extensions.DependencyInjection).
- Replace Console.WriteLine with calls to ILogger methods (Microsoft.Extensions.Logging).
- You can modify any code except the ExpressionBackendTask and ThreadBackendTask classes.

#### ThreadBackendTask
- Inherit from the ThreadBackendTask class.
- Limit the number of threads to 5.

#### ExpressionBackendTask
- Inherit from the ExpressionBackendTask class.
- Limit data return to only the specified list of fields.
