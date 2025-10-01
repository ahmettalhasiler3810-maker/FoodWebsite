var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.FoodWebsite>("foodwebsite");

builder.Build().Run();
