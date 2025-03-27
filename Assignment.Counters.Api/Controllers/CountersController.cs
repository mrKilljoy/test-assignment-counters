using System.ComponentModel.DataAnnotations;
using Assignment.Counters.Application.Interfaces;
using Assignment.Counters.Application.Models.Requests;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Assignment.Counters.Api.Controllers;

/// <summary>
/// The API used to work with counters.
/// </summary>
/// <remarks>
/// For the context, a counter is created per user, so each counter in a team is a single user.
/// </remarks>
[Route("api/[controller]")]
[ApiController]
public class CountersController : BaseController
{
    private readonly ICounterManager _counterManager;

    public CountersController(ICounterManager counterManager)
    {
        _counterManager = counterManager;
    }

    /// <summary>
    /// Provides information on a specific counter.
    /// </summary>
    /// <param name="id">The counter ID.</param>
    /// <returns>Data on the counter.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCounter([FromRoute] Guid id)
    {
        try
        {
            var result = await _counterManager.Get(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return SpecifyErrorResponse(ex);
        }
    }

    /// <summary>
    /// Creates a counter for a user.
    /// </summary>
    /// <param name="request">The request with information for a new counter.</param>
    /// <param name="validator">The request model validator.</param>
    /// <returns>Operation result.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateCounter(
        [FromBody][Required]NewCounterRequest request,
        [FromServices]IValidator<NewCounterRequest> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            
            var result = await _counterManager.Create(request.UserName, request.TeamId);
            return Created(new Uri(Request.GetEncodedUrl()+ "/" + result.Id), result.Id);
        }
        catch (Exception ex)
        {
            return SpecifyErrorResponse(ex);
        }
    }
    
    /// <summary>
    /// Increments a counter by specified number of steps.
    /// </summary>
    /// <param name="request">The request with counter information.</param>
    /// <returns>Operation result.</returns>
    [Route("{id}/increment")]
    [HttpPost]
    public async Task<IActionResult> Increment(
        [FromBody][Required]IncrementCounterRequest request,
        [FromServices]IValidator<IncrementCounterRequest> validator)
    {
        try
        {
            var validation = await validator.ValidateAsync(request);
            if (!validation.IsValid)
            {
                validation.AddToModelState(ModelState);
                return BadRequest(ModelState);
            }
            
            await _counterManager.Increment(request.CounterId, request.Value, request.LastTimestamp);
            return Ok();
        }
        catch (Exception ex)
        {
            return SpecifyErrorResponse(ex);
        }
    }
    
    /// <summary>
    /// Deletes a counter.
    /// </summary>
    /// <param name="id">The counter ID.</param>
    /// <returns>Operation result.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCounter([FromRoute]Guid id)
    {
        try
        {
            await _counterManager.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return SpecifyErrorResponse(ex);
        }
    }
}