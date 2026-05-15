using Application.DTOs.Hotel.Request;
using Application.DTOs.Hotel.Response;
using Application.Interfaces.Hotel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UTE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HotelController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<HotelResponse>>> List(CancellationToken ct = default)
            => Ok(await _hotelService.GetAllAsync(ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<HotelResponse>> Get(int id, CancellationToken ct = default)
        {
            var res = await _hotelService.GetAsync(id, ct);
            return res is null ? NotFound() : Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult<HotelResponse>> Create([FromBody] HotelCreateRequest request, CancellationToken ct = default)
        {
            var created = await _hotelService.CreateAsync(request, ct);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] HotelUpdateRequest req, CancellationToken ct = default)
            => await _hotelService.UpdateAsync(id, req, ct) ? NoContent() : NotFound();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
            => await _hotelService.DeleteAsync(id, ct) ? NoContent() : NotFound();
    }
}

