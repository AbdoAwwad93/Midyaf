using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Midyaf.Core.DTOs;
using Midyaf.Core.Interfaces;
using Midyaf.Models;
using System.Security.Cryptography.X509Certificates;

namespace Midyaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public HotelController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [HttpGet("/hotel")]

        public async Task<IActionResult> GetAllHotels()
        {
            var response = new GeneralResponse();
            var hotels = await unitOfWork.Hotels.GetAllAsync();
            if (hotels != null)
            {
                response.SetResponse("All hotels retrived successfully", true, Data: hotels);
                return Ok(response);
            }
            response.SetResponse("Error occured while retriving hotels", false);
            return BadRequest(response);
        }
        [HttpPost("/hotel/add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(HotelDTO hotelDto)
        {
            var response = new GeneralResponse();
            if (!ModelState.IsValid)
            {
                response.SetResponse("Invalid Data", false);
                return BadRequest(response);
            }
            var hotel = mapper.Map<Hotel>(hotelDto);
            await unitOfWork.Hotels.AddAsync(hotel);
            await unitOfWork.SaveAsync();
            response.SetResponse("Hotel added successfully", true, hotel);

            return Ok(response);
        }
        [HttpPatch("/hotel/edit/{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, HotelDTO hotelDto)
        {
            var response = new GeneralResponse();
            if (!ModelState.IsValid)
            {
                response.SetResponse("invalid data", false);
                return BadRequest(response);
            }
            var hotel = await unitOfWork.Hotels.GetByIdAsync(id);
            if(hotel== null)
            {
                response.SetResponse("No hotel existed with this data", false);
                return BadRequest(response);
            }
            mapper.Map(hotelDto,hotel);
            await unitOfWork.Hotels.UpdateAsync(hotel);
            await unitOfWork.SaveAsync();
            response.SetResponse("Hotel edited successfully", true,Data:hotel);
            return Ok(response);
        }
        [HttpDelete("hotel/delete/{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var response  = new GeneralResponse();
            var hotel = await unitOfWork.Hotels.GetByIdAsync(id);
            if (hotel == null)
            {
                response.SetResponse("there is no hotel with this data", false);
                return BadRequest(response);
            }
            await unitOfWork.Hotels.RemoveAsync(hotel);
            await unitOfWork.SaveAsync();
            response.SetResponse("Hotel removed successfully", true);
            return Ok(response);
        }

    }
}
