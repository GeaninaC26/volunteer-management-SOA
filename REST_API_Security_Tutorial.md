# Comprehensive REST API Security Tutorial

## Table of Contents
1. [Introduction](#introduction)
2. [Authentication vs Authorization](#authentication-vs-authorization)
3. [Authentication Schemes](#authentication-schemes)
4. [Setting Up Multiple Authentication Methods](#setting-up-multiple-authentication-methods)
5. [Authorization Policies](#authorization-policies)
6. [Securing Endpoints](#securing-endpoints)
7. [Best Practices](#best-practices)
8. [Complete Implementation Example](#complete-implementation-example)

---

## Introduction

Securing a REST API is critical to protect sensitive data and ensure only authorized users can access specific resources. This tutorial demonstrates a multi-layered security approach using ASP.NET Core, implementing:

- **JWT (JSON Web Token) Bearer authentication** for API clients
- **Google OAuth** for social login
- **Cookie-based authentication** for web applications
- **Custom authorization policies** for role-based access control

---

## Authentication vs Authorization

### Authentication
**Authentication** answers: *"Who are you?"*
- Verifies the identity of a user
- Validates credentials (tokens, cookies, OAuth providers)
- Establishes user identity

### Authorization
**Authorization** answers: *"What can you do?"*
- Determines what an authenticated user can access
- Enforces permissions and roles
- Controls access to specific resources or operations

---

## Authentication Schemes

### 1. JWT Bearer Authentication

JWT is ideal for **stateless API authentication**, commonly used by mobile apps and SPAs.

**How it works:**
1. Client sends credentials to authentication server
2. Server validates and returns a JWT token
3. Client includes token in `Authorization: Bearer <token>` header
4. Server validates token on each request

**Configuration:**

```csharp
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Authority = builder.Configuration
        .GetSection("Authentication:Schemes:Bearer:Authority").Value;
    options.Audience = builder.Configuration
        .GetSection("Authentication:Schemes:Bearer:Audience").Value;
})
```

**appsettings.json:**
```json
{
  "Authentication": {
    "Schemes": {
      "Bearer": {
        "Authority": "https://your-auth-server.com",
        "Audience": "your-api-identifier"
      }
    }
  }
}
```

**Key Properties:**
- `Authority`: The issuer of the token (e.g., Auth0, IdentityServer, Azure AD)
- `Audience`: Who the token is intended for (your API identifier)

---

### 2. Google OAuth Authentication

OAuth allows users to **authenticate using third-party providers** without sharing passwords.

**Configuration:**

```csharp
.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
{
    options.ClientId = builder.Configuration
        .GetSection("Authentication:Google:ClientId").Value ?? "";
    options.ClientSecret = builder.Configuration
        .GetSection("Authentication:Google:ClientSecret").Value ?? "";
})
```

**appsettings.json:**
```json
{
  "Authentication": {
    "Google": {
      "ClientId": "your-google-client-id.apps.googleusercontent.com",
      "ClientSecret": "your-google-client-secret"
    }
  }
}
```

**Setup Steps:**
1. Create a project in [Google Cloud Console](https://console.cloud.google.com)
2. Enable Google+ API
3. Create OAuth 2.0 credentials
4. Configure authorized redirect URIs
5. Copy ClientId and ClientSecret to configuration

---

### 3. Cookie-Based Authentication

Cookies work well for **traditional web applications** where the browser manages session state.

**Configuration:**

```csharp
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "auth_cookie";
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
})
```

**Key Features:**
- `SlidingExpiration`: Refreshes cookie lifetime on each request
- `ExpireTimeSpan`: How long the cookie remains valid
- `LoginPath`: Redirect URL for unauthenticated users
- `LogoutPath`: Endpoint to clear authentication

---

## Setting Up Multiple Authentication Methods

### Configuring the Authentication Chain

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddJwtBearer(/* ... */)
.AddCookie(/* ... */)
.AddGoogle(/* ... */);
```

**Why Multiple Schemes?**
- **Web UI users**: Use Google OAuth → Cookie
- **API clients**: Use JWT tokens
- **Flexibility**: Different clients can authenticate differently

### Middleware Order Matters

```csharp
app.UseAuthentication();  // Must come first
app.UseAuthorization();   // Then authorization
```

**Critical:** Authentication must be configured before authorization, and both must be added to the pipeline before endpoints are mapped.

---

## Authorization Policies

### Default Fallback Policy

Require authentication for **all endpoints by default**:

```csharp
builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(
            CookieAuthenticationDefaults.AuthenticationScheme, 
            JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser()
        .Build())
```

**What this does:**
- Any endpoint without explicit authorization requires authentication
- Accepts both Cookie and JWT authentication
- Secure by default (fail-safe approach)

---

### Custom Role-Based Policy: Admin Only

```csharp
.AddPolicy("AdminOnly", policy => {
    policy.AddAuthenticationSchemes(
        CookieAuthenticationDefaults.AuthenticationScheme, 
        JwtBearerDefaults.AuthenticationScheme);
    policy.RequireAssertion(context => {
        var adminEmails = builder.Configuration
            .GetSection("Authentication:AdminEmails").Get<List<string>>();
        
        if (adminEmails == null || adminEmails.Count == 0)
            return false;
        
        var emailClaim = context.User.Claims.FirstOrDefault(c => 
            c.Type.Contains("emailaddress"));
        
        return emailClaim != null && adminEmails.Contains(emailClaim.Value);
    });
})
```

**Configuration:**
```json
{
  "Authentication": {
    "AdminEmails": [
      "admin@example.com",
      "superuser@example.com"
    ]
  }
}
```

**How it works:**
1. Loads list of admin emails from configuration
2. Extracts email claim from authenticated user
3. Checks if user's email is in the admin list
4. Grants/denies access based on match

---

### Public Access Policy

For endpoints that should be **accessible without authentication**:

```csharp
.AddPolicy("Public", policy => policy.RequireAssertion(_ => true));
```

---

## Securing Endpoints

### Applying Authorization Policies

#### 1. Allow Anonymous Access

```csharp
app.MapGet("/api/type/gender", (HttpContext context) =>
{
    return Enum.GetValues(typeof(Gender)).Cast<Gender>();
})
.AllowAnonymous();  // Override fallback policy
```

Use for:
- Public reference data
- Health check endpoints
- Login/registration pages

---

#### 2. Require Specific Policy

```csharp
app.MapPost("/api/admin/users", (UserDto user) =>
{
    // Create user logic
})
.RequireAuthorization("AdminOnly");
```

---

#### 3. Use Default Authentication

```csharp
app.MapGet("/api/volunteers", () =>
{
    // Get volunteers logic
});
// No attribute = uses fallback policy (requires authentication)
```

---

### Authentication Flow Endpoints

#### Login Endpoint

```csharp
app.MapGet("/login", (HttpContext context) =>
{
    return Results.Challenge(
        new AuthenticationProperties { RedirectUri = "/" }, 
        [GoogleDefaults.AuthenticationScheme]
    );
}).AllowAnonymous();
```

**Flow:**
1. User navigates to `/login`
2. Redirected to Google OAuth
3. After authentication, redirected back to app
4. Cookie is set for subsequent requests

---

#### Logout Endpoint

```csharp
app.MapGet("/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});
```

**Actions:**
- Clears authentication cookie
- Ends user session
- Redirects to home page

---

#### User Info Endpoint

```csharp
app.Map("/userinfo", (HttpContext context) =>
{
    var user = context.User;
    if (user?.Identity?.IsAuthenticated ?? false)
    {
        var claims = user.Claims.ToDictionary(
            c => c.Type.Split("/").Last(), 
            c => c.Value
        );
        return Results.Json(claims);
    }
    else
    {
        return Results.Unauthorized();
    }
}).AllowAnonymous();
```

**Purpose:**
- Returns authenticated user's information
- Useful for frontend to display user data
- Returns claims (name, email, etc.)

---