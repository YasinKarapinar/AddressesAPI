using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AddressesAPI.Models;
using System;

namespace AddressesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly AddressContext _context;

        public AddressesController(AddressContext context)
        {
            _context = context;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressDTO>>> GetAddresses()
        {
            return await _context.Addresses
                .Select(a => AddressToDTO(a))
                .ToListAsync();
        }

        // GET: api/Addresses/5
        [HttpGet("{filterCriteria}")]
        public async Task<ActionResult<IEnumerable<AddressDTO>>> GetAddressesByCriteria(string filterColumn, string filterCriteria, string sort)
        {
            var addresses = new List<Address>();
            var addressDTOs = new List<AddressDTO>();
            IQueryable<Address> query = null;

            switch (filterColumn)
            {
                case "StreetName":
                    query = _context.Addresses.Where(a => a.StreetName.Contains(filterCriteria));
                    break;
                case "HouseNumber":
                    query = _context.Addresses.Where(a => a.HouseNumber.Equals(Int32.Parse(filterCriteria)));
                    break;
                case "PostalCode":
                    query = _context.Addresses.Where(a => a.PostalCode.Contains(filterCriteria));
                    break;
                case "City":
                    query = _context.Addresses.Where(a => a.City.Contains(filterCriteria));
                    break;
                case "Country":
                    query = _context.Addresses.Where(a => a.Country.Contains(filterCriteria));
                    break;
            }

            if(query == null)
            {
                return NotFound();
            }

            if (sort.Equals("asc"))
            {
                addressDTOs = await query.Select(a => AddressToDTO(a)).ToListAsync();
                return addressDTOs;
            }
            if (sort.Equals("desc"))
            {
                addresses = await query.OrderByDescending(a => a.Id).ToListAsync();

                foreach (Address a in addresses)
                {
                    addressDTOs.Add(AddressToDTO(a));
                }
                return addressDTOs;
            }
            else
            {
                return NotFound();
            }
        }

        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, AddressDTO addressDTO)
        {
            if (id != addressDTO.Id)
            {
                return BadRequest();
            }

            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }

            address.StreetName = addressDTO.StreetName;
            address.HouseNumber = addressDTO.HouseNumber;
            address.PostalCode = addressDTO.PostalCode;
            address.City = addressDTO.City;
            address.Country = addressDTO.Country;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!AddressExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<ActionResult<AddressDTO>> PostAddress(AddressDTO addressDTO)
        {
            var address = new Address
            {
                StreetName = addressDTO.StreetName,
                HouseNumber = addressDTO.HouseNumber,
                PostalCode = addressDTO.PostalCode,
                City = addressDTO.City,
                Country = addressDTO.Country
            };

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetAddresses),
                new { id = address.Id },
                AddressToDTO(address));
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);

            if (address == null)
            {
                return NotFound();
            }

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.Id == id);
        }

        private static AddressDTO AddressToDTO(Address address) =>
            new AddressDTO
            {
                Id = address.Id,
                StreetName = address.StreetName,
                HouseNumber = address.HouseNumber,
                PostalCode = address.PostalCode,
                City = address.City,
                Country = address.Country
            };
    }
}
