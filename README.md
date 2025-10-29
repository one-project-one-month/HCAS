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