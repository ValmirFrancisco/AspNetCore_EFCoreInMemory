using AspNetCore_EFCoreInMemory.Helpers;
using AutoMapper;
using CrossCutting.Mappings;
using Data.Context;
using Data.Repository;
using Domain.Entities;
using Domain.Interfaces.Repositories;
using Domain.Interfaces.Services.Post;
using Domain.Interfaces.Services.Usuario;
using Microsoft.EntityFrameworkCore;
using Service.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase("InMemory"), ServiceLifetime.Singleton);

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddTransient<IUsuarioService, UsuarioService>();
builder.Services.AddTransient<IPostService, PostService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

var config = new AutoMapper.MapperConfiguration(cfg =>
{
    cfg.AddProfile(new DtoToModelProfile());
    cfg.AddProfile(new EntityToDtoProfile());
    cfg.AddProfile(new ModelToEntityProfile());
});

IMapper mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers().AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var context = app.Services.GetService<ApiContext>();
HelpContext.Configure(app.Services.GetRequiredService<IHttpContextAccessor>(), app.Configuration, context);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

AdicionarDadosTeste(HelpContext.ApiContext);

app.Run();

static void AdicionarDadosTeste(ApiContext context)
{
    List<UsuarioEntity> usuarios = new List<UsuarioEntity>();
    var usr1 = new UsuarioEntity
    {
        CreateAt = DateTime.Now,
        Nome = "Araribóia Clayderman",
        Email = "arariboia.claydeman@email.com"
    };
    var usr2 = new UsuarioEntity
    {
        CreateAt = DateTime.Now,
        Nome = "Asdrúbal Moncorvo Filho",
        Email = "asdrubal.moncorvo@email.com"
    };
    var usr3 = new UsuarioEntity
    {
        CreateAt = DateTime.Now,
        Nome = "Washington Ajefferson Clas Clay Souza",
        Email = "washington.clay@aol.com"
    };
    usuarios.Add(usr1);
    usuarios.Add(usr2);
    usuarios.Add(usr3);
    context.Usuarios.AddRange(usuarios);
    var pst1 = new PostEntity
    {
        UsuarioId = usr1.Id,
        Conteudo = $"Primeiro Post(post1) do Usuario : {usr1.Nome}",
        CreateAt = DateTime.Now
    };
    context.Posts.Add(pst1);
    var pst2 = new PostEntity
    {
        UsuarioId = usr1.Id,
        Conteudo = $"Segundo Post(post1) do Usuario : {usr1.Nome}",
        CreateAt = DateTime.Now
    };
    context.Posts.Add(pst2);
    var pst3 = new PostEntity
    {
        UsuarioId = usr2.Id,
        Conteudo = $"Primeiro Post(post1) do Usuario : {usr2.Nome}",
        CreateAt = DateTime.Now
    };
    context.Posts.Add(pst3);
    context.SaveChanges();
}
