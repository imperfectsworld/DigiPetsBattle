

using Digi_Pets_Battle.Models;
using Digi_Pets_Battle.Services;
using Microsoft.AspNetCore.Mvc;

namespace Digi_Pets_Battle.Controllers;

[Route("api/[controller]")]
[ApiController]

public class PetController : Controller
{
    private DigiPetsDbContext dbContext = new DigiPetsDbContext();
    private Random random = new Random();
    private readonly AccountDetailServices _service;

    public PetController(AccountDetailServices accountDetailServices)
    {
        _service = accountDetailServices;
    }

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
    public async Task<IActionResult> AddPet([FromHeader(Name = "X-API-Key")] string apiKey, [FromBody] Pet newPet)
    {
        AccountDetails account = await _service.PerformAction(apiKey, "CREATE");
        if (account == null)
        {
            return Unauthorized();
        }
        newPet.AccountId = account.id;
        newPet.Id = 0;
        dbContext.Pets.Add(newPet);
        dbContext.SaveChanges();
        return Created($"/api/Pet/{newPet.Id}", newPet);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> KillPet([FromHeader(Name = "X-API-Key")] string apiKey, int id)
    {
        Pet dead = dbContext.Pets.FirstOrDefault(pet => pet.Id == id);
        if (dead == null)
        {
            return NotFound("No matching Id");
        }
        AccountDetails account = await _service.GetAccountByApiKey(apiKey);
        if (account == null)
        {
            return Unauthorized();
        }
        if (dead.AccountId != account.id)
        {
            return Forbid();  // 403 Forbidden if the pet doesn't belong to the account
        }
        dbContext.Pets.Remove(dead);
            dbContext.SaveChanges();
            return NoContent();
        }


    [HttpPost("/{id}/Heal")]
    public async Task<IActionResult> AddHealthPet([FromHeader(Name = "X-API-Key")] string apiKey, int id)
    {

        Pet healthy = dbContext.Pets.FirstOrDefault(pet => pet.Id == id);
        if (healthy == null)
        {
            return NotFound("No matching Id");
        }
        AccountDetails account = await _service.PerformAction(apiKey, "HEAL");
        if (account == null)
        {
            return Unauthorized();
        }

        // Step 2: Ensure the pet belongs to the account
        if (healthy.AccountId != account.id)
        {
            return Forbid();
        }


        float healthyPet = (float)(random.NextDouble() * (0.3 - 0.1) + 0.1);
        healthy.Health = Math.Min(healthy.Health.GetValueOrDefault() + healthyPet, 1.0f); 
        dbContext.SaveChanges();
        
        return Ok(healthy);
        
    }
    [HttpPost("/{id}/Train")]
    public async Task<IActionResult> TrainPet([FromHeader(Name = "X-API-Key")] string apiKey, int id)
    {
        Pet train = dbContext.Pets.FirstOrDefault(pet => pet.Id == id);
        if (train == null)
        {
            return NotFound("No matching Id");
        }
        AccountDetails account = await _service.PerformAction(apiKey, "TRAIN");
        if (account == null)
        {
            return Unauthorized();
        }

        // Step 2: Ensure the pet belongs to the account
        if (train.AccountId != account.id)
        {
            return Forbid();
        }
        float randomNumber = random.Next(1, 4);
        train.Strength += randomNumber;
        dbContext.SaveChanges();
        return Ok(train);
    }


    [HttpPost("/{id}/Battle")]
    public async Task<IActionResult> BattlePets([FromHeader(Name = "X-API-Key")] string apiKey, int id, int opponentId)
    {
        BattleResult battle = new BattleResult();
        battle.win = false;
        battle.fighter1 = dbContext.Pets.FirstOrDefault(pet => pet.Id == id);
        battle.fighter2 = dbContext.Pets.FirstOrDefault(pet => pet.Id == opponentId);
        if (battle.fighter1 == null || battle.fighter2 == null)
        {
            return NotFound("No matching Id");
        }
        AccountDetails account = await _service.PerformAction(apiKey, "BATTLE");
        if (account == null)
        {
            return Unauthorized();
        }

        // Step 2: Ensure the pet belongs to the account
        if (battle.fighter1.AccountId != account.id || battle.fighter2.AccountId != account.id)
        {
            return Forbid();
        }
        float fighter1Ap = (float)(battle.fighter1.Health * battle.fighter1.Strength * battle.fighter1.Experience * (random.NextDouble()));
        
        float fighter2Ap = (float)(battle.fighter2.Health * battle.fighter2.Strength * battle.fighter2.Experience * (random.NextDouble()));

        float totalPower = fighter1Ap + fighter2Ap;

        float damageF1 = fighter1Ap / totalPower;
        float damageF2 = fighter2Ap / totalPower;

        battle.fighter1.Health = battle.fighter1.Health - (battle.fighter1.Health *  damageF2);
        battle.fighter2.Health = battle.fighter2.Health - (battle.fighter2.Health * damageF1);

        battle.fighter1.Experience += 1;
        battle.fighter2.Experience += 1;

        battle.win = fighter1Ap > fighter2Ap;

        return Ok(battle);
    }
}

