using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Services;

namespace TrickyTrayAPI.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class DonorsController : ControllerBase
    {

        private readonly IDonorService _donorservice;

        public DonorsController(IDonorService donorservice)
        {
            _donorservice = donorservice;
        }
        //פונקציה שמביאה כמה תורמים יש לי בסך הכל



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
        public async Task<ActionResult<GetDonorDTO>> Update(int id, CreateDonorDTO donor)
        {
            var exists = await _donorservice.ExistsAsync(id);
            if (!exists)
                return NotFound();

            var updatedDonor = await _donorservice.UpdateAsync(id,donor);
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
       

        [HttpGet("{donorId}/with-gifts")]
        public async Task<ActionResult<GetDonorWithGiftsDTO>> GetDonorWithGifts(int donorId)
        {
            var dto = await _donorservice.GetDonorWithGiftsAsync(donorId);

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<GetDonorWithGiftsDTO>>> FilterDonors(
    [FromQuery] string? name,
    [FromQuery] string? email,
    [FromQuery] string? giftName)
        {

            var result = await _donorservice.FilterDonorsAsync(name, email, giftName);

            return Ok(result);
        }


    }
}

