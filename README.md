# 4. Semester Projekt

This is the backend for FLUXETL:
- ASP.NET Core WebAPI
- C#
- Apache Kafka
- Docker
  
#Run backend:

To run the backend:
1. Fork the repository.
2. Navigate to the solution folder of ETL-Project.
3. Run `docker-compose up`.
4. Wait ~ 10 sec. to make sure Kafka is running and run `create-topics.ps1`.
5. Set startup projects to include:
   - ETLConfig.API
   - ExtractAPI
   - Transform
   - Load
6. Run.

#Connectors:

Currently supports reading from:
- API
- Excel
- MSSQL

and writing to:
- MSSQL
- MySQL
- Excel

To add more connectors:
1. Create a class that implements either ISourceProvider or ITargetWriter, in ExtractAPI or Load.
2. In ETLDomain add a SourceInfo class in Sources or Targets folder. Eg. ExcelSourceInfo or PostgreSQLTargetInfo
   that inherits from either File-, Db- or APISourceBaseInfo (similar in targets).
3. Add the class in SourceInfoBase or TargetInfoBase.
[JsonDerivedType(typeof(ExcelSourceInfo), "excel")]

4. The system automatically registers the new connector.

