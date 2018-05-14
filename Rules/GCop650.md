# GCop 650

> *"If this is an Api controller, instead of [Authorize] use [AuthorizeApi] attribute."*

## Rule description

Authorization is deciding whether a user is allowed to perform an action. Web API provides a built-in authorization filter, `AuthorizeAttribute`. This filter checks whether the user is authenticated. If not, it returns HTTP status code 401 (Unauthorized), without invoking the action.

## Example

```csharp
[Authorize]
public class ApiController : BaseController
{
    ...
}
```

*should be* 🡻

```csharp
[AuthorizeApi]
public class ApiController : BaseController
{
    ...
}
```