 SELECT [j].[Id], [j].[CategoryId], [j].[CreatedAt], [j].[Description], [j].[EmployerId], [j].[Latitude], [j].[LocationString], [j].[Longitude], [j].[MaxSalary], [j].[MinSalary], [j].[Title], [j].[WorkType]
      FROM [Jobs] AS [j]
      WHERE [j].[CategoryId] = 1