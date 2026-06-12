# Tenant onboarding and operations

The application uses a shared application/shared SQL database model. A request is resolved to a tenant by the exact, verified host in `TenantDomains`. Development and test environments may fall back to `Tenancy:DefaultTenantId`; production disables that fallback by default.

## Deployment sequence

1. Apply `20260612120000_AddTenantOnboardingAndIsolation` before enabling strict host resolution.
2. Insert and verify the production host for the legacy/default tenant, or temporarily enable `Tenancy:AllowDefaultTenant` for the first platform-administrator login.
3. Sign in as a `SuperAdmin` in the default tenant. The login receives the `PlatformAdmin` claim required by the tenant administration API.
4. Create each tenant through `POST /api/tenant-administration`.
5. Point the tenant DNS name at the application. Host resolution starts working as soon as the onboarding transaction commits.
6. Disable default-tenant fallback in every shared production environment.

`Database:ApplyMigrationsOnStartup` is intentionally `false` by default. Enable it only where the deployment identity is allowed to alter the database; otherwise run EF migrations in the release pipeline.

## Create request

```json
{
  "slug": "example-college",
  "legalName": "Example College (Pty) Ltd",
  "appTitle": "Example College Online",
  "hostName": "portal.example.edu",
  "adminFirstName": "Tenant",
  "adminLastName": "Administrator",
  "adminEmail": "admin@example.edu",
  "adminPassword": "replace-with-a-strong-secret",
  "planName": "Standard",
  "startsOn": "2026-06-12T00:00:00Z",
  "expiresOn": "2027-06-12T00:00:00Z",
  "gracePeriodDays": 7,
  "maxUsers": 50,
  "maxStudents": 1000,
  "contactEmail": "support@example.edu",
  "billingContactEmail": "billing@example.edu"
}
```

Onboarding creates the profile, verified primary domain, subscription, and initial tenant administrator in one SQL transaction. Duplicate slugs, hosts, and administrator email addresses are rejected.

## Administration endpoints

All endpoints require the `PlatformAdmin` policy.

- `POST /api/tenant-administration`
- `POST /api/tenant-administration/{tenantId}/renew`
- `PUT /api/tenant-administration/{tenantId}/active`
- `PUT /api/tenant-administration/{tenantId}/profile`
- `PUT /api/tenant-administration/{tenantId}/subscription-status`
- `GET /api/tenant-administration/{tenantId}/usage`

## Isolation rules

- Authentication cookies carry `TenantId` and `TenantSlug` claims.
- A cookie issued for one tenant receives HTTP 403 on another tenant host.
- EF query filters isolate `Users` and `Academics.Students`.
- Added users/students are stamped with the request tenant when no tenant ID is supplied.
- Cross-tenant updates and deletes of tenant-owned records are rejected.
- Active user/student quotas are checked during `SaveChangesAsync`.
- File uploads inherit the resolved tenant automatically, allowing tenant-specific storage settings.
- Branding and subscription caches are keyed by tenant ID.

When another aggregate becomes independently queryable across tenants, make its root implement `ITenantOwned`, add its migration/backfill, and register a tenant query filter before exposing it to tenant-facing code.
