using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Midyaf.Core.DTOs;
using Midyaf.Core.Interfaces;
using Midyaf.Models;

namespace Midyaf.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public HotelController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [HttpGet("/Hotels")]
        public async Task<IActionResult> GetAllHotels()
        {
            var response = new GeneralResponse();
            var hotels = await unitOfWork.Hotels.GetAllAsync();
            if(hotels != null)
            {
                response.SetResponse("All hotels retrived successfully",true,Data:hotels);
                return Ok(response);
            }
            response.SetResponse("Error occured while retriving hotels", false);
            return BadRequest(response);
        }
        [HttpPost("/Hotel/add")]
        public async Task<IActionResult> Add(AddHotelDTO hotelDto)
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
    }
}
