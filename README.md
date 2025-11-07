```
git rm -r --cached .
git add .
git commit -am 'fix: git cache cleared'
git push
```

```
dotnet tool install --global dotnet-ef
``` 
   
```
dotnet ef dbcontext scaffold "Server=.;Database=HealthCareDB;User ID=sa;Password=sasa@123;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -o AppDbContextModels -c AppDbContext -f
```


HCAS.Api    => Web API + Mobile
HCAS.WebApp => Blazor Web App (UI)


HCAS.WebApp => HCAS.Domain (BL) => HCAS.Database
HCAS.Api    => HCAS.Domain (BL) => HCAS.Database 

HCAS.Domain => Features => Services (Models - DTO (Request, Response), Service)

DTO - Data Transfer Object

HCAS.WebApp => HCAS.Api => HCAS.Domain (BL) => HCAS.Database
