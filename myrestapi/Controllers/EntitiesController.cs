namespace myrestapi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

[Route("api/entities")]
[ApiController]
public class EntitiesController : ControllerBase
{
    private static List<Entity> _mockDatabase = new List<Entity>
    {
        new Entity
        {
            Id = "1",
            Names = new List<Name>
            {
                new Name { FirstName = "John", MiddleName = "Doe", Surname = "Smith" }
            },
            Addresses = new List<Address>
            {
                new Address { AddressLine = "123 Main St", City = "Example City", Country = "USA" }
            },
            Dates = new List<Date>
            {
                new Date { DateType = "Birth", DateValue = new DateTime(1980, 1, 1) }
            },
            Deceased = false,
            Gender = "Male"
        },
        new Entity
        {
            Id = "2",
            Names = new List<Name>
            {
                new Name { FirstName = "Jane", MiddleName = "Doe", Surname = "Smith" }
            },
            Addresses = new List<Address>
            {
                new Address { AddressLine = "456 Oak St", City = "Another City", Country = "Canada" }
            },
            Dates = new List<Date>
            {
                new Date { DateType = "Birth", DateValue = new DateTime(1975, 5, 10) }
            },
            Deceased = true,
            Gender = "Female"
        }
    };

    // GET api/entities
    [HttpGet]
    public ActionResult<IEnumerable<Entity>> GetEntities(
        string search = "",
        bool? deceased = null,
        string gender = "",
        DateTime? startDate = null,
        DateTime? endDate = null,
        List<string>? countries = null)

    {
        IEnumerable<Entity> entities = _mockDatabase;
        if (!string.IsNullOrEmpty(search))
        {
            entities = entities.Where(e =>
                e.Names.Any(n =>
                    (n.FirstName != null && n.FirstName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (n.MiddleName != null && n.MiddleName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (n.Surname != null && n.Surname.Contains(search, StringComparison.OrdinalIgnoreCase))) ||
                e.Addresses?.Any(a =>
                    (a.Country != null && a.Country.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (a.AddressLine != null && a.AddressLine.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (a.City != null && a.City.Contains(search, StringComparison.OrdinalIgnoreCase))) == true);
        }
        
        if (deceased.HasValue)
        {
            entities = entities.Where(e => e.Deceased == deceased.Value);
        }

        if (!string.IsNullOrEmpty(gender))
        {
            entities = entities.Where(e => e.Gender != null && e.Gender.Equals(gender, StringComparison.OrdinalIgnoreCase));
        }

        if (startDate.HasValue)
        {
            entities = entities.Where(e =>
                e.Dates?.Any(d => d.DateValue.HasValue && d.DateValue.Value.Date >= startDate.Value.Date) == true ||
                e.Dates == null);
        }

        if (endDate.HasValue)
        {
            entities = entities.Where(e =>
                e.Dates?.Any(d => d.DateValue.HasValue && d.DateValue.Value.Date <= endDate.Value.Date) == true ||
                e.Dates == null);
        }

        if (countries != null && countries.Any())
        {
            entities = entities.Where(e =>
                e.Addresses?.Any(a =>
                    a.Country != null && countries.Contains(a.Country, StringComparer.OrdinalIgnoreCase)) == true ||
                e.Addresses == null);
        }

        return Ok(entities);
    }

    // GET api/entities/{id}
    [HttpGet("{id}")]
    public ActionResult<Entity> GetEntity(string id)
    {
        var entity = _mockDatabase.FirstOrDefault(e => e.Id == id);
        if (entity == null)
            return NotFound();
        return Ok(entity);
    }

    // POST api/entities
    [HttpPost]
    public ActionResult<Entity> CreateEntity(Entity entity)
    {
        entity.Id = Guid.NewGuid().ToString();
        _mockDatabase.Add(entity);
        return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, entity);
    }

    // PUT api/entities/{id}
    [HttpPut("{id}")]
    public ActionResult<Entity> UpdateEntity(string id, Entity updatedEntity)
    {
        var existingEntity = _mockDatabase.FirstOrDefault(e => e.Id == id);

        if (existingEntity == null)
            return NotFound();
        existingEntity.Addresses = updatedEntity.Addresses;
        existingEntity.Dates = updatedEntity.Dates;
        existingEntity.Deceased = updatedEntity.Deceased;
        existingEntity.Gender = updatedEntity.Gender;
        existingEntity.Names = updatedEntity.Names;
        return Ok(existingEntity);
    }

    // DELETE api/entities/{id}
    [HttpDelete("{id}")]
    public ActionResult DeleteEntity(string id)
    {
        var entity = _mockDatabase.FirstOrDefault(e => e.Id == id);
        if (entity == null)
            return NotFound();
        _mockDatabase.Remove(entity);
        return NoContent();
    }
}
