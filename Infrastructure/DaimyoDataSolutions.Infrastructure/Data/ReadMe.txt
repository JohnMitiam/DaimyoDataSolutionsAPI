*on the root (new Table Migration)

dotnet ef migrations add ProductImagesMigration --project Infrastructure/DaimyoDataSolutions.Infrastructure --startup-project Presentation/DaimyoDataSolutions.API


*add Each SPs on the latest migration file before Update
dotnet ef database update --project Infrastructure/DaimyoDataSolutions.Infrastructure --startup-project Presentation/DaimyoDataSolutions.API