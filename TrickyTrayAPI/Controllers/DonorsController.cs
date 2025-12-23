using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Services;

namespace TrickyTrayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonorsController : ControllerBase
    {
        private readonly IDonorService _donorservice;

        public DonorsController(IDonorService donorservice)
        {
            _donorservice = donorservice;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetDonorDTO>>> GetAll()
        {
            var donors = await _donorservice.GetAllDonors();
            return Ok(donors);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CreateDonorDTO>> GetById(int id)
        {
            var donor = await _donorservice.GetDonorById(id);

            if (donor == null)
                return NotFound();

            return Ok(donor);
        }

        [HttpPost]
        public async Task<ActionResult<Donor>> Create(CreateDonorDTO donor)
        {
            var createdDonor = await _donorservice.AddDonor(donor);
            return CreatedAtAction(nameof(GetById), new { id = createdDonor.Id }, createdDonor);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Donor>> Update(int id, Donor donor)
        {
            if (id != donor.Id)
                return BadRequest();

            var exists = await _donorservice.ExistsAsync(id);
            if (!exists)
                return NotFound();

            var updatedDonor = await _donorservice.UpdateAsync(donor);
            return Ok(updatedDonor);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _donorservice.DeleteDonor(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}

