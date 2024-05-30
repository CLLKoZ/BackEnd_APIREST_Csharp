using BackendAPI.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;

using BackendAPI.Services.Contrato;
using BackendAPI.Services.Implementacion;

using AutoMapper;
using BackendAPI.DTOs;
using BackendAPI.Utilidades;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbempresaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
builder.Services.AddScoped<IEmpleadoService, EmpleadoService>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPolitica", app =>
    {
        app.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#region Peticiones APIREST
app.MapGet("/departamento/lista", async (IDepartamentoService _departamentoService, IMapper _mapper) =>
{
    var listaDepartamento = await _departamentoService.GetList();
    var listaDepartamentoDTO = _mapper.Map<List<DepartamentoDTO>>(listaDepartamento);

    if (listaDepartamentoDTO.Count > 0)
        return Results.Ok(listaDepartamentoDTO);
    else
        return Results.NotFound();
});

app.MapGet("/empleado/lista", async(IEmpleadoService _empleadoService, IMapper _mapper) =>
{
    var listaEmpleado = await _empleadoService.GetList();
    var listaEmpleadoDTO = _mapper.Map<List<EmpleadoDTO>>(listaEmpleado);

    if (listaEmpleadoDTO.Count > 0)
        return Results.Ok(listaEmpleadoDTO);
    else
        return Results.NotFound();
});

app.MapPost("/empleado/crear", async (EmpleadoDTO modelo, IEmpleadoService _empleadoService, IMapper _mapper) => {
    var _empleado = _mapper.Map<Empleado>(modelo);
    var _empleadoCreado = await _empleadoService.Add(_empleado);

    if (_empleadoCreado.IdEmpleado != 0)
        return Results.Ok(_mapper.Map<EmpleadoDTO>(modelo));
    else
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
});

app.MapPut("/empleado/actualizar/{idEmpleado}", async (int idEmpleado, EmpleadoDTO modelo, IEmpleadoService _empleadoService, IMapper _mapper) => {
    var _encontrado = await _empleadoService.Get(idEmpleado);

    if (_encontrado == null)
        return Results.NotFound();

    var _empleado = _mapper.Map<Empleado>(modelo);
    _encontrado.NombreCompleto = _empleado.NombreCompleto;
    _encontrado.IdDepartamento = _empleado.IdDepartamento;
    _encontrado.Sueldo = _empleado.Sueldo;
    _encontrado.FechaContrato = _empleado.FechaContrato;

    var respuesta = await _empleadoService.Update(_encontrado);

    if (respuesta)
        return Results.Ok(_mapper.Map<EmpleadoDTO>(_encontrado));
    else
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
});

app.MapDelete("/empleado/eliminar/{idEmpleado}", async (int idEmpleado, IEmpleadoService _empleadoService) => {
    var _encontrado = await _empleadoService.Get(idEmpleado);

    if (_encontrado == null)
        return Results.NotFound();

    var respuesta = await _empleadoService.Delete(_encontrado);

    if (respuesta)
        return Results.Ok();
    else
        return Results.StatusCode(StatusCodes.Status500InternalServerError);
});
#endregion

app.UseCors("NuevaPolitica");

app.Run();

