using aspire_cosmos.ApiService.Data;
using aspire_cosmos.ApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace aspire_cosmos.ApiService.Endpoints;

public static class PersonEndpoints
{
    public static void MapPersonEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/people");

        group.MapGet("/", async (PersonDbContext db) =>
            await db.People.ToListAsync());

        group.MapGet("/{id}", async (string id, PersonDbContext db) =>
            await db.People.FindAsync(id) is Person person ? Results.Ok(person) : Results.NotFound());

        group.MapPost("/", async (Person person, PersonDbContext db) =>
        {
            db.People.Add(person);
            await db.SaveChangesAsync();
            return Results.Created($"/people/{person.Id}", person);
        });

        group.MapPut("/{id}", async (string id, Person input, PersonDbContext db) =>
        {
            var person = await db.People.FindAsync(id);
            if (person is null) return Results.NotFound();
            person.FirstName = input.FirstName;
            person.LastName = input.LastName;
            person.BirthDate = input.BirthDate;
            person.Sex = input.Sex;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        group.MapDelete("/{id}", async (string id, PersonDbContext db) =>
        {
            var person = await db.People.FindAsync(id);
            if (person is null) return Results.NotFound();
            db.People.Remove(person);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}
