using Digi_Pets_Battle.Models;
using Microsoft.AspNetCore.Mvc;

namespace Digi_Pets_Battle.Controllers;

[Route("api/[controller]")]
[ApiController]

public class PetController : Controller
{
    private DigiPetsDbContext dbContext = new DigiPetsDbContext();
    private Random random = new Random();
    // private readonly [NAME OF SERVICE] [service]

    [HttpGet()]
    public IActionResult GetAllPets(int? accountId = null)
    {
        List<Pet> result = dbContext.Pets.ToList();
        if (accountId != null)
        {
            result = result.Where(pet => pet.AccountId == accountId).ToList();
        }
        return Ok(result);
    }

    [HttpGet("{id}")]
    public IActionResult GetPetById(int id)
    {
        Pet digiPet = dbContext.Pets.FirstOrDefault(pets => pets.Id == id);
        if (digiPet == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(digiPet);
        }
    }

    [HttpPost()]
    public IActionResult AddPet([FromBody] Pet newPet)
    {
        newPet.Id = 0;
        dbContext.Pets.Add(newPet);
        dbContext.SaveChanges();
        return Created($"/api/Pet/{newPet.Id}", newPet);
    }

    [HttpDelete("{id}")]
    public IActionResult KillPet(int id)
    {
        Pet dead = dbContext.Pets.FirstOrDefault(pet => pet.Id == id);
        if (dead == null)
        {
            return NotFound("No matching Id");
        }
        else
        {
            dbContext.Pets.Remove(dead);
            dbContext.SaveChanges();
            return NoContent();
        }
    }

    [HttpPost("/{id}/Heal")]
    public IActionResult AddHealthPet(int id)
    {
        
        Pet healthy = dbContext.Pets.FirstOrDefault(pet => pet.Id == id);
        if (healthy == null)
        {
            return NotFound("No matching Id");
        }

        float healthyPet = (float)(random.NextDouble() * (0.3 - 0.1) + 0.1);
        healthy.Health = Math.Min(healthy.Health.GetValueOrDefault() + healthyPet, 1.0f); 
        dbContext.SaveChanges();
        
        return Ok(healthy);
        
    }

    [HttpPost("/{id}/Train")]
    public IActionResult TrainPet(int id)
    {
        Pet train = dbContext.Pets.FirstOrDefault(pet => pet.Id == id);
        if (train == null)
        {
            return NotFound("No matching Id");
        }

        return Ok(train);
    }

    [HttpPost("/{id}/Battle")]
    public IActionResult BattlePets(int id, int opponentId)
    {
        Pet fighter1 = dbContext.Pets.FirstOrDefault(pet => pet.Id == id);
        Pet fighter2 = dbContext.Pets.FirstOrDefault(pet => pet.Id == opponentId);
        if (fighter1 == null && fighter2 == null)
        {
            return NotFound("No matching Id");
        }
        return Ok(fighter1);
    }
}