assessment Paragin

#To run:

dotnet run --project Paragin.ExamAnalyzer.Cli -- --input "Documents\Developer Assignment Sample_Data.xlsx" --output "C:\Temp\results2.csv"

or

dotnet run --project Paragin.ExamAnalyzer.Cli -- -i "Documents\Developer Assignment Sample_Data.xlsx" -o "C:\Temp\results.csv"


#to get the analytics in a different file/path:

dotnet run --project Paragin.ExamAnalyzer.Cli -- -i "Documents\Developer Assignment Sample_Data.xlsx" -o "C:\Temp\results.csv" -a "C:\Temp\analytics.csv"