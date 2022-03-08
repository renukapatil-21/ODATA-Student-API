using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.OData.Batch;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using ODataStudent.Data;
using ODataStudent.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
static IEdmModel GetEdmModel()
{
    ODataConventionModelBuilder builder = new();
    builder.EntitySet<Student>("Students");
    return builder.GetEdmModel();
}

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<StudentDataContext>(x =>
{
    x.UseSqlServer(connectionString);
});

var batchHandler = new DefaultODataBatchHandler();
builder.Services.AddControllers().AddOData(opt => opt.AddRouteComponents("v1", GetEdmModel(), batchHandler).Filter().Select().Expand().OrderBy().SetMaxTop(25));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ODataStudents", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseODataBatching();

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
